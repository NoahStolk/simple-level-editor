using OneOf;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Model.EntityTypes;
using SimpleLevelEditor.Model.Enums;
using SimpleLevelEditor.Utils;
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
		Level3dData level = Level3dData.Default;
		while (reader.Read())
		{
			if (reader.NodeType == XmlNodeType.Element)
			{
				switch (reader.Name)
				{
					case "Level":
						level.Version = int.Parse(reader.GetAttribute("Version") ?? throw _invalidFormat);
						break;
					case "Meshes":
						level.Meshes = ReadMeshes(reader);
						break;
					case "Textures":
						level.Textures = ReadTextures(reader);
						break;
					case "WorldObjects":
						level.WorldObjects = ReadWorldObjects(reader);
						break;
					case "Entities":
						level.Entities = ReadEntities(reader);
						break;
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
					BoundingMesh = reader.GetAttribute("BoundingMesh") ?? throw _invalidFormat,
					Position = ReadVector3(reader.GetAttribute("Position") ?? throw _invalidFormat),
					Rotation = MathUtils.ToRadians(ReadVector3(reader.GetAttribute("Rotation") ?? throw _invalidFormat)),
					Scale = ReadVector3(reader.GetAttribute("Scale") ?? throw _invalidFormat),
					Values = Enum.Parse<WorldObjectValues>(reader.GetAttribute("Values") ?? throw _invalidFormat),
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
		List<Entity> entities = new();
		while (reader.Read())
		{
			if (reader is { NodeType: XmlNodeType.Element, Name: "Entity" })
			{
				Entity entity = new()
				{
					Name = reader.GetAttribute("Name") ?? throw _invalidFormat,
					Shape = ReadShape(reader.GetAttribute("Shape") ?? throw _invalidFormat),
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

	private static OneOf<Point, Sphere, Aabb, StandingCylinder> ReadShape(string shape)
	{
		string[] parts = shape.Split(' ');
		switch (parts[0])
		{
			case "Point":
				return new Point(ReadVector3(parts[1]));
			case "Sphere":
				return new Sphere(ReadVector3(parts[1]), float.Parse(parts[2]));
			case "Aabb":
				return new Aabb(ReadVector3(parts[1]), ReadVector3(parts[2]));
			case "StandingCylinder":
				return new StandingCylinder(ReadVector3(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
			default:
				throw new Exception($"Unknown shape: {shape}");
		}
	}

	private static List<EntityProperty> ReadProperties(XmlReader reader)
	{
		List<EntityProperty> properties = new();
		while (reader.Read())
		{
			if (reader is { NodeType: XmlNodeType.Element, Name: "Property" })
			{
				EntityProperty property = new()
				{
					Key = reader.GetAttribute("Key") ?? throw _invalidFormat,
					Value = int.Parse(reader.GetAttribute("Value") ?? throw _invalidFormat),
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

	private static Vector3 ReadVector3(string vector)
	{
		string[] parts = vector.Split(' ');
		return new(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
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
			writer.WriteAttributeString("BoundingMesh", worldObject.BoundingMesh);
			writer.WriteVector3("Position", worldObject.Position);
			writer.WriteVector3("Rotation", MathUtils.ToDegrees(worldObject.Rotation));
			writer.WriteVector3("Scale", worldObject.Scale);
			writer.WriteAttributeString("Values", worldObject.Values.ToString());
			writer.WriteEndElement();
		}

		writer.WriteEndElement();

		writer.WriteStartElement("Entities");
		foreach (Entity entity in level.Entities)
		{
			writer.WriteStartElement("Entity");
			writer.WriteAttributeString("Name", entity.Name);
			writer.WriteAttributeString("Shape", entity.Shape.ToString());
			foreach (EntityProperty property in entity.Properties)
			{
				writer.WriteStartElement("Property");
				writer.WriteAttributeString("Key", property.Key);
				writer.WriteAttributeString("Value", property.Value.ToString());
				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}

		writer.WriteEndElement();

		writer.WriteEndElement();
	}

	private static void WriteVector3(this XmlWriter writer, string name, Vector3 vector)
	{
		writer.WriteAttributeString(name, $"{vector.X} {vector.Y} {vector.Z}");
	}
}
