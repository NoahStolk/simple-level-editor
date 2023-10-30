using Detach.Parsers.Texture;
using Silk.NET.OpenGL;

namespace SimpleLevelEditor.Content;

public static class TextureLoader
{
	public static unsafe uint Load(TextureData texture)
	{
		uint textureId = Graphics.Gl.GenTexture();

		Graphics.Gl.BindTexture(TextureTarget.Texture2D, textureId);

		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);

		fixed (byte* b = texture.ColorData)
			Graphics.Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, texture.Width, texture.Height, 0, GLEnum.Rgba, PixelType.UnsignedByte, b);

		Graphics.Gl.GenerateMipmap(TextureTarget.Texture2D);

		return textureId;
	}
}
