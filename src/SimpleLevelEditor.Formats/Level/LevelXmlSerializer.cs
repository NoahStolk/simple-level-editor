using SimpleLevelEditor.Formats.Level.Model;
using System.Globalization;
using System.Text;
using System.Xml;

namespace SimpleLevelEditor.Formats.Level;

public static class LevelXmlSerializer
{
	private static readonly Encoding _encoding = new UTF8Encoding(false);
	private static readonly XmlWriterSettings _xmlWriterSettings = new() { Indent = true, IndentChars = "\t", Encoding = _encoding };

	// TODO: Remove this when custom binary serialization is implemented.
	private static readonly XmlWriterSettings _xmlWriterSettingsCompact = new() { Indent = false, Encoding = _encoding };

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
}
