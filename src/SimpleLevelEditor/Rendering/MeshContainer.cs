using Silk.NET.OpenGL;
using SimpleLevelEditor.ContentParsers.Model;
using SimpleLevelEditor.ContentParsers.Model.ObjFormat;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Rendering;

public static class MeshContainer
{
	private static readonly Dictionary<string, (Mesh Mesh, uint Vao)> _meshes = new();

	public static (Mesh Mesh, uint Vao)? GetMesh(string name)
	{
		if (_meshes.TryGetValue(name, out (Mesh Mesh, uint Vao) data))
			return data;

		return null;
	}

	public static void Rebuild(string workingDirectory)
	{
		_meshes.Clear();

		foreach (string modelPath in LevelState.Level.Meshes)
		{
			string absolutePath = Path.Combine(workingDirectory, modelPath);

			if (!File.Exists(absolutePath))
				continue;

			ModelData modelData = ObjParser.Parse(File.ReadAllBytes(absolutePath));
			if (modelData.Meshes.Count == 0)
				continue;

			Mesh mainMesh = GetMesh(modelData, modelData.Meshes[0]);
			uint vao = CreateFromMesh(mainMesh);
			_meshes.Add(modelPath, (mainMesh, vao));
		}
	}

	private static Mesh GetMesh(ModelData modelData, MeshData meshData)
	{
		Vertex[] outVertices = new Vertex[meshData.Faces.Count];
		uint[] outFaces = new uint[meshData.Faces.Count];
		for (int j = 0; j < meshData.Faces.Count; j++)
		{
			ushort t = meshData.Faces[j].Texture;

			outVertices[j] = new(
			modelData.Positions[meshData.Faces[j].Position - 1],
			modelData.Textures.Count > t - 1 && t > 0 ? modelData.Textures[t - 1] : default, // TODO: Separate face type?
			modelData.Normals[meshData.Faces[j].Normal - 1]);
			outFaces[j] = (ushort)j;
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
}
