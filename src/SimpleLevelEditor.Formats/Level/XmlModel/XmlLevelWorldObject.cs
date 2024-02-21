using System.Xml.Serialization;

namespace SimpleLevelEditor.Formats.Level.XmlModel;

public record XmlLevelWorldObject
{
	[XmlAttribute]
	public required string Mesh { get; init; }

	[XmlAttribute]
	public required string Texture { get; init; }

	[XmlAttribute]
	public required string Position { get; init; }

	[XmlAttribute]
	public required string Rotation { get; init; }

	[XmlAttribute]
	public required string Scale { get; init; }

	[XmlAttribute]
	public required string Flags { get; init; }
}
