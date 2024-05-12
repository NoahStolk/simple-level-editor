using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.Formats.Level.XmlModel;
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
			EntityConfig = level.EntityConfigPath,
			Meshes = level.Meshes.ConvertAll(m => new XmlLevelMesh { Path = m }),
			Textures = level.Textures.ConvertAll(m => new XmlLevelTexture { Path = m }),
			WorldObjects = level.WorldObjects.ConvertAll(wo => new XmlLevelWorldObject
			{
				Mesh = wo.Mesh,
				Texture = wo.Texture,
				Position = Types.Level.EntityPropertyValue.NewVector3(wo.Position).WriteValue(),
				Rotation = Types.Level.EntityPropertyValue.NewVector3(wo.Rotation).WriteValue(),
				Scale = Types.Level.EntityPropertyValue.NewVector3(wo.Scale).WriteValue(),
				Flags = string.Join(',', wo.Flags),
			}),
			Entities = level.Entities.ConvertAll(e => new XmlLevelEntity
			{
				Name = e.Name,
				Position = Types.Level.EntityPropertyValue.NewVector3(e.Position).WriteValue(),
				Shape = $"{e.Shape.GetShapeId()} {e.Shape.WriteValue()}".TrimEnd(),
				Properties = e.Properties
					.Select(p => new XmlLevelEntityProperty
					{
						Name = p.Key,
						Value = $"{p.Value.GetTypeId()} {p.Value.WriteValue()}",
					})
					.ToList(),
			}),
		};

		using XmlWriter xmlWriter = XmlWriter.Create(ms, _xmlWriterSettings);
		_serializer.Serialize(xmlWriter, xmlLevel, ns);
		xmlWriter.Flush();
		ms.Write("\n"u8);
	}
}
