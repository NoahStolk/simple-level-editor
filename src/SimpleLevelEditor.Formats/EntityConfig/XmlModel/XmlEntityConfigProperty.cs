using System.Xml.Serialization;

namespace SimpleLevelEditor.Formats.EntityConfig.XmlModel;

public record XmlEntityConfigProperty
{
	[XmlAttribute]
	public required string Name { get; init; }

	[XmlAttribute]
	public required XmlEntityConfigPropertyType Type { get; init; }

	[XmlAttribute]
	public string? Description { get; init; }

	[XmlAttribute]
	public string? DefaultValue { get; init; }

	[XmlAttribute]
	public string? Step { get; init; }

	[XmlAttribute]
	public string? MinValue { get; init; }

	[XmlAttribute]
	public string? MaxValue { get; init; }
}
