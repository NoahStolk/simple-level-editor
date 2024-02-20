using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.Formats.Utils;
using System.Globalization;
using System.Text;
using System.Xml;

namespace SimpleLevelEditor.Formats.Level;

public static class XmlFormatSerializer
{
	private static readonly InvalidDataException _invalidFormat = new("Invalid format");
	private static readonly Encoding _encoding = new UTF8Encoding(false);
	private static readonly XmlWriterSettings _xmlWriterSettings = new() { Indent = true, Encoding = _encoding };
	private static readonly XmlWriterSettings _xmlWriterSettingsCompact = new() { Indent = false, Encoding = _encoding };

	#region Read level

	public static Level3dData ReadLevel(XmlReader reader)
	{
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
		int worldObjectIndex = 0;
		List<WorldObject> worldObjects = [];
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
		int entityIndex = 0;
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

					int indexOfSpace = reader.Value.IndexOf(' ', StringComparison.Ordinal);
					string type = reader.Value[..indexOfSpace];
					string value = reader.Value[(indexOfSpace + 1)..];

					properties.Add(new()
					{
						Key = reader.Name,
						Value = DataFormatter.ReadProperty(value, type),
					});
				}

				entityIndex++; // 0 is reserved for the default entity.
				Entity entity = new()
				{
					Id = entityIndex,
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

	#endregion Read level

	#region Write level

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
		writer.WriteAttributeString("Version", level.Version.ToString(CultureInfo.InvariantCulture));
		writer.WriteAttributeString("EntityConfig", level.EntityConfigPath ?? string.Empty);

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
			writer.WriteAttributeString("Position", DataFormatter.WriteProperty(worldObject.Position));
			writer.WriteAttributeString("Rotation", DataFormatter.WriteProperty(worldObject.Rotation));
			writer.WriteAttributeString("Scale", DataFormatter.WriteProperty(worldObject.Scale));
			writer.WriteAttributeString("Flags", string.Join(',', worldObject.Flags.Select(s => s.Trim())));
			writer.WriteEndElement();
		}

		writer.WriteEndElement();

		writer.WriteStartElement("Entities");
		foreach (Entity entity in level.Entities)
		{
			writer.WriteStartElement("Entity");
			writer.WriteAttributeString("Name", entity.Name.Trim());
			writer.WriteAttributeString("Position", DataFormatter.WriteProperty(entity.Position));
			writer.WriteAttributeString("Shape", DataFormatter.WriteShape(entity.Shape));

			foreach (EntityProperty property in entity.Properties)
			{
				if (property.Key.Length == 0 || !char.IsLetter(property.Key[0]) || property.Key is "Name" or "Position" or "Shape")
					continue;

				writer.WriteAttributeString(property.Key.Trim(), $"{DataFormatter.WritePropertyType(property.Value)} {DataFormatter.WriteProperty(property.Value)}");
			}

			writer.WriteEndElement();
		}

		writer.WriteEndElement();

		writer.WriteEndElement();
	}

	#endregion Write level
}
