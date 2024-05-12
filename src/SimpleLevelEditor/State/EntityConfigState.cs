using SimpleLevelEditor.Formats;
using SimpleLevelEditor.Formats.Types.EntityConfig;
using System.Text.Json;

namespace SimpleLevelEditor.State;

public static class EntityConfigState
{
	public static EntityConfigData EntityConfig { get; private set; } = EntityConfigData.CreateDefault();

	public static void LoadEntityConfig(string path)
	{
		using FileStream fs = new(path, FileMode.Open);
		EntityConfig = JsonSerializer.Deserialize<EntityConfigData>(fs, SimpleLevelEditorJsonSerializer.DefaultSerializerOptions) ?? throw new InvalidOperationException("EntityConfig JSON is not valid.");

		LevelEditorState.RenderFilter.Clear();
		LevelEditorState.RenderFilter.Add("WorldObjects", true);
		foreach (EntityDescriptor entity in EntityConfig.Entities)
			LevelEditorState.RenderFilter.Add($"Entities:{entity.Name}", true);
	}
}
