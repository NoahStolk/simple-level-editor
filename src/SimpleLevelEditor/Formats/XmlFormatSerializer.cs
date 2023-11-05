using OneOf;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Model.EntityShapes;
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
					Position = ReadVector3(reader.GetAttribute("Position") ?? throw _invalidFormat),
					Rotation = ReadVector3(reader.GetAttribute("Rotation") ?? throw _invalidFormat),
					Scale = ReadVector3(reader.GetAttribute("Scale") ?? throw _invalidFormat),
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
					Position = ReadVector3(reader.GetAttribute("Position") ?? throw _invalidFormat),
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

	private static IEntityShape ReadShape(string shape)
	{
		string[] parts = shape.Split(' ');
		return parts[0] switch
		{
			"point" => new Point(),
			"sphere" => new Sphere(float.Parse(parts[1])),
			"aabb" => new Aabb(new(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3])), new(float.Parse(parts[4]), float.Parse(parts[5]), float.Parse(parts[6]))),
			_ => throw _invalidFormat,
		};
	}

	private static List<EntityProperty> ReadProperties(XmlReader reader)
	{
		List<EntityProperty> properties = new();
		while (reader.Read())
		{
			if (reader is { NodeType: XmlNodeType.Element, Name: "Property" })
			{
				string type = reader.GetAttribute("Type") ?? throw _invalidFormat;
				OneOf<bool, int, float, Vector2, Vector3, Vector4, string> value = type switch
				{
					"bool" => bool.Parse(reader.GetAttribute("Value")?.ToLower() ?? throw _invalidFormat),
					"s32" => int.Parse(reader.GetAttribute("Value") ?? throw _invalidFormat),
					"float" => float.Parse(reader.GetAttribute("Value") ?? throw _invalidFormat),
					"float2" => ReadVector2(reader.GetAttribute("Value") ?? throw _invalidFormat),
					"float3" => ReadVector3(reader.GetAttribute("Value") ?? throw _invalidFormat),
					"float4" => ReadVector4(reader.GetAttribute("Value") ?? throw _invalidFormat),
					"str" => reader.GetAttribute("Value") ?? throw _invalidFormat,
					_ => throw _invalidFormat,
				};

				EntityProperty property = new()
				{
					Key = reader.GetAttribute("Key") ?? throw _invalidFormat,
					Value = value,
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

	private static Vector2 ReadVector2(string vector)
	{
		string[] parts = vector.Split(' ');
		return new(float.Parse(parts[0]), float.Parse(parts[1]));
	}

	private static Vector3 ReadVector3(string vector)
	{
		string[] parts = vector.Split(' ');
		return new(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
	}

	private static Vector4 ReadVector4(string vector)
	{
		string[] parts = vector.Split(' ');
		return new(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
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
			writer.WriteVector3("Position", worldObject.Position);
			writer.WriteVector3("Rotation", worldObject.Rotation);
			writer.WriteVector3("Scale", worldObject.Scale);
			writer.WriteAttributeString("Flags", string.Join(", ", worldObject.Flags.Select(s => s.Trim())));
			writer.WriteEndElement();
		}

		writer.WriteEndElement();

		writer.WriteStartElement("Entities");
		foreach (Entity entity in level.Entities)
		{
			writer.WriteStartElement("Entity");
			writer.WriteAttributeString("Name", entity.Name.Trim());
			writer.WriteVector3("Position", entity.Position);

			switch (entity.Shape)
			{
				case Aabb aabb: writer.WriteAttributeString("Shape", $"aabb {aabb.Min.X} {aabb.Min.Y} {aabb.Min.Z} {aabb.Max.X} {aabb.Max.Y} {aabb.Max.Z}"); break;
				case Sphere sphere: writer.WriteAttributeString("Shape", $"sphere {sphere.Radius}"); break;
				case Point: writer.WriteAttributeString("Shape", "point"); break;
			}

			writer.WriteStartElement("Properties");
			foreach (EntityProperty property in entity.Properties)
			{
				writer.WriteStartElement("Property");
				writer.WriteAttributeString("Key", property.Key.Trim());
				writer.WritePropertyType("Type", property.Value);
				writer.WritePropertyValue("Value", property.Value);
				writer.WriteEndElement();
			}

			writer.WriteEndElement();

			writer.WriteEndElement();
		}

		writer.WriteEndElement();

		writer.WriteEndElement();
	}

	private static void WriteVector3(this XmlWriter writer, string name, Vector3 vector)
	{
		writer.WriteAttributeString(name, $"{vector.X} {vector.Y} {vector.Z}");
	}

	private static void WritePropertyType(this XmlWriter writer, string name, OneOf<bool, int, float, Vector2, Vector3, Vector4, string> value)
	{
		string valueString = value.Value switch
		{
			bool => "bool",
			int => "s32",
			float => "float",
			Vector2 => "float2",
			Vector3 => "float3",
			Vector4 => "float4",
			string => "str",
			_ => throw new NotImplementedException(),
		};
		writer.WriteAttributeString(name, valueString);
	}

	private static void WritePropertyValue(this XmlWriter writer, string name, OneOf<bool, int, float, Vector2, Vector3, Vector4, string> value)
	{
		string valueString = value.Value switch
		{
			bool b => b.ToString().ToLower(),
			int i => i.ToString(),
			float f => f.ToString(),
			Vector2 v => $"{v.X} {v.Y}",
			Vector3 v => $"{v.X} {v.Y} {v.Z}",
			Vector4 v => $"{v.X} {v.Y} {v.Z} {v.W}",
			string s => s,
			_ => throw new NotImplementedException(),
		};
		writer.WriteAttributeString(name, valueString);
	}
}
