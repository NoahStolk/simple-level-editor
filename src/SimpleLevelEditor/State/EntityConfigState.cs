using SimpleLevelEditor.Formats;
using SimpleLevelEditor.Formats.Types.EntityConfig;
using SimpleLevelEditor.Formats.Types.Level;

namespace SimpleLevelEditor.State;

public static class EntityConfigState
{
	public static EntityConfigData EntityConfig { get; private set; } = EntityConfigData.CreateDefault();

	public static void LoadEntityConfig(string path)
	{
		using FileStream fs = new(path, FileMode.Open);
		EntityConfigData? entityConfig = SimpleLevelEditorJsonSerializer.DeserializeEntityConfig(fs);
		if (entityConfig == null)
		{
			DebugState.AddWarning("Failed to load entity config.");
			return;
		}

		EntityConfig = entityConfig;

		LevelEditorState.RenderFilter.Clear();
		LevelEditorState.RenderFilter.Add("WorldObjects", true);
		foreach (EntityDescriptor entity in entityConfig.Entities)
			LevelEditorState.RenderFilter.Add($"Entities:{entity.Name}", true);
	}

	public static EntityShape? GetEntityShape(Entity entity)
	{
		if (EntityConfig.Entities.Length == 0)
			return null; // EntityConfig not loaded yet.

		EntityShape? entityShape = EntityConfig.Entities.FirstOrDefault(e => e.Name == entity.Name)?.Shape;
		if (entityShape == null)
			throw new InvalidOperationException($"Entity '{entity.Name}' does not have a shape defined in the EntityConfig.");

		return entityShape;
	}
}
