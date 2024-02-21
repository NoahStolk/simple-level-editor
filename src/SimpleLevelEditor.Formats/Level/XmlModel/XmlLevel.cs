using System.Xml.Serialization;

namespace SimpleLevelEditor.Formats.Level.XmlModel;

[XmlRoot("Level")]
public record XmlLevel
{
	[XmlAttribute]
	public int Version { get; init; }

	[XmlAttribute]
	public string? EntityConfig { get; init; }

	[XmlArray]
	[XmlArrayItem("Mesh")]
	public required List<XmlLevelMesh> Meshes { get; init; }

	[XmlArray]
	[XmlArrayItem("Texture")]
	public required List<XmlLevelTexture> Textures { get; init; }

	[XmlArray]
	[XmlArrayItem("WorldObject")]
	public required List<XmlLevelWorldObject> WorldObjects { get; init; }

	[XmlArray]
	[XmlArrayItem("Entity")]
	public required List<XmlLevelEntity> Entities { get; init; }
}
