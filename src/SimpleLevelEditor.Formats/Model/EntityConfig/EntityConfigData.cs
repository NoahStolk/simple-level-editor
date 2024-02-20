using System.Xml.Serialization;

namespace SimpleLevelEditor.Formats.Model.EntityConfig;

public record EntityConfigData
{
	[XmlAttribute]
	public required int Version;

	[XmlElement("Entity")]
	public required List<EntityDescriptor> Entities;

	public static EntityConfigData CreateDefault()
	{
		return new()
		{
			Version = 1,
			Entities = [],
		};
	}
}
