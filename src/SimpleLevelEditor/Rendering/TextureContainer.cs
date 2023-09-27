using Silk.NET.OpenGL;
using SimpleLevelEditor.Content.Parsers.Texture;
using SimpleLevelEditor.Content.Parsers.Texture.TgaFormat;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Rendering;

public static class TextureContainer
{
	private static readonly Dictionary<string, uint> _textures = new();

	public static uint? GetTexture(string path)
	{
		if (_textures.TryGetValue(path, out uint textureId))
			return textureId;

		return null;
	}

	public static void Rebuild(string workingDirectory)
	{
		_textures.Clear();

		foreach (string texturePath in LevelState.Level.Textures)
		{
			string absolutePath = Path.Combine(workingDirectory, texturePath);

			if (!File.Exists(absolutePath))
				continue;

			TextureData textureData = TgaParser.Parse(File.ReadAllBytes(absolutePath));
			uint textureId = CreateFromTexture(textureData.Width, textureData.Height, textureData.ColorData);
			_textures.Add(texturePath, textureId);
		}
	}

	private static uint CreateFromTexture(uint width, uint height, byte[] pixels)
	{
		uint textureId = Graphics.Gl.GenTexture();

		Graphics.Gl.BindTexture(TextureTarget.Texture2D, textureId);

		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);

		GlUtils.TexImageRgba2D(width, height, pixels);

		Graphics.Gl.GenerateMipmap(TextureTarget.Texture2D);

		return textureId;
	}
}
