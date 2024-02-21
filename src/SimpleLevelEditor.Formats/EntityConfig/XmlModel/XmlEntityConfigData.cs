using System.Xml.Serialization;

namespace SimpleLevelEditor.Formats.EntityConfig.XmlModel;

[XmlRoot("EntityConfigData")]
public record XmlEntityConfigData
{
	[XmlAttribute]
	public required int Version { get; init; }

	[XmlElement("Entity")]
	public required List<XmlEntityConfigEntity> Entities { get; init; }
}
