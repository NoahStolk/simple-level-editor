namespace SimpleLevelEditor.Formats.EntityConfig;

public sealed record EntityConfigData
{
	public required List<string> ModelPaths { get; init; }

	public required List<string> TexturePaths { get; init; }

	public required List<EntityDescriptor> Entities { get; init; }

	public static EntityConfigData CreateDefault()
	{
		return new EntityConfigData
		{
			ModelPaths = [],
			TexturePaths = [],
			Entities = [],
		};
	}

	public EntityConfigData DeepCopy()
	{
		return new EntityConfigData
		{
			ModelPaths = [..ModelPaths],
			TexturePaths = [..TexturePaths],
			Entities = [..Entities.Select(e => e.DeepCopy())],
		};
	}

	public void AddModelPath(string path)
	{
		if (!ModelPaths.Contains(path))
			ModelPaths.Add(path);
	}

	public void AddTexturePath(string path)
	{
		if (!TexturePaths.Contains(path))
			TexturePaths.Add(path);
	}

	public void AddEntity(EntityDescriptor entity)
	{
		// TODO: Check for duplicate names.
		Entities.Add(entity);
	}

	public void RemoveModelPath(string path)
	{
		ModelPaths.Remove(path);
	}

	public void RemoveTexturePath(string path)
	{
		TexturePaths.Remove(path);
	}

	public void RemoveEntity(EntityDescriptor entity)
	{
		Entities.Remove(entity);
	}
}