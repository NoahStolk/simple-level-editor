using Detach.Parsers.Model;
using Detach.Parsers.Model.ObjFormat;
using Silk.NET.OpenGL;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Rendering;

public static class ModelContainer
{
	private static readonly Dictionary<string, ModelEntry> _levelModels = new();
	private static readonly Dictionary<string, ModelEntry> _entityConfigModels = new();

	private static readonly Dictionary<string, MeshPreviewFramebuffer> _meshPreviewFramebuffers = new();

	public static ModelEntry? GetLevelModel(string path)
	{
		if (_levelModels.TryGetValue(path, out ModelEntry? data))
			return data;

		DebugState.AddWarning($"Cannot find level model '{path}'");
		return null;
	}

	public static ModelEntry? GetEntityConfigModel(string path)
	{
		if (_entityConfigModels.TryGetValue(path, out ModelEntry? data))
			return data;

		DebugState.AddWarning($"Cannot find entity config model '{path}'");
		return null;
	}

	public static MeshPreviewFramebuffer? GetMeshPreviewFramebuffer(string path)
	{
		if (_meshPreviewFramebuffers.TryGetValue(path, out MeshPreviewFramebuffer? data))
			return data;

		DebugState.AddWarning($"Cannot find mesh preview framebuffer '{path}'");
		return null;
	}

	public static void Rebuild(string? levelFilePath)
	{
		foreach (KeyValuePair<string, MeshPreviewFramebuffer> kvp in _meshPreviewFramebuffers)
			kvp.Value.Destroy();

		_levelModels.Clear();
		_entityConfigModels.Clear();

		_meshPreviewFramebuffers.Clear();

		string? levelDirectory = Path.GetDirectoryName(levelFilePath);
		if (levelDirectory == null)
			return;

		// Load level meshes.
		foreach (string meshPath in LevelState.Level.Meshes)
		{
			string absolutePath = Path.Combine(levelDirectory, meshPath);

			if (!File.Exists(absolutePath))
				continue;

			ModelEntry? entry = ReadModel(absolutePath);
			if (entry != null)
			{
				_levelModels.Add(meshPath, entry);

				// TODO: Rewrite MeshPreviewFramebuffer to support multiple meshes as well.
				if (entry.MeshEntries.Count == 0)
					throw new InvalidOperationException("ReadModel should not return models with no meshes.");
				_meshPreviewFramebuffers.Add(meshPath, new MeshPreviewFramebuffer(entry.MeshEntries[0]));
			}
		}

		// TODO: Test if all the textures are loaded correctly if the entity config is in a completely different directory.
		if (LevelState.Level.EntityConfigPath != null)
		{
			string absoluteEntityConfigPath = Path.Combine(levelDirectory, LevelState.Level.EntityConfigPath.Value);
			string? entityConfigDirectory = Path.GetDirectoryName(absoluteEntityConfigPath);
			if (entityConfigDirectory == null)
				return;

			foreach (string meshPath in EntityConfigState.EntityConfig.Meshes)
			{
				string absolutePath = Path.Combine(levelDirectory, meshPath);

				if (!File.Exists(absolutePath))
					continue;

				ModelEntry? entry = ReadModel(absolutePath);
				if (entry != null)
					_entityConfigModels.Add(meshPath, entry);
			}
		}
	}

	private static ModelEntry? ReadModel(string absolutePath)
	{
		ModelData modelData = ObjParser.Parse(File.ReadAllBytes(absolutePath));
		if (modelData.Meshes.Count == 0)
			return null;

		List<MeshEntry> meshes = [];
		foreach (MeshData meshData in modelData.Meshes)
		{
			Mesh mesh = GetMeshData(modelData, meshData);
			uint vao = CreateFromMesh(mesh);

			Vector3 boundingMin = new(float.MaxValue);
			Vector3 boundingMax = new(float.MinValue);
			foreach (Vector3 position in mesh.Vertices.Select(v => v.Position))
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
				int distinctNormals = edge.Value.Distinct(NormalComparer.Instance).Count();
				if (edge.Value.Count > 1 && distinctNormals == 1)
					continue;

				lineIndices.Add(edge.Key.A);
				lineIndices.Add(edge.Key.B);
			}

			// TODO: Add a material parser to Detach.

			meshes.Add(new MeshEntry(mesh, vao, lineIndices.ToArray(), VaoUtils.CreateLineVao(modelData.Positions.ToArray()), boundingMin, boundingMax));
		}

		return new ModelEntry(meshes);
	}

	private static void AddEdge(IDictionary<Edge, List<Vector3>> edges, Edge edge, Vector3 normal)
	{
		if (!edges.ContainsKey(edge))
			edges.Add(edge, []);

		edges[edge].Add(normal);
	}

	private static Mesh GetMeshData(ModelData modelData, MeshData meshData)
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

		return new Mesh(outVertices, outFaces);
	}

	private static unsafe uint CreateFromMesh(Mesh mesh)
	{
		uint vao = Graphics.Gl.GenVertexArray();
		uint vbo = Graphics.Gl.GenBuffer();

		Graphics.Gl.BindVertexArray(vao);

		Graphics.Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
		fixed (Vertex* v = &mesh.Vertices[0])
			Graphics.Gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(mesh.Vertices.Length * sizeof(Vertex)), v, BufferUsageARB.StaticDraw);

		Graphics.Gl.EnableVertexAttribArray(0);
		Graphics.Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)0);

		Graphics.Gl.EnableVertexAttribArray(1);
		Graphics.Gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(3 * sizeof(float)));

		Graphics.Gl.EnableVertexAttribArray(2);
		Graphics.Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(5 * sizeof(float)));

		Graphics.Gl.BindVertexArray(0);
		Graphics.Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		Graphics.Gl.DeleteBuffer(vbo);

		return vao;
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

	private sealed class NormalComparer : IEqualityComparer<Vector3>
	{
		public static readonly NormalComparer Instance = new();

		public bool Equals(Vector3 x, Vector3 y)
		{
			const float epsilon = 0.01f;
			return Math.Abs(x.X - y.X) < epsilon && Math.Abs(x.Y - y.Y) < epsilon && Math.Abs(x.Z - y.Z) < epsilon;
		}

		public int GetHashCode(Vector3 obj)
		{
			return obj.GetHashCode();
		}
	}
}
