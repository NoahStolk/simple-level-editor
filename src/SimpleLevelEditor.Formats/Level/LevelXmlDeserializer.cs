using SimpleLevelEditor.Formats.Level.Model;
using System.Globalization;
using System.Xml;

namespace SimpleLevelEditor.Formats.Level;

public static class LevelXmlDeserializer
{
	public static Level3dData ReadLevel(Stream stream)
	{
		stream.Position = 0;

		int? version = null;
		using XmlReader reader = XmlReader.Create(stream);
		while (reader.Read() && !version.HasValue)
		{
			if (reader is { NodeType: XmlNodeType.Element, IsEmptyElement: false, Name: "Level" } && int.TryParse(reader.GetAttribute("Version"), CultureInfo.InvariantCulture, out int parsedVersion))
				version = parsedVersion;
		}

		return version switch
		{
			1 => LevelXmlDeserializerV1.ReadLevel(stream),
			2 => LevelXmlDeserializerV2.ReadLevel(stream),
			_ => throw new NotSupportedException($"Version {version} is not supported."),
		};
	}
}
