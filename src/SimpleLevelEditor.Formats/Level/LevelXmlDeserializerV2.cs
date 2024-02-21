using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.Formats.Level.XmlModel;
using SimpleLevelEditor.Formats.Utils;
using System.Xml;
using System.Xml.Serialization;

namespace SimpleLevelEditor.Formats.Level;

internal static class LevelXmlDeserializerV2
{
	private static readonly XmlSerializer _serializer = new(typeof(XmlLevel));

	public static Level3dData ReadLevel(Stream stream)
	{
		stream.Position = 0;

		using XmlReader reader = XmlReader.Create(stream);
		if (_serializer.Deserialize(reader) is not XmlLevel xmlEntityConfigData)
			throw new InvalidOperationException("XML is not valid.");

		Level3dData level3dData = new()
		{
			Version = xmlEntityConfigData.Version,
			EntityConfigPath = xmlEntityConfigData.EntityConfig,
			Meshes = xmlEntityConfigData.Meshes.ConvertAll(m => m.Path),
			Textures = xmlEntityConfigData.Textures.ConvertAll(m => m.Path),
			WorldObjects = xmlEntityConfigData.WorldObjects.ConvertAll(wo => new WorldObject
			{
				Mesh = wo.Mesh,
				Texture = wo.Texture,
				Position = ParseUtils.TryReadVector3(wo.Position, out Vector3 position) ? position : Vector3.Zero,
				Rotation = ParseUtils.TryReadVector3(wo.Rotation, out Vector3 rotation) ? rotation : Vector3.Zero,
				Scale = ParseUtils.TryReadVector3(wo.Scale, out Vector3 scale) ? scale : Vector3.One,
				Flags = wo.Flags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
			}),
			Entities = xmlEntityConfigData.Entities.ConvertAll(e => new Entity
			{
				Shape = DataFormatter.ReadShape(e.Shape),
				Name = e.Name,
				Position = ParseUtils.TryReadVector3(e.Position, out Vector3 position) ? position : Vector3.Zero,
				Properties = e.Properties.ConvertAll(p => new EntityProperty
				{
					Key = p.Name,
					Value = DataFormatter.ReadProperty(p.Value),
				}),
			}),
		};

		return level3dData;
	}
}
