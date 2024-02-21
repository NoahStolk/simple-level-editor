using System.Xml.Serialization;

namespace SimpleLevelEditor.Formats.Level.XmlModel;

public record XmlLevelEntityProperty
{
	[XmlAttribute]
	public required string Name { get; init; }

	[XmlAttribute]
	public required string Value { get; init; }
}
