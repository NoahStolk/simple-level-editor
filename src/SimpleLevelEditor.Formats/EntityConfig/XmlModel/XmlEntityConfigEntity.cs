using System.Xml.Serialization;

namespace SimpleLevelEditor.Formats.EntityConfig.XmlModel;

public record XmlEntityConfigEntity
{
	[XmlAttribute]
	public required string Name { get; init; }

	[XmlAttribute]
	public required XmlEntityConfigEntityShape Shape { get; init; }

	[XmlElement("Property")]
	public required List<XmlEntityConfigProperty> Properties { get; init; }
}
