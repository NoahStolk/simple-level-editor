using System.Xml.Serialization;

namespace SimpleLevelEditor.Formats.Level.XmlModel;

public record XmlLevelEntity
{
	[XmlAttribute]
	public required string Name { get; init; }

	[XmlAttribute]
	public required string Position { get; init; }

	[XmlAttribute]
	public required string Shape { get; init; }

	[XmlArray]
	[XmlArrayItem("Property")]
	public required List<XmlLevelEntityProperty> Properties { get; init; }
}
