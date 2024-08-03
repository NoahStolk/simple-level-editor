using Silk.NET.OpenGL;
using SimpleLevelEditor.State.States.Assets;

namespace SimpleLevelEditor.State.Utils;

internal static class GlObjectUtils
{
	public static unsafe uint CreateTexture(GL gl, uint width, uint height, byte[] pixels)
	{
		uint textureId = gl.GenTexture();

		gl.BindTexture(TextureTarget.Texture2D, textureId);

		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);

		fixed (byte* b = pixels)
			gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, width, height, 0, GLEnum.Rgba, PixelType.UnsignedByte, b);

		gl.GenerateMipmap(TextureTarget.Texture2D);

		return textureId;
	}

	public static unsafe uint CreateMesh(GL gl, Geometry geometry)
	{
		uint vao = gl.GenVertexArray();
		uint vbo = gl.GenBuffer();

		gl.BindVertexArray(vao);

		gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
		fixed (PositionTextureNormal* v = &geometry.Vertices[0])
			gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(geometry.Vertices.Length * sizeof(PositionTextureNormal)), v, BufferUsageARB.StaticDraw);

		gl.EnableVertexAttribArray(0);
		gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(PositionTextureNormal), (void*)0);

		gl.EnableVertexAttribArray(1);
		gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(PositionTextureNormal), (void*)(3 * sizeof(float)));

		gl.EnableVertexAttribArray(2);
		gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)sizeof(PositionTextureNormal), (void*)(5 * sizeof(float)));

		gl.BindVertexArray(0);
		gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		gl.DeleteBuffer(vbo);

		return vao;
	}
}
