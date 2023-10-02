namespace SimpleLevelEditor.Model;

public record Level3dData
{
	public required int Version;
	public required List<string> Meshes;
	public required List<string> Textures;
	public required List<WorldObject> WorldObjects;
	public required List<Entity> Entities;

	public static Level3dData Default => new()
	{
		Version = 1,
		Meshes = new(),
		Textures = new(),
		WorldObjects = new(),
		Entities = new(),
	};

	public Level3dData DeepCopy()
	{
		return new()
		{
			Version = Version,
			Meshes = new(Meshes),
			Textures = new(Textures),
			WorldObjects = new(WorldObjects),
			Entities = new(Entities),
		};
	}
}
