using SimpleLevelEditor.Formats.EntityConfig.Model;
using System.Xml;
using System.Xml.Serialization;

namespace SimpleLevelEditor.Formats.EntityConfig;

public static class EntityConfigXmlDeserializer
{
	private static readonly InvalidDataException _invalidFormat = new("Invalid format");

	public static EntityConfigData ReadEntityConfig(XmlReader reader)
	{
		string rootNode = reader.ReadElementContentAsString();
		if (rootNode != "EntityConfig")
		{
			throw _invalidFormat;
		}

		return null!;
	}
}
