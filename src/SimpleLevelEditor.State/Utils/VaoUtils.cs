using Silk.NET.OpenGL;
using System.Numerics;

namespace SimpleLevelEditor.State.Utils;

public static class VaoUtils
{
	public static unsafe uint CreateLineVao(GL gl, Vector3[] vertices)
	{
		uint lineVao = gl.GenVertexArray();
		gl.BindVertexArray(lineVao);

		uint vbo = gl.GenBuffer();
		gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

		fixed (Vector3* v = &vertices[0])
			gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(vertices.Length * sizeof(Vector3)), v, BufferUsageARB.StaticDraw);

		gl.EnableVertexAttribArray(0);
		gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vector3), (void*)0);

		gl.BindVertexArray(0);
		gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		gl.DeleteBuffer(vbo);

		return lineVao;
	}

	public static unsafe uint CreatePlaneVao(GL gl, float[] vertexComponents)
	{
		uint planeVao = gl.GenVertexArray();
		gl.BindVertexArray(planeVao);

		uint vbo = gl.GenBuffer();
		gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

		const int vertexSize = 5 * sizeof(float);
		fixed (float* v = &vertexComponents[0])
			gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(vertexComponents.Length * vertexSize), v, BufferUsageARB.StaticDraw);

		gl.EnableVertexAttribArray(0);
		gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, vertexSize, (void*)0);

		gl.EnableVertexAttribArray(1);
		gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, vertexSize, (void*)(3 * sizeof(float)));

		gl.BindVertexArray(0);
		gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		gl.DeleteBuffer(vbo);

		return planeVao;
	}
}
