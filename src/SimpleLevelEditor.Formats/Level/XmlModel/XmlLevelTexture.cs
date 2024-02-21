using System.Xml.Serialization;

namespace SimpleLevelEditor.Formats.Level.XmlModel;

public record XmlLevelTexture
{
	[XmlAttribute]
	public required string Path { get; init; }
}
