using SimpleLevelEditor.Formats.Level.XmlModel;
using SimpleLevelEditor.Formats.Types.Level;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SimpleLevelEditor.Formats.Level;

public static class LevelXmlSerializer
{
	private const int _version = 2;

	private static readonly XmlSerializer _serializer = new(typeof(XmlLevel));
	private static readonly XmlWriterSettings _xmlWriterSettings = new()
	{
		Encoding = new UTF8Encoding(false),
		Indent = true,
		IndentChars = "\t",
	};

	public static void WriteLevel(MemoryStream ms, Level3dData level)
	{
		XmlSerializerNamespaces ns = new();
		ns.Add(string.Empty, string.Empty);

		XmlLevel xmlLevel = new()
		{
			Version = _version,
			EntityConfig = level.EntityConfigPath?.Value,
			Meshes = level.Meshes.Select(m => new XmlLevelMesh { Path = m }).ToList(),
			Textures = level.Textures.Select(m => new XmlLevelTexture { Path = m }).ToList(),
			WorldObjects = level.WorldObjects
				.Select(wo => new XmlLevelWorldObject
				{
					Mesh = wo.Mesh,
					Texture = wo.Texture,
					Position = EntityPropertyValue.NewVector3(wo.Position).WriteValue(),
					Rotation = EntityPropertyValue.NewVector3(wo.Rotation).WriteValue(),
					Scale = EntityPropertyValue.NewVector3(wo.Scale).WriteValue(),
					Flags = string.Join(',', wo.Flags),
				})
				.ToList(),
			Entities = level.Entities
				.Select(e => new XmlLevelEntity
				{
					Name = e.Name,
					Position = EntityPropertyValue.NewVector3(e.Position).WriteValue(),
					Shape = $"{e.Shape.GetShapeId()} {e.Shape.WriteValue()}".TrimEnd(),
					Properties = e.Properties
						.Select(p => new XmlLevelEntityProperty
						{
							Name = p.Key,
							Value = $"{p.Value.GetTypeId()} {p.Value.WriteValue()}",
						})
						.ToList(),
				})
				.ToList(),
		};

		using XmlWriter xmlWriter = XmlWriter.Create(ms, _xmlWriterSettings);
		_serializer.Serialize(xmlWriter, xmlLevel, ns);
		xmlWriter.Flush();
		ms.Write("\n"u8);
	}
}
