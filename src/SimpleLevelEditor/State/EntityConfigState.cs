using SimpleLevelEditor.Formats.EntityConfig;
using SimpleLevelEditor.Formats.EntityConfig.Model;
using System.Xml;

namespace SimpleLevelEditor.State;

public static class EntityConfigState
{
	public static EntityConfigData EntityConfig { get; private set; } = EntityConfigData.CreateDefault();

	public static void LoadEntityConfig(string path)
	{
		using FileStream fs = new(path, FileMode.Open);
		using XmlReader reader = XmlReader.Create(fs);
		EntityConfig = EntityConfigXmlDeserializer.ReadEntityConfig(reader);

		LevelEditorState.RenderFilter.Clear();
		LevelEditorState.RenderFilter.Add("WorldObjects", true);
		foreach (EntityDescriptor entity in EntityConfig.Entities)
			LevelEditorState.RenderFilter.Add($"Entities:{entity.Name}", true);
	}
}
