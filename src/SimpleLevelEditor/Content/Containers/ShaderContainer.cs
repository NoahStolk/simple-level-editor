using SimpleLevelEditor.Content.Loaders;

namespace SimpleLevelEditor.Content.Containers;

public static class ShaderContainer
{
	private static readonly Dictionary<string, uint> _shaders = new();

	public static IReadOnlyDictionary<string, uint> Shaders => _shaders;

	public static void Add(string name, string vertexCode, string fragmentCode)
	{
		_shaders.Add(name, ShaderLoader.Load(vertexCode, fragmentCode));
	}
}
