using Silk.NET.OpenGL;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.Rendering.Vertices;

namespace SimpleLevelEditor.Utils;

public static class GlObjectUtils
{
	public static unsafe uint CreateTexture(uint width, uint height, byte[] pixels)
	{
		uint textureId = Graphics.Gl.GenTexture();

		Graphics.Gl.BindTexture(TextureTarget.Texture2D, textureId);

		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);

		fixed (byte* b = pixels)
			Graphics.Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, width, height, 0, GLEnum.Rgba, PixelType.UnsignedByte, b);

		Graphics.Gl.GenerateMipmap(TextureTarget.Texture2D);

		return textureId;
	}

	public static unsafe uint CreateMesh(Geometry geometry)
	{
		uint vao = Graphics.Gl.GenVertexArray();
		uint vbo = Graphics.Gl.GenBuffer();

		Graphics.Gl.BindVertexArray(vao);

		Graphics.Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);
		fixed (PositionTextureNormal* v = &geometry.Vertices[0])
			Graphics.Gl.BufferData(BufferTargetARB.ArrayBuffer, (uint)(geometry.Vertices.Length * sizeof(PositionTextureNormal)), v, BufferUsageARB.StaticDraw);

		Graphics.Gl.EnableVertexAttribArray(0);
		Graphics.Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(PositionTextureNormal), (void*)0);

		Graphics.Gl.EnableVertexAttribArray(1);
		Graphics.Gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, (uint)sizeof(PositionTextureNormal), (void*)(3 * sizeof(float)));

		Graphics.Gl.EnableVertexAttribArray(2);
		Graphics.Gl.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, (uint)sizeof(PositionTextureNormal), (void*)(5 * sizeof(float)));

		Graphics.Gl.BindVertexArray(0);
		Graphics.Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		Graphics.Gl.DeleteBuffer(vbo);

		return vao;
	}
}
