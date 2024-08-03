using Detach.Parsers.Texture;
using Silk.NET.OpenGL;

namespace SimpleLevelEditor.State.States.InternalContent;

public static class InternalContentState
{
	private static readonly Dictionary<string, uint> _textures = new();
	private static readonly Dictionary<string, ShaderCacheEntry> _shaders = new();

	public static IReadOnlyDictionary<string, uint> Textures => _textures;
	public static IReadOnlyDictionary<string, ShaderCacheEntry> Shaders => _shaders;

	public static void AddTexture(GL gl, string name, TextureData texture)
	{
		_textures.Add(name, TextureLoader.Load(gl, texture));
	}

	public static void AddShader(GL gl, string name, string vertexCode, string fragmentCode)
	{
		_shaders.Add(name, new ShaderCacheEntry(ShaderLoader.Load(gl, vertexCode, fragmentCode)));
	}
}
