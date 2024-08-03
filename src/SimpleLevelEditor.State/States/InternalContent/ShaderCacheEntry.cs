using Silk.NET.OpenGL;

namespace SimpleLevelEditor.State.States.InternalContent;

public sealed class ShaderCacheEntry
{
	private readonly Dictionary<string, int> _uniformLocations = new();

	public ShaderCacheEntry(uint id)
	{
		Id = id;
	}

	public uint Id { get; }

	public int GetUniformLocation(GL gl, string name)
	{
		if (_uniformLocations.TryGetValue(name, out int location))
			return location;

		location = gl.GetUniformLocation(Id, name);
		_uniformLocations.Add(name, location);

		return location;
	}
}
