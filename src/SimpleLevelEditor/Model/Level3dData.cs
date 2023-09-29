namespace SimpleLevelEditor.Model;

public class Level3dData
{
	public required int Version { get; set; }

	public required List<string> Meshes { get; set; }

	public required List<string> Textures { get; set; }

	public required List<WorldObject> WorldObjects { get; set; }

	public required List<Entity> Entities { get; set; }

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
