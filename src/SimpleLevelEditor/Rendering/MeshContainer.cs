using Detach.Parsers.Model;
using Detach.Parsers.Model.ObjFormat;
using Silk.NET.OpenGL;
using SimpleLevelEditor.Content.Data;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Rendering;

public static class MeshContainer
{
	private static readonly Dictionary<string, Entry> _meshes = new();

	public static Entry? GetMesh(string path)
	{
		if (_meshes.TryGetValue(path, out Entry? data))
			return data;

		DebugState.AddWarning($"Cannot find mesh '{path}'");
		return null;
	}

	public static void Rebuild(string? levelFilePath)
	{
		_meshes.Clear();

		string? levelDirectory = Path.GetDirectoryName(levelFilePath);
		if (levelDirectory == null)
			return;

		foreach (string modelPath in LevelState.Level.Meshes)
		{
			string absolutePath = Path.Combine(levelDirectory, modelPath);

			if (!File.Exists(absolutePath))
				continue;

			ModelData modelData = ObjParser.Parse(File.ReadAllBytes(absolutePath));
			if (modelData.Meshes.Count == 0)
				continue;

			Mesh mainMesh = GetMesh(modelData, modelData.Meshes[0]);
			uint vao = CreateFromMesh(mainMesh);

			Vector3 boundingMin = new(float.MaxValue);
			Vector3 boundingMax = new(float.MinValue);
			foreach (Vector3 position in mainMesh.Vertices.Select(v => v.Position))
			{
				boundingMin = Vector3.Min(boundingMin, position);
				boundingMax = Vector3.Max(boundingMax, position);
			}

			// Find main mesh edges.
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

				AddEdge(edges, new(a, b), normal);
				AddEdge(edges, new(b, c), normal);
				AddEdge(edges, new(c, a), normal);
			}

			// Find edges that are only used by one triangle.
			List<uint> lineIndices = new();
			foreach (KeyValuePair<Edge, List<Vector3>> edge in edges)
			{
				int distinctNormals = edge.Value.Distinct().Count();
				if (edge.Value.Count > 1 && distinctNormals == 1)
					continue;

				lineIndices.Add(edge.Key.A);
				lineIndices.Add(edge.Key.B);
			}

			_meshes.Add(modelPath, new(mainMesh, vao, lineIndices.ToArray(), VaoUtils.CreateLineVao(modelData.Positions.ToArray()), boundingMin, boundingMax));
		}

		void AddEdge(IDictionary<Edge, List<Vector3>> edges, Edge d, Vector3 normal)
		{
			if (!edges.ContainsKey(d))
				edges.Add(d, new());

			edges[d].Add(normal);
		}
	}

	private static Mesh GetMesh(ModelData modelData, MeshData meshData)
	{
		// TODO: Do not duplicate vertices.
		Vertex[] outVertices = new Vertex[meshData.Faces.Count];
		uint[] outFaces = new uint[meshData.Faces.Count];
		for (int j = 0; j < meshData.Faces.Count; j++)
		{
			ushort t = meshData.Faces[j].Texture;

			outVertices[j] = new(
				modelData.Positions[meshData.Faces[j].Position - 1],
				modelData.Textures.Count > t - 1 && t > 0 ? modelData.Textures[t - 1] : default, // TODO: Separate face type?
				modelData.Normals[meshData.Faces[j].Normal - 1]);
			outFaces[j] = (uint)j;
		}

		return new(outVertices, outFaces);
	}

	private static unsafe uint CreateFromMesh(Mesh mesh)
	{
		uint vao = Graphics.Gl.GenVertexArray();
		Graphics.Gl.BindVertexArray(vao);

		uint vbo = Graphics.Gl.GenBuffer();
		Graphics.Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

		GlUtils.BufferData(BufferTargetARB.ArrayBuffer, mesh.Vertices, BufferUsageARB.StaticDraw);

		Graphics.Gl.EnableVertexAttribArray(0);
		Graphics.Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)0);

		Graphics.Gl.EnableVertexAttribArray(1);
		Graphics.Gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(Vertex), (void*)(3 * sizeof(float)));

		Graphics.Gl.BindVertexArray(0);
		Graphics.Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		Graphics.Gl.DeleteBuffer(vbo);

		return vao;
	}

	public record Entry(Mesh Mesh, uint MeshVao, uint[] LineIndices, uint LineVao, Vector3 BoundingMin, Vector3 BoundingMax);

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
