using SimpleLevelEditor.Formats.Types.Level;

namespace SimpleLevelEditor.Formats.Level.Model;

public record Level3dData
{
	public required string? EntityConfigPath;
	public required List<string> Meshes;
	public required List<string> Textures;
	public required List<WorldObject> WorldObjects;
	public required List<Entity> Entities;

	public static Level3dData CreateDefault()
	{
		return new Level3dData
		{
			EntityConfigPath = null,
			Meshes = [],
			Textures = [],
			WorldObjects = [],
			Entities = [],
		};
	}

	public Level3dData DeepCopy()
	{
		List<string> newMeshes = [];
		newMeshes.AddRange(Meshes);

		List<string> newTextures = [];
		newTextures.AddRange(Textures);

		List<WorldObject> newWorldObjects = [];
		newWorldObjects.AddRange(WorldObjects.Select(t => t.DeepCopy()));

		List<Entity> newEntities = [];
		newEntities.AddRange(Entities.Select(t => t.DeepCopy()));

		return new Level3dData
		{
			EntityConfigPath = EntityConfigPath,
			Meshes = newMeshes,
			Textures = newTextures,
			WorldObjects = newWorldObjects,
			Entities = newEntities,
		};
	}
}
