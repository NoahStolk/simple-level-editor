using Detach.Parsers.Model;
using Detach.Parsers.Model.MtlFormat;
using Detach.Parsers.Model.ObjFormat;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Rendering;

/*
model
	materials
		textures
	meshes
 */
public sealed class ModelContainer
{
	private readonly Dictionary<string, Model> _models = new();
	private readonly Dictionary<string, MeshPreviewFramebuffer> _meshPreviewFramebuffers = new();
	private readonly string _containerName;

	private ModelContainer(string containerName)
	{
		_containerName = containerName;
	}

	public static ModelContainer LevelContainer { get; } = new("Level");
	public static ModelContainer EntityConfigContainer { get; } = new("EntityConfig");

	public Model? GetModel(string path)
	{
		if (_models.TryGetValue(path, out Model? data))
			return data;

		DebugState.AddWarning($"Cannot find level model '{path}' in container '{_containerName}'.");
		return null;
	}

	public MeshPreviewFramebuffer? GetMeshPreviewFramebuffer(string path)
	{
		if (_meshPreviewFramebuffers.TryGetValue(path, out MeshPreviewFramebuffer? data))
			return data;

		DebugState.AddWarning($"Cannot find mesh preview framebuffer '{path}' in container '{_containerName}'.");
		return null;
	}

	public void Rebuild(string? sourceFilePath, List<string> modelPaths)
	{
		foreach (KeyValuePair<string, MeshPreviewFramebuffer> kvp in _meshPreviewFramebuffers)
			kvp.Value.Destroy();

		_models.Clear();
		_meshPreviewFramebuffers.Clear();

		string? levelDirectory = Path.GetDirectoryName(sourceFilePath);
		if (levelDirectory == null)
			return;

		// Load level meshes.
		foreach (string meshPath in modelPaths)
		{
			string absolutePath = Path.Combine(levelDirectory, meshPath);

			if (!File.Exists(absolutePath))
				continue;

			Model? entry = ReadModel(absolutePath);
			if (entry != null)
			{
				_models.Add(meshPath, entry);

				// TODO: Rewrite MeshPreviewFramebuffer to support multiple meshes as well.
				if (entry.Meshes.Count == 0)
					throw new InvalidOperationException("ReadModel should not return models with no meshes.");
				_meshPreviewFramebuffers.Add(meshPath, new MeshPreviewFramebuffer(entry.Meshes[0]));
			}
		}
	}

	private static Model? ReadModel(string absolutePath)
	{
		ModelData modelData = ObjParser.Parse(File.ReadAllBytes(absolutePath));
		if (modelData.Meshes.Count == 0)
			return null;

		string? directoryName = Path.GetDirectoryName(absolutePath);

		Dictionary<string, MaterialsData> allMaterials = [];
		foreach (string materialLibrary in modelData.MaterialLibraries)
		{
			string absolutePathToMtlFile = directoryName == null ? materialLibrary : Path.Combine(directoryName, materialLibrary);
			if (!File.Exists(absolutePathToMtlFile))
				continue;

			if (allMaterials.ContainsKey(materialLibrary))
				continue;

			MaterialsData materialsData = MtlParser.Parse(File.ReadAllBytes(absolutePathToMtlFile));
			allMaterials.Add(materialLibrary, materialsData);
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
			for (int i = 0; i < modelData.Meshes[0].Faces.Count; i += 3)
			{
				uint a = (ushort)(modelData.Meshes[0].Faces[i + 0].Position - 1);
				uint b = (ushort)(modelData.Meshes[0].Faces[i + 1].Position - 1);
				uint c = (ushort)(modelData.Meshes[0].Faces[i + 2].Position - 1);

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

			MaterialData? material = null;
			foreach (MaterialsData materials in allMaterials.Values)
			{
				material = materials.Materials.Find(m => m.Name == meshData.MaterialName);
				if (material != null)
					break;
			}

			if (material == null)
				material = new MaterialData("Default", Vector3.One, Vector3.One, Vector3.One, Vector3.One, 1, 1, 1, string.Empty);

			meshes.Add(new Mesh(geometry, new Material(material.DiffuseMap), vao, lineIndices.ToArray(), VaoUtils.CreateLineVao(modelData.Positions.ToArray()), boundingMin, boundingMax));
		}

		return new Model(allMaterials, meshes);
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
