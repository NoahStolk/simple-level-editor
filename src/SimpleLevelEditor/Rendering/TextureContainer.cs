using Detach.Parsers.Texture;
using Detach.Parsers.Texture.TgaFormat;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Rendering;

public static class TextureContainer
{
	private static readonly Dictionary<string, uint> _levelTextures = new();
	private static readonly Dictionary<string, uint> _entityConfigTextures = new();

	public static uint? GetLevelTexture(string path)
	{
		if (_levelTextures.TryGetValue(path, out uint textureId))
			return textureId;

		DebugState.AddWarning($"Cannot find level texture '{path}'");
		return null;
	}

	public static uint? GetEntityConfigTexture(string path)
	{
		if (_entityConfigTextures.TryGetValue(path, out uint textureId))
			return textureId;

		DebugState.AddWarning($"Cannot find entity config texture '{path}'");
		return null;
	}

	public static void Rebuild(string? levelFilePath)
	{
		_levelTextures.Clear();
		_entityConfigTextures.Clear();

		// TODO: Why is this necessary?
		uint defaultTextureId = GlObjectUtils.CreateTexture(1, 1, [0xFF, 0xFF, 0xFF, 0xFF]);
		_levelTextures.Add(string.Empty, defaultTextureId);

		string? levelDirectory = Path.GetDirectoryName(levelFilePath);
		if (levelDirectory == null)
			return;

		LoadTextures(_levelTextures, levelDirectory, LevelState.Level.Textures.ToList());

		// TODO: Test if all the textures are loaded correctly if the entity config is in a completely different directory.
		if (LevelState.Level.EntityConfigPath != null)
		{
			string absoluteEntityConfigPath = Path.Combine(levelDirectory, LevelState.Level.EntityConfigPath.Value);
			string? entityConfigDirectory = Path.GetDirectoryName(absoluteEntityConfigPath);
			if (entityConfigDirectory == null)
				return;

			LoadTextures(_entityConfigTextures, entityConfigDirectory, EntityConfigState.EntityConfig.Textures.ToList());
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
