using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using SimpleLevelEditor.Formats.EntityConfig.XmlModel;
using SimpleLevelEditor.Formats.Types.EntityConfig;
using System.Xml;
using System.Xml.Serialization;

namespace SimpleLevelEditor.Formats.EntityConfig;

public static class EntityConfigXmlDeserializer
{
	private static readonly XmlSerializer _serializer = new(typeof(XmlEntityConfigData));

	public static EntityConfigData ReadEntityConfig(Stream stream)
	{
		stream.Position = 0;

		using XmlReader reader = XmlReader.Create(stream);
		if (_serializer.Deserialize(reader) is not XmlEntityConfigData xmlEntityConfigData)
			throw new InvalidOperationException("XML is not valid.");

		if (xmlEntityConfigData.Version != 2)
			throw new InvalidOperationException($"Unsupported entity config version '{xmlEntityConfigData.Version}'.");

		EntityConfigData entityConfig = new(
			version: xmlEntityConfigData.Version,
			entities: ListModule.OfSeq(xmlEntityConfigData.Entities.ConvertAll(e =>
			{
				return new EntityDescriptor(
					name: e.Name,
					shape: EntityShape.FromShapeText(e.Shape),
					properties: ListModule.OfSeq(e.Properties.ConvertAll(p => new EntityPropertyDescriptor(
						p.Name,
						EntityPropertyTypeDescriptor.FromXmlData(p.Type, p.DefaultValue ?? FSharpOption<string>.None, p.Step ?? FSharpOption<string>.None, p.MinValue ?? FSharpOption<string>.None, p.MaxValue ?? FSharpOption<string>.None),
						p.Description))));
			})));

		return entityConfig;
	}
}
