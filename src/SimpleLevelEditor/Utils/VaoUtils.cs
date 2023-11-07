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
}
