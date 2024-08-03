using Detach.Parsers.Texture;
using Silk.NET.OpenGL;

namespace SimpleLevelEditor.State.States.InternalContent;

internal static class TextureLoader
{
	public static unsafe uint Load(GL gl, TextureData texture)
	{
		uint textureId = gl.GenTexture();

		gl.BindTexture(TextureTarget.Texture2D, textureId);

		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);

		fixed (byte* b = texture.ColorData)
			gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, texture.Width, texture.Height, 0, GLEnum.Rgba, PixelType.UnsignedByte, b);

		gl.GenerateMipmap(TextureTarget.Texture2D);

		return textureId;
	}
}
