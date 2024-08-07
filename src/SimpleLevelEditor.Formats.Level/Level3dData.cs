namespace SimpleLevelEditor.Formats.Level;

public sealed record Level3dData
{
	public required string? EntityConfigPath { get; set; }

	public required List<string> ModelPaths { get; set; }

	public required List<WorldObject> WorldObjects { get; set; }

	public required List<Entity> Entities { get; set; }

	public static Level3dData CreateDefault()
	{
		return new Level3dData
		{
			EntityConfigPath = null,
			ModelPaths = [],
			WorldObjects = [],
			Entities = [],
		};
	}

	public Level3dData DeepCopy()
	{
		return new Level3dData
		{
			EntityConfigPath = EntityConfigPath,
			ModelPaths = [..ModelPaths],
			WorldObjects = [..WorldObjects.Select(wo => wo.DeepCopy())],
			Entities = [..Entities.Select(e => e.DeepCopy())],
		};
	}

	public void AddModel(string modelPath)
	{
		if (ModelPaths.Contains(modelPath))
			return;

		ModelPaths.Add(modelPath);
	}

	public void AddWorldObject(WorldObject worldObject)
	{
		WorldObjects.Add(worldObject);
	}

	public void AddEntity(Entity entity)
	{
		Entities.Add(entity);
	}

	public void RemoveModel(string modelPath)
	{
		ModelPaths.Remove(modelPath);
	}

	public void RemoveWorldObject(WorldObject worldObject)
	{
		WorldObjects.Remove(worldObject);
	}

	public void RemoveEntity(Entity entity)
	{
		Entities.Remove(entity);
	}
}
