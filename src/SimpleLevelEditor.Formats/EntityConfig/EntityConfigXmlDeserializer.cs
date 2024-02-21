using OneOf;
using SimpleLevelEditor.Formats.EntityConfig.Model;
using SimpleLevelEditor.Formats.EntityConfig.Model.PropertyTypes;
using SimpleLevelEditor.Formats.Utils;
using System.Xml;
using System.Xml.Serialization;

namespace SimpleLevelEditor.Formats.EntityConfig;

public static class EntityConfigXmlDeserializer
{
	public static EntityConfigData ReadEntityConfig(XmlReader reader)
	{
		XmlSerializer serializer = new(typeof(XmlEntityConfigData));
		if (serializer.Deserialize(reader) is not XmlEntityConfigData xmlEntityConfigData)
			throw new InvalidOperationException("XML is not valid.");

		EntityConfigData entityConfig = new()
		{
			Version = xmlEntityConfigData.Version,
			Entities = xmlEntityConfigData.Entities.ConvertAll(e => new EntityDescriptor
			{
				Name = e.Name,
				Shape = e.Shape,
				Properties = e.Properties.ConvertAll(p => new EntityPropertyDescriptor
				{
					Name = p.Name,
					Type = CreatePropertyType(p),
					Description = p.Description,
				}),
			}),
		};

		return entityConfig;

		static OneOf<BoolPropertyType, IntPropertyType, FloatPropertyType, Vector2PropertyType, Vector3PropertyType, Vector4PropertyType, StringPropertyType, RgbPropertyType, RgbaPropertyType> CreatePropertyType(XmlProperty property)
		{
			return property.Type switch
			{
				XmlPropertyType.Bool => new BoolPropertyType
				{
					DefaultValue = ParseUtils.TryReadBool(property.DefaultValue, out bool value) && value,
				},
				XmlPropertyType.Int => new IntPropertyType
				{
					DefaultValue = ParseUtils.TryReadInt(property.DefaultValue, out int value) ? value : 0,
					Step = ParseUtils.TryReadInt(property.Step, out int step) ? step : 1,
					MinValue = ParseUtils.TryReadInt(property.MinValue, out int minValue) ? minValue : int.MinValue,
					MaxValue = ParseUtils.TryReadInt(property.MaxValue, out int maxValue) ? maxValue : int.MaxValue,
				},
				XmlPropertyType.Float => new FloatPropertyType
				{
					DefaultValue = ParseUtils.TryReadFloat(property.DefaultValue, out float value) ? value : 0f,
					Step = ParseUtils.TryReadFloat(property.Step, out float step) ? step : 1,
					MinValue = ParseUtils.TryReadFloat(property.MinValue, out float minValue) ? minValue : float.MinValue,
					MaxValue = ParseUtils.TryReadFloat(property.MaxValue, out float maxValue) ? maxValue : float.MaxValue,
				},
				XmlPropertyType.Vector2 => new Vector2PropertyType
				{
					DefaultValue = ParseUtils.TryReadVector2(property.DefaultValue, out Vector2 value) ? value : Vector2.Zero,
					Step = ParseUtils.TryReadFloat(property.Step, out float step) ? step : 1,
					MinValue = ParseUtils.TryReadFloat(property.MinValue, out float minValue) ? minValue : float.MinValue,
					MaxValue = ParseUtils.TryReadFloat(property.MaxValue, out float maxValue) ? maxValue : float.MaxValue,
				},
				XmlPropertyType.Vector3 => new Vector3PropertyType
				{
					DefaultValue = ParseUtils.TryReadVector3(property.DefaultValue, out Vector3 value) ? value : Vector3.Zero,
					Step = ParseUtils.TryReadFloat(property.Step, out float step) ? step : 1,
					MinValue = ParseUtils.TryReadFloat(property.MinValue, out float minValue) ? minValue : float.MinValue,
					MaxValue = ParseUtils.TryReadFloat(property.MaxValue, out float maxValue) ? maxValue : float.MaxValue,
				},
				XmlPropertyType.Vector4 => new Vector4PropertyType
				{
					DefaultValue = ParseUtils.TryReadVector4(property.DefaultValue, out Vector4 value) ? value : Vector4.Zero,
					Step = ParseUtils.TryReadFloat(property.Step, out float step) ? step : 1,
					MinValue = ParseUtils.TryReadFloat(property.MinValue, out float minValue) ? minValue : float.MinValue,
					MaxValue = ParseUtils.TryReadFloat(property.MaxValue, out float maxValue) ? maxValue : float.MaxValue,
				},
				XmlPropertyType.String => new StringPropertyType
				{
					DefaultValue = property.DefaultValue ?? string.Empty,
				},
				XmlPropertyType.Rgb => new RgbPropertyType
				{
					DefaultValue = ParseUtils.TryReadRgb(property.DefaultValue, out Rgb value) ? value : default,
				},
				XmlPropertyType.Rgba => new RgbaPropertyType
				{
					DefaultValue = ParseUtils.TryReadRgba(property.DefaultValue, out Rgba value) ? value : default,
				},
				_ => throw new InvalidOperationException($"Unknown property type: {property.Type}"),
			};
		}
	}

#pragma warning disable CA1034 // Nested types should not be visible. (Types must be public for XML deserialization to work.)
#pragma warning disable CA1002 // Do not expose generic lists. (This is required for XML deserialization to work.)
#pragma warning disable CA1720 // Identifiers must not contain type names.
#pragma warning disable SA1201 // Elements must appear in the correct order.
	[XmlRoot("EntityConfigData")]
	public record XmlEntityConfigData
	{
		[XmlAttribute]
		public required int Version { get; init; }

		[XmlElement("Entity")]
		public required List<XmlEntity> Entities { get; init; }
	}

	public record XmlEntity
	{
		[XmlAttribute]
		public required string Name { get; init; }

		[XmlAttribute]
		public required EntityShape Shape { get; init; }

		[XmlElement("Property")]
		public required List<XmlProperty> Properties { get; init; }
	}

	public record XmlProperty
	{
		[XmlAttribute]
		public required string Name { get; init; }

		[XmlAttribute]
		public required XmlPropertyType Type { get; init; }

		[XmlAttribute]
		public string? Description { get; init; }

		[XmlAttribute]
		public string? DefaultValue { get; init; }

		[XmlAttribute]
		public string? Step { get; init; }

		[XmlAttribute]
		public string? MinValue { get; init; }

		[XmlAttribute]
		public string? MaxValue { get; init; }
	}

	public enum XmlPropertyType
	{
		Bool,
		Int,
		Float,
		Vector2,
		Vector3,
		Vector4,
		String,
		Rgb,
		Rgba,
	}
#pragma warning restore SA1201, CA1720, CA1002, CA1034
}
