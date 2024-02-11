using System.Xml.Serialization;

namespace SimpleLevelEditor.Model.EntityConfig;

public record EntityPropertyDescriptor
{
	[XmlAttribute]
	public required string Name;

	[XmlAttribute]
	public required EntityPropertyType Type;
}
