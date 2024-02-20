using System.Xml.Serialization;

namespace SimpleLevelEditor.Formats.Model.EntityConfig;

public record EntityDescriptor
{
	[XmlAttribute]
	public required string Name;

	[XmlAttribute]
	public required EntityShape Shape;

	[XmlElement("Property")]
	public required List<EntityPropertyDescriptor> Properties;
}
