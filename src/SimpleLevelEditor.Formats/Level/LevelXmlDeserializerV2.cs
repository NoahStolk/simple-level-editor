using Microsoft.FSharp.Core;
using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.Formats.Level.XmlModel;
using SimpleLevelEditor.Formats.Types.Level;
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
			EntityConfigPath = xmlEntityConfigData.EntityConfig,
			Meshes = xmlEntityConfigData.Meshes.ConvertAll(m => m.Path),
			Textures = xmlEntityConfigData.Textures.ConvertAll(m => m.Path),
			WorldObjects = xmlEntityConfigData.WorldObjects
				.Select((wo, i) => new WorldObject
				{
					Id = i + 1,
					Mesh = wo.Mesh,
					Texture = wo.Texture,
					Position = ParseUtils.TryReadVector3(wo.Position, out Vector3 position) ? position : Vector3.Zero,
					Rotation = ParseUtils.TryReadVector3(wo.Rotation, out Vector3 rotation) ? rotation : Vector3.Zero,
					Scale = ParseUtils.TryReadVector3(wo.Scale, out Vector3 scale) ? scale : Vector3.One,
					Flags = wo.Flags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList(),
				})
				.ToList(),
			Entities = xmlEntityConfigData.Entities
				.Select((e, i) =>
				{
					FSharpOption<ShapeDescriptor>? shape = ShapeDescriptor.FromShapeData(e.Shape);
					if (shape == null)
						throw new InvalidOperationException($"Shape {e.Shape} is not valid.");

					return new Entity
					{
						Id = i + 1,
						Shape = shape.Value,
						Name = e.Name,
						Position = ParseUtils.TryReadVector3(e.Position, out Vector3 position) ? position : Vector3.Zero,
						Properties = e.Properties.ConvertAll(p =>
						{
							FSharpOption<EntityPropertyValue>? value = EntityPropertyValue.FromTypeData(p.Value);
							if (value == null)
								throw new InvalidOperationException($"Property {p.Value} value is not valid.");

							return new EntityProperty
							{
								Key = p.Name,
								Value = value.Value,
							};
						}),
					};
				})
				.ToList(),
		};

		return level3dData;
	}
}
