using OneOf;
using SimpleLevelEditor.Formats.EntityConfig.Model;
using SimpleLevelEditor.Formats.EntityConfig.Model.PropertyTypes;
using SimpleLevelEditor.Formats.EntityConfig.XmlModel;
using SimpleLevelEditor.Formats.Utils;
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

		EntityConfigData entityConfig = new()
		{
			Version = xmlEntityConfigData.Version,
			Entities = xmlEntityConfigData.Entities.ConvertAll(e => new EntityDescriptor
			{
				Name = e.Name,
				Shape = e.Shape switch
				{
					XmlEntityConfigEntityShape.Point => EntityShape.Point,
					XmlEntityConfigEntityShape.Sphere => EntityShape.Sphere,
					XmlEntityConfigEntityShape.Aabb => EntityShape.Aabb,
					_ => throw new InvalidOperationException($"Unknown entity shape: {e.Shape}"),
				},
				Properties = e.Properties.ConvertAll(p => new EntityPropertyDescriptor
				{
					Name = p.Name,
					Type = CreatePropertyType(p),
					Description = p.Description,
				}),
			}),
		};

		return entityConfig;

		static OneOf<BoolPropertyType, IntPropertyType, FloatPropertyType, Vector2PropertyType, Vector3PropertyType, Vector4PropertyType, StringPropertyType, RgbPropertyType, RgbaPropertyType> CreatePropertyType(XmlEntityConfigProperty property)
		{
			return property.Type switch
			{
				XmlEntityConfigPropertyType.Bool => new BoolPropertyType
				{
					DefaultValue = ParseUtils.TryReadBool(property.DefaultValue, out bool value) && value,
				},
				XmlEntityConfigPropertyType.Int => new IntPropertyType
				{
					DefaultValue = ParseUtils.TryReadInt(property.DefaultValue, out int value) ? value : 0,
					Step = ParseUtils.TryReadInt(property.Step, out int step) ? step : 1,
					MinValue = ParseUtils.TryReadInt(property.MinValue, out int minValue) ? minValue : int.MinValue,
					MaxValue = ParseUtils.TryReadInt(property.MaxValue, out int maxValue) ? maxValue : int.MaxValue,
				},
				XmlEntityConfigPropertyType.Float => new FloatPropertyType
				{
					DefaultValue = ParseUtils.TryReadFloat(property.DefaultValue, out float value) ? value : 0f,
					Step = ParseUtils.TryReadFloat(property.Step, out float step) ? step : 1,
					MinValue = ParseUtils.TryReadFloat(property.MinValue, out float minValue) ? minValue : float.MinValue,
					MaxValue = ParseUtils.TryReadFloat(property.MaxValue, out float maxValue) ? maxValue : float.MaxValue,
				},
				XmlEntityConfigPropertyType.Vector2 => new Vector2PropertyType
				{
					DefaultValue = ParseUtils.TryReadVector2(property.DefaultValue, out Vector2 value) ? value : Vector2.Zero,
					Step = ParseUtils.TryReadFloat(property.Step, out float step) ? step : 1,
					MinValue = ParseUtils.TryReadFloat(property.MinValue, out float minValue) ? minValue : float.MinValue,
					MaxValue = ParseUtils.TryReadFloat(property.MaxValue, out float maxValue) ? maxValue : float.MaxValue,
				},
				XmlEntityConfigPropertyType.Vector3 => new Vector3PropertyType
				{
					DefaultValue = ParseUtils.TryReadVector3(property.DefaultValue, out Vector3 value) ? value : Vector3.Zero,
					Step = ParseUtils.TryReadFloat(property.Step, out float step) ? step : 1,
					MinValue = ParseUtils.TryReadFloat(property.MinValue, out float minValue) ? minValue : float.MinValue,
					MaxValue = ParseUtils.TryReadFloat(property.MaxValue, out float maxValue) ? maxValue : float.MaxValue,
				},
				XmlEntityConfigPropertyType.Vector4 => new Vector4PropertyType
				{
					DefaultValue = ParseUtils.TryReadVector4(property.DefaultValue, out Vector4 value) ? value : Vector4.Zero,
					Step = ParseUtils.TryReadFloat(property.Step, out float step) ? step : 1,
					MinValue = ParseUtils.TryReadFloat(property.MinValue, out float minValue) ? minValue : float.MinValue,
					MaxValue = ParseUtils.TryReadFloat(property.MaxValue, out float maxValue) ? maxValue : float.MaxValue,
				},
				XmlEntityConfigPropertyType.String => new StringPropertyType
				{
					DefaultValue = property.DefaultValue ?? string.Empty,
				},
				XmlEntityConfigPropertyType.Rgb => new RgbPropertyType
				{
					DefaultValue = ParseUtils.TryReadRgb(property.DefaultValue, out Rgb value) ? value : default,
				},
				XmlEntityConfigPropertyType.Rgba => new RgbaPropertyType
				{
					DefaultValue = ParseUtils.TryReadRgba(property.DefaultValue, out Rgba value) ? value : default,
				},
				_ => throw new InvalidOperationException($"Unknown property type: {property.Type}"),
			};
		}
	}
}
