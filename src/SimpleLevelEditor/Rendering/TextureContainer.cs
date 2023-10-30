using Detach.Parsers.Texture;
using Detach.Parsers.Texture.TgaFormat;
using Silk.NET.OpenGL;
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

		DebugState.AddWarning($"Cannot find texture '{path}'");
		return null;
	}

	public static void Rebuild(string? levelFilePath)
	{
		_textures.Clear();

		uint defaultTextureId = CreateFromTexture(1, 1, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
		_textures.Add(string.Empty, defaultTextureId);

		string? levelDirectory = Path.GetDirectoryName(levelFilePath);
		if (levelDirectory == null)
			return;

		foreach (string texturePath in LevelState.Level.Textures)
		{
			string absolutePath = Path.Combine(levelDirectory, texturePath);

			if (!File.Exists(absolutePath))
				continue;

			TextureData textureData = TgaParser.Parse(FileWrapper.ForceReadAllBytes(absolutePath));
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
