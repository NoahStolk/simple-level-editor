using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using SimpleLevelEditor.Formats.Level.XmlModel;
using SimpleLevelEditor.Formats.Types.Level;
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

		Level3dData level3dData = new(
			entityConfigPath: xmlEntityConfigData.EntityConfig,
			meshes: ListModule.OfSeq(xmlEntityConfigData.Meshes.ConvertAll(m => m.Path)),
			textures: ListModule.OfSeq(xmlEntityConfigData.Textures.ConvertAll(m => m.Path)),
			worldObjects: ListModule.OfSeq(xmlEntityConfigData.WorldObjects
				.Select((wo, i) =>
				{
					FSharpOption<WorldObject>? worldObject = WorldObject.FromData(i + 1, wo.Mesh, wo.Texture, wo.Scale, wo.Rotation, wo.Position, wo.Flags);
					if (worldObject == null)
						throw new InvalidOperationException("World object is not valid."); // TODO: Add more info.

					return worldObject.Value;
				})
				.ToList()),
			entities: ListModule.OfSeq(xmlEntityConfigData.Entities
				.Select((e, i) =>
				{
					FSharpOption<Entity>? entity = Entity.FromData(i + 1, e.Shape, e.Name, e.Position, MapModule.OfSeq(e.Properties.Select(p => Tuple.Create(p.Name, p.Value))));
					if (entity == null)
						throw new InvalidOperationException("Entity is not valid."); // TODO: Add more info.

					return entity.Value;
				})
				.ToList()));

		return level3dData;
	}
}
