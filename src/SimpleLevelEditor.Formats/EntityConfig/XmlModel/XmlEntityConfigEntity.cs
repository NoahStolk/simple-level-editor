using SimpleLevelEditor.Formats.EntityConfig.Model;
using System.Xml.Serialization;

namespace SimpleLevelEditor.Formats.EntityConfig.XmlModel;

public record XmlEntityConfigEntity
{
	[XmlAttribute]
	public required string Name { get; init; }

	[XmlAttribute]
	public required EntityShape Shape { get; init; }

	[XmlElement("Property")]
	public required List<XmlEntityConfigProperty> Properties { get; init; }
}
