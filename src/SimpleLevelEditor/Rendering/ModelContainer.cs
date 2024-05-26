using Detach.Parsers.Model;
using Detach.Parsers.Model.MtlFormat;
using Detach.Parsers.Model.ObjFormat;
using Detach.Parsers.Texture;
using Detach.Parsers.Texture.TgaFormat;
using SimpleLevelEditor.State.Messages;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Rendering;

public sealed class ModelContainer
{
	private readonly Dictionary<string, Model> _models = new();
	private readonly Dictionary<string, ModelPreviewFramebuffer> _modelPreviewFramebuffers = new();
	private readonly string _containerName;
	private readonly bool _addModelPreviewFramebuffers;

	private ModelContainer(string containerName, bool addModelPreviewFramebuffers)
	{
		_containerName = containerName;
		_addModelPreviewFramebuffers = addModelPreviewFramebuffers;
	}

	public static ModelContainer LevelContainer { get; } = new("Level", true);
	public static ModelContainer EntityConfigContainer { get; } = new("EntityConfig", false);

	public Model? GetModel(string path)
	{
		if (_models.TryGetValue(path, out Model? model))
			return model;

		MessagesState.AddWarning($"Cannot find level model '{path}' in container '{_containerName}'.");
		return null;
	}

	public ModelPreviewFramebuffer? GetModelPreviewFramebuffer(string path)
	{
		if (_modelPreviewFramebuffers.TryGetValue(path, out ModelPreviewFramebuffer? modelPreviewFramebuffer))
			return modelPreviewFramebuffer;

		MessagesState.AddWarning($"Cannot find model preview framebuffer '{path}' in container '{_containerName}'.");
		return null;
	}

	public void Rebuild(string? sourceFilePath, List<string> modelPaths)
	{
		if (_addModelPreviewFramebuffers)
		{
			foreach (KeyValuePair<string, ModelPreviewFramebuffer> kvp in _modelPreviewFramebuffers)
				kvp.Value.Destroy();
			_modelPreviewFramebuffers.Clear();
		}

		_models.Clear();

		string? levelDirectory = Path.GetDirectoryName(sourceFilePath);
		if (levelDirectory == null)
			return;

		foreach (string modelPath in modelPaths)
		{
			string absolutePath = Path.Combine(levelDirectory, modelPath);

			if (!File.Exists(absolutePath))
				continue;

			Model? model = ReadModel(absolutePath);
			if (model != null)
			{
				_models.Add(modelPath, model);

				if (_addModelPreviewFramebuffers)
					_modelPreviewFramebuffers.Add(modelPath, new ModelPreviewFramebuffer(model));
			}
		}
	}

