using SimpleLevelEditor.Model;
using System.Text;
using System.Xml;

namespace SimpleLevelEditor.Formats;

public static class XmlFormatSerializer
{
	private static readonly Exception _invalidFormat = new("Invalid format");
	private static readonly XmlWriterSettings _xmlWriterSettings = new() { Indent = true, Encoding = new UTF8Encoding(false) };
	private static readonly XmlWriterSettings _xmlWriterSettingsCompact = new() { Indent = false, Encoding = new UTF8Encoding(false) };

	public static Level3dData ReadLevel(XmlReader reader)
	{
		Level3dData level = Level3dData.CreateDefault();
		while (reader.Read())
		{
			if (reader is { NodeType: XmlNodeType.Element, IsEmptyElement: false })
			{
				switch (reader.Name)
				{
					case "Level": level.Version = int.Parse(reader.GetAttribute("Version") ?? throw _invalidFormat); break;
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
		List<string> meshes = new();
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
		List<string> textures = new();
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
		int worldObjectIndex = 0;
		List<WorldObject> worldObjects = new();
		while (reader.Read())
		{
			if (reader is { NodeType: XmlNodeType.Element, Name: "WorldObject" })
			{
				worldObjectIndex++; // 0 is reserved for the default object.
				WorldObject worldObject = new()
				{
					Id = worldObjectIndex,
					Mesh = reader.GetAttribute("Mesh") ?? throw _invalidFormat,
					Texture = reader.GetAttribute("Texture") ?? throw _invalidFormat,
					Position = DataFormatter.ReadVector3(reader.GetAttribute("Position") ?? throw _invalidFormat),
					Rotation = DataFormatter.ReadVector3(reader.GetAttribute("Rotation") ?? throw _invalidFormat),
					Scale = DataFormatter.ReadVector3(reader.GetAttribute("Scale") ?? throw _invalidFormat),
					Flags = reader.GetAttribute("Flags")?.Split(',').Select(s => s.Trim()).ToList() ?? new List<string>(),
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
		int entityIndex = 0;
		List<Entity> entities = new();
		while (reader.Read())
		{
			if (reader is { NodeType: XmlNodeType.Element, Name: "Entity" })
			{
				entityIndex++; // 0 is reserved for the default entity.
				Entity entity = new()
				{
					Id = entityIndex,
					Name = reader.GetAttribute("Name") ?? throw _invalidFormat,
					Position = DataFormatter.ReadVector3(reader.GetAttribute("Position") ?? throw _invalidFormat),
					Shape = DataFormatter.ReadShape(reader.GetAttribute("Shape") ?? throw _invalidFormat),
					Properties = ReadProperties(reader),
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

	private static List<EntityProperty> ReadProperties(XmlReader reader)
	{
		List<EntityProperty> properties = new();
		while (reader.Read())
		{
			if (reader is { NodeType: XmlNodeType.Element, Name: "Property" })
			{
				string type = reader.GetAttribute("Type") ?? throw _invalidFormat;
				string valueString = reader.GetAttribute("Value") ?? throw _invalidFormat;
				EntityProperty property = new()
				{
					Key = reader.GetAttribute("Key") ?? throw _invalidFormat,
					Value = DataFormatter.ReadProperty(valueString, type),
				};
				properties.Add(property);
			}
			else if (reader is { NodeType: XmlNodeType.EndElement, Name: "Entity" })
			{
				break;
			}
		}

		return properties;
	}

	public static void WriteLevel(MemoryStream ms, Level3dData level, bool writeCompact)
	{
		using XmlWriter writer = XmlWriter.Create(ms, writeCompact ? _xmlWriterSettingsCompact : _xmlWriterSettings);
		WriteLevel(level, writer);
		writer.Flush();
		ms.Write("\n"u8);
	}

	private static void WriteLevel(Level3dData level, XmlWriter writer)
	{
		writer.WriteStartElement("Level");
		writer.WriteAttributeString("Version", level.Version.ToString());

		writer.WriteStartElement("Meshes");
		foreach (string mesh in level.Meshes)
		{
			writer.WriteStartElement("Mesh");
			writer.WriteAttributeString("Path", mesh);
			writer.WriteEndElement();
		}

		writer.WriteEndElement();

		writer.WriteStartElement("Textures");
		foreach (string texture in level.Textures)
		{
			writer.WriteStartElement("Texture");
			writer.WriteAttributeString("Path", texture);
			writer.WriteEndElement();
		}

		writer.WriteEndElement();

		writer.WriteStartElement("WorldObjects");
		foreach (WorldObject worldObject in level.WorldObjects)
		{
			writer.WriteStartElement("WorldObject");
			writer.WriteAttributeString("Mesh", worldObject.Mesh);
			writer.WriteAttributeString("Texture", worldObject.Texture);
			writer.WriteAttributeString("Position", DataFormatter.Write(worldObject.Position));
			writer.WriteAttributeString("Rotation", DataFormatter.Write(worldObject.Rotation));
			writer.WriteAttributeString("Scale", DataFormatter.Write(worldObject.Scale));
			writer.WriteAttributeString("Flags", string.Join(", ", worldObject.Flags.Select(s => s.Trim())));
			writer.WriteEndElement();
		}

		writer.WriteEndElement();

		writer.WriteStartElement("Entities");
		foreach (Entity entity in level.Entities)
		{
			writer.WriteStartElement("Entity");
			writer.WriteAttributeString("Name", entity.Name.Trim());
			writer.WriteAttributeString("Position", DataFormatter.Write(entity.Position));
			writer.WriteAttributeString("Shape", DataFormatter.Write(entity.Shape));

			writer.WriteStartElement("Properties");
			foreach (EntityProperty property in entity.Properties)
			{
				writer.WriteStartElement("Property");
				writer.WriteAttributeString("Key", property.Key.Trim());
				writer.WriteAttributeString("Type", DataFormatter.WritePropertyType(property.Value));
				writer.WriteAttributeString("Value", DataFormatter.Write(property.Value));
				writer.WriteEndElement();
			}

			writer.WriteEndElement();

			writer.WriteEndElement();
		}

		writer.WriteEndElement();

		writer.WriteEndElement();
	}
}
