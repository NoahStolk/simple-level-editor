using Detach.Parsers.Texture;

namespace SimpleLevelEditor.Content;

public static class InternalContent
{
	private static readonly Dictionary<string, uint> _textures = new();
	private static readonly Dictionary<string, ShaderCacheEntry> _shaders = new();

	public static IReadOnlyDictionary<string, uint> Textures => _textures;
	public static IReadOnlyDictionary<string, ShaderCacheEntry> Shaders => _shaders;

	public static void AddTexture(string name, TextureData texture)
	{
		_textures.Add(name, TextureLoader.Load(texture));
	}

	public static void AddShader(string name, string vertexCode, string fragmentCode)
	{
		_shaders.Add(name, new ShaderCacheEntry(ShaderLoader.Load(vertexCode, fragmentCode)));
	}
}
