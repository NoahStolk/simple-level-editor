namespace SimpleLevelEditor.Content;

public static class ShaderContainer
{
	private static readonly Dictionary<string, ShaderCacheEntry> _shaders = new();

	public static IReadOnlyDictionary<string, ShaderCacheEntry> Shaders => _shaders;

	public static void Add(string name, string vertexCode, string fragmentCode)
	{
		_shaders.Add(name, new(ShaderLoader.Load(vertexCode, fragmentCode)));
	}
}
