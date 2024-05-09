using Silk.NET.OpenGL;
using static SimpleLevelEditor.Graphics;

namespace SimpleLevelEditor.Utils;

public static class VaoUtils
{
	public static unsafe uint CreateLineVao(Vector3[] vertices)
	{
		uint lineVao = Gl.GenVertexArray();
		Gl.BindVertexArray(lineVao);

		uint vbo = Gl.GenBuffer();
		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

		fixed (Vector3* v = &vertices[0])
			Gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(vertices.Length * sizeof(Vector3)), v, BufferUsageARB.StaticDraw);

		Gl.EnableVertexAttribArray(0);
		Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vector3), (void*)0);

		Gl.BindVertexArray(0);
		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		Gl.DeleteBuffer(vbo);

		return lineVao;
	}

	public static unsafe uint CreatePlaneVao(float[] vertexComponents)
	{
		uint planeVao = Gl.GenVertexArray();
		Gl.BindVertexArray(planeVao);

		uint vbo = Gl.GenBuffer();
		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

		const int vertexSize = 5 * sizeof(float);
		fixed (float* v = &vertexComponents[0])
			Gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(vertexComponents.Length * vertexSize), v, BufferUsageARB.StaticDraw);

		Gl.EnableVertexAttribArray(0);
		Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, vertexSize, (void*)0);

		Gl.EnableVertexAttribArray(1);
		Gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, vertexSize, (void*)(3 * sizeof(float)));

		Gl.BindVertexArray(0);
		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		Gl.DeleteBuffer(vbo);

		return planeVao;
	}
}
