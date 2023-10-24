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
		List<string> newMeshes = new();
		for (int i = 0; i < Meshes.Count; i++)
			newMeshes.Add(Meshes[i]);

		List<string> newTextures = new();
		for (int i = 0; i < Textures.Count; i++)
			newTextures.Add(Textures[i]);

		List<WorldObject> newWorldObjects = new();
		for (int i = 0; i < WorldObjects.Count; i++)
			newWorldObjects.Add(WorldObjects[i].DeepCopy());

		List<Entity> newEntities = new();
		for (int i = 0; i < Entities.Count; i++)
			newEntities.Add(Entities[i].DeepCopy());

		return new()
		{
			Version = Version,
			Meshes = newMeshes,
			Textures = newTextures,
			WorldObjects = newWorldObjects,
			Entities = newEntities,
		};
	}
}
