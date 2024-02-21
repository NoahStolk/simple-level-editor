using System.Xml.Serialization;

namespace SimpleLevelEditor.Formats.Level.XmlModel;

public record XmlLevelMesh
{
	[XmlAttribute]
	public required string Path { get; init; }
}