	private static Model? ReadModel(string absolutePathToObjFile)
	{
		ModelData modelData = ObjParser.Parse(File.ReadAllBytes(absolutePathToObjFile));
		if (modelData.Meshes.Count == 0)
			return null;

		string? directoryName = Path.GetDirectoryName(absolutePathToObjFile);

		Dictionary<string, MaterialLibrary> allMaterials = [];
		foreach (string materialLibrary in modelData.MaterialLibraries)
		{
			string absolutePathToMtlFile = directoryName == null ? materialLibrary : Path.Combine(directoryName, materialLibrary);
			if (!File.Exists(absolutePathToMtlFile))
				continue;

			if (allMaterials.ContainsKey(materialLibrary))
				continue;

			MaterialsData materialsData = MtlParser.Parse(File.ReadAllBytes(absolutePathToMtlFile));
			allMaterials.Add(materialLibrary, new MaterialLibrary(absolutePathToMtlFile, materialsData.Materials.ConvertAll(m =>
			{
				string mtlDirectory = Path.GetDirectoryName(absolutePathToMtlFile) ?? throw new InvalidOperationException("MTL file is not in a directory.");
				string absolutePathToDiffuseMap = Path.Combine(mtlDirectory, m.DiffuseMap);
				TextureData textureData = TgaParser.Parse(File.ReadAllBytes(absolutePathToDiffuseMap));
				return new Material(m.Name, new Map(absolutePathToDiffuseMap, textureData));
			})));
		}

		List<Mesh> meshes = [];
		foreach (MeshData meshData in modelData.Meshes)
		{
			Geometry geometry = GetMeshData(modelData, meshData);
			uint vao = GlObjectUtils.CreateMesh(geometry);

			Vector3 boundingMin = new(float.MaxValue);
			Vector3 boundingMax = new(float.MinValue);
			foreach (Vector3 position in geometry.Vertices.Select(v => v.Position))
			{
				boundingMin = Vector3.Min(boundingMin, position);
				boundingMax = Vector3.Max(boundingMax, position);
			}

			// Find mesh edges.
			Dictionary<Edge, List<Vector3>> edges = new();
			for (int i = 0; i < meshData.Faces.Count; i += 3)
			{
				uint a = (ushort)(meshData.Faces[i + 0].Position - 1);
				uint b = (ushort)(meshData.Faces[i + 1].Position - 1);
				uint c = (ushort)(meshData.Faces[i + 2].Position - 1);

				Vector3 positionA = modelData.Positions[(int)a];
				Vector3 positionB = modelData.Positions[(int)b];
				Vector3 positionC = modelData.Positions[(int)c];
				Vector3 normal = Vector3.Normalize(Vector3.Cross(positionB - positionA, positionC - positionA));
				if (float.IsNaN(normal.X) || float.IsNaN(normal.Y) || float.IsNaN(normal.Z))
					continue;

				AddEdge(edges, new Edge(a, b), normal);
				AddEdge(edges, new Edge(b, c), normal);
				AddEdge(edges, new Edge(c, a), normal);
			}

			// Find edges that are only used by one triangle.
			List<uint> lineIndices = [];
			foreach (KeyValuePair<Edge, List<Vector3>> edge in edges)
			{
				int distinctNormals = edge.Value.Distinct(NormalVectorComparer.Instance).Count();
				if (edge.Value.Count > 1 && distinctNormals == 1)
					continue;

				lineIndices.Add(edge.Key.A);
				lineIndices.Add(edge.Key.B);
			}

			meshes.Add(new Mesh(geometry, meshData.MaterialName, vao, lineIndices.ToArray(), VaoUtils.CreateLineVao(modelData.Positions.ToArray()), boundingMin, boundingMax));
		}

		Vector3 modelBoundingMin = new(float.MaxValue);
		Vector3 modelBoundingMax = new(float.MinValue);
		foreach (Mesh mesh in meshes)
		{
			modelBoundingMin = Vector3.Min(modelBoundingMin, mesh.BoundingMin);
			modelBoundingMax = Vector3.Max(modelBoundingMax, mesh.BoundingMax);
		}

		Vector3 modelBoundingCenter = (modelBoundingMin + modelBoundingMax) / 2;
		float modelBoundingRadius = Vector3.Distance(modelBoundingCenter, modelBoundingMax);
		return new Model(absolutePathToObjFile, allMaterials, meshes, modelBoundingCenter, modelBoundingRadius);
	}

	private static void AddEdge(IDictionary<Edge, List<Vector3>> edges, Edge edge, Vector3 normal)
	{
		if (!edges.ContainsKey(edge))
			edges.Add(edge, []);

		edges[edge].Add(normal);
	}

	private static Geometry GetMeshData(ModelData modelData, MeshData meshData)
	{
		Vertex[] outVertices = new Vertex[meshData.Faces.Count];
		uint[] outFaces = new uint[meshData.Faces.Count];
		for (int i = 0; i < meshData.Faces.Count; i++)
		{
			ushort v = meshData.Faces[i].Position;
			ushort vt = meshData.Faces[i].Texture;
			ushort vn = meshData.Faces[i].Normal;

			Vector3 position = modelData.Positions.Count > v - 1 && v > 0 ? modelData.Positions[v - 1] : default;
			Vector2 texture = modelData.Textures.Count > vt - 1 && vt > 0 ? modelData.Textures[vt - 1] : default;
			Vector3 normal = modelData.Normals.Count > vn - 1 && vn > 0 ? modelData.Normals[vn - 1] : default;

			texture = texture with { Y = 1 - texture.Y };

			outVertices[i] = new Vertex(position, texture, normal);
			outFaces[i] = (uint)i;
		}

		return new Geometry(outVertices, outFaces);
	}

	private sealed record Edge(uint A, uint B)
	{
		public bool Equals(Edge? other)
		{
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;

			return A == other.A && B == other.B || A == other.B && B == other.A;
		}

		public override int GetHashCode()
		{
			return A < B ? HashCode.Combine(A, B) : HashCode.Combine(B, A);
		}
	}
}
