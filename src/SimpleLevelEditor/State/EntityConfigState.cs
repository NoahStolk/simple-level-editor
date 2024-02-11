using SimpleLevelEditor.Formats;
using SimpleLevelEditor.Model.EntityConfig;
using System.Xml;

namespace SimpleLevelEditor.State;

public static class EntityConfigState
{
	public static EntityConfigData EntityConfig { get; private set; } = EntityConfigData.CreateDefault();

	public static void LoadEntityConfig(string path)
	{
		using FileStream fs = new(path, FileMode.Open);
		using XmlReader reader = XmlReader.Create(fs);
		EntityConfig = XmlFormatSerializer.ReadEntityConfig(reader);
	}
}
