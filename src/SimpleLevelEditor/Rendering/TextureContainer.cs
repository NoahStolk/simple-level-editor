using Detach.Parsers.Texture;
using Detach.Parsers.Texture.TgaFormat;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Rendering;

// TODO: Rewrite this class; instead of manually rebuilding by hand, just request the texture and load it from the full file path if it doesn't yet exist.
// TODO: We would need a robust way to resolve file paths from:
// - The entity config file.
// - The level file.
// - The .obj file.
// - The .mtl file.
public static class TextureContainer
{
	private static readonly Dictionary<string, uint> _textures = new();

	public static uint? GetTexture(string path)
	{
		if (_textures.TryGetValue(path, out uint textureId))
			return textureId;

		DebugState.AddWarning($"Cannot find entity config texture '{path}'");
		return null;
	}

	public static void Rebuild(string? levelFilePath)
	{
		_textures.Clear();

		// TODO: Why is this necessary?
		uint defaultTextureId = GlObjectUtils.CreateTexture(1, 1, [0xFF, 0xFF, 0xFF, 0xFF]);
		_textures.Add(string.Empty, defaultTextureId);

		string? levelDirectory = Path.GetDirectoryName(levelFilePath);
		if (levelDirectory == null)
			return;

		// TODO: Test if all the textures are loaded correctly if the entity config is in a completely different directory.
		if (LevelState.Level.EntityConfigPath != null)
		{
			string absoluteEntityConfigPath = Path.Combine(levelDirectory, LevelState.Level.EntityConfigPath.Value);
			string? entityConfigDirectory = Path.GetDirectoryName(absoluteEntityConfigPath);
			if (entityConfigDirectory == null)
				return;

			LoadTextures(_textures, entityConfigDirectory, EntityConfigState.EntityConfig.Textures.ToList());
		}

		void LoadTextures(Dictionary<string, uint> textureDictionary, string directoryPath, List<string> texturePaths)
		{
			foreach (string texturePath in texturePaths)
			{
				string absolutePath = Path.Combine(directoryPath, texturePath);

				if (!File.Exists(absolutePath))
					continue;

				TextureData textureData = TgaParser.Parse(File.ReadAllBytes(absolutePath));
				uint textureId = GlObjectUtils.CreateTexture(textureData.Width, textureData.Height, textureData.ColorData);
				textureDictionary.Add(texturePath, textureId);
			}
		}
	}
}
