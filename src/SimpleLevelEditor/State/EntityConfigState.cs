using SimpleLevelEditor.Formats;
using SimpleLevelEditor.Formats.Types.EntityConfig;

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
}
