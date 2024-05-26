using Microsoft.FSharp.Core;
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
		FSharpOption<EntityConfigData>? entityConfig = SimpleLevelEditorJsonSerializer.DeserializeEntityConfigFromStream(fs);
		if (entityConfig == null)
		{
			DebugState.AddWarning("Failed to load entity config.");
			return;
		}

		EntityConfig = entityConfig.Value;

		LevelEditorState.RenderFilter.Clear();
		LevelEditorState.RenderFilter.Add("WorldObjects", true);
		foreach (EntityDescriptor entity in EntityConfig.Entities)
			LevelEditorState.RenderFilter.Add($"Entities:{entity.Name}", true);
	}

	public static EntityShapeDescriptor? GetEntityShapeDescriptor(Entity entity)
	{
		if (EntityConfig.Entities.Length == 0)
			return null; // EntityConfig not loaded yet.

		EntityShapeDescriptor? entityShapeDescriptor = EntityConfig.Entities.FirstOrDefault(e => e.Name == entity.Name)?.Shape;
		if (entityShapeDescriptor == null)
			throw new InvalidOperationException($"Entity '{entity.Name}' does not have a shape defined in the EntityConfig.");

		return entityShapeDescriptor;
	}
}
