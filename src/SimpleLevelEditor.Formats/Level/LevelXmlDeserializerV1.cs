using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.Formats.Utils;
using System.Globalization;
using System.Xml;

namespace SimpleLevelEditor.Formats.Level;

[Obsolete("V1 is obsolete.")]
internal static class LevelXmlDeserializerV1
{
	private static readonly InvalidDataException _invalidFormat = new("Invalid format");

	public static Level3dData ReadLevel(Stream stream)
	{
		stream.Position = 0;

		using XmlReader reader = XmlReader.Create(stream);
		Level3dData level = Level3dData.CreateDefault();
		while (reader.Read())
		{
			if (reader is { NodeType: XmlNodeType.Element, IsEmptyElement: false })
			{
				switch (reader.Name)
				{
					case "Level":
						level.Version = int.Parse(reader.GetAttribute("Version") ?? throw _invalidFormat, CultureInfo.InvariantCulture);
						level.EntityConfigPath = reader.GetAttribute("EntityConfig");
						break;
					case "Meshes": level.Meshes = ReadMeshes(reader); break;
					case "Textures": level.Textures = ReadTextures(reader); break;
					case "WorldObjects": level.WorldObjects = ReadWorldObjects(reader); break;
					case "Entities": level.Entities = ReadEntities(reader); break;
				}
			}
		}

		return level;
	}

	private static List<string> ReadMeshes(XmlReader reader)
	{
		List<string> meshes = [];
		while (reader.Read())
		{
			if (reader is { NodeType: XmlNodeType.Element, Name: "Mesh" })
			{
				meshes.Add(reader.GetAttribute("Path") ?? throw _invalidFormat);
			}
			else if (reader is { NodeType: XmlNodeType.EndElement, Name: "Meshes" })
			{
				break;
			}
		}

		return meshes;
	}

	private static List<string> ReadTextures(XmlReader reader)
	{
		List<string> textures = [];
		while (reader.Read())
		{
			if (reader is { NodeType: XmlNodeType.Element, Name: "Texture" })
			{
				textures.Add(reader.GetAttribute("Path") ?? throw _invalidFormat);
			}
			else if (reader is { NodeType: XmlNodeType.EndElement, Name: "Textures" })
			{
				break;
			}
		}

		return textures;
	}

	private static List<WorldObject> ReadWorldObjects(XmlReader reader)
	{
		List<WorldObject> worldObjects = [];
		while (reader.Read())
		{
			if (reader is { NodeType: XmlNodeType.Element, Name: "WorldObject" })
			{
				WorldObject worldObject = new()
				{
					Mesh = reader.GetAttribute("Mesh") ?? throw _invalidFormat,
					Texture = reader.GetAttribute("Texture") ?? throw _invalidFormat,
					Position = ParseUtils.ReadVector3(reader.GetAttribute("Position") ?? throw _invalidFormat),
					Rotation = ParseUtils.ReadVector3(reader.GetAttribute("Rotation") ?? throw _invalidFormat),
					Scale = ParseUtils.ReadVector3(reader.GetAttribute("Scale") ?? throw _invalidFormat),
					Flags = reader.GetAttribute("Flags")?.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).ToList() ?? [],
				};
				worldObjects.Add(worldObject);
			}
			else if (reader is { NodeType: XmlNodeType.EndElement, Name: "WorldObjects" })
			{
				break;
			}
		}

		return worldObjects;
	}

	private static List<Entity> ReadEntities(XmlReader reader)
	{
		List<Entity> entities = [];
		while (reader.Read())
		{
			if (reader is { NodeType: XmlNodeType.Element, Name: "Entity" })
			{
				List<EntityProperty> properties = [];
				for (int i = 0; i < reader.AttributeCount; i++)
				{
					reader.MoveToAttribute(i);
					if (reader.Name is "Name" or "Position" or "Shape")
						continue;

					properties.Add(new()
					{
						Key = reader.Name,
						Value = DataFormatter.ReadProperty(reader.Value),
					});
				}

				Entity entity = new()
				{
					Name = reader.GetAttribute("Name") ?? throw _invalidFormat,
					Position = ParseUtils.ReadVector3(reader.GetAttribute("Position") ?? throw _invalidFormat),
					Shape = DataFormatter.ReadShape(reader.GetAttribute("Shape") ?? throw _invalidFormat),
					Properties = properties,
				};
				entities.Add(entity);
			}
			else if (reader is { NodeType: XmlNodeType.EndElement, Name: "Entities" })
			{
				break;
			}
		}

		return entities;
	}
}
