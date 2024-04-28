using SimpleLevelEditor.Formats.EntityConfig.Model;
using SimpleLevelEditor.Formats.EntityConfig.XmlModel;
using SimpleLevelEditor.Formats.Types;
using SimpleLevelEditor.Formats.Types.EntityConfig;
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
			Entities = xmlEntityConfigData.Entities.ConvertAll(e =>
			{
				int indexOfFirstDelimiter = e.Shape.IndexOf(';', StringComparison.Ordinal);
				if (indexOfFirstDelimiter == -1)
					throw new InvalidOperationException("Invalid shape format.");

				string shapeId = e.Shape[..indexOfFirstDelimiter];
				string shapeData = e.Shape[(indexOfFirstDelimiter + 1)..];

				return new EntityDescriptor
				{
					Name = e.Name,
					Shape = EntityShape.FromIdData(shapeId, shapeData),
					Properties = e.Properties.ConvertAll(p => new EntityPropertyDescriptor
					{
						Name = p.Name,
						Type = CreatePropertyType(p),
						Description = p.Description,
					}),
				};
			}),
		};

		return entityConfig;

		// TODO: Move this to F#.
		static EntityPropertyTypeDescriptor CreatePropertyType(XmlEntityConfigProperty property)
		{
			return property.Type switch
			{
				XmlEntityConfigPropertyType.Bool => EntityPropertyTypeDescriptor.NewBoolProperty(ParseUtils.TryReadBool(property.DefaultValue, out bool value) && value),
				XmlEntityConfigPropertyType.Int => EntityPropertyTypeDescriptor.NewIntProperty(
					ParseUtils.TryReadInt(property.DefaultValue, out int value) ? value : 0,
					ParseUtils.TryReadInt(property.Step, out int step) ? step : 1,
					ParseUtils.TryReadInt(property.MinValue, out int minValue) ? minValue : int.MinValue,
					ParseUtils.TryReadInt(property.MaxValue, out int maxValue) ? maxValue : int.MaxValue),
				XmlEntityConfigPropertyType.Float => EntityPropertyTypeDescriptor.NewFloatProperty(
					ParseUtils.TryReadFloat(property.DefaultValue, out float value) ? value : 0f,
					ParseUtils.TryReadFloat(property.Step, out float step) ? step : 1,
					ParseUtils.TryReadFloat(property.MinValue, out float minValue) ? minValue : float.MinValue,
					ParseUtils.TryReadFloat(property.MaxValue, out float maxValue) ? maxValue : float.MaxValue),
				XmlEntityConfigPropertyType.Vector2 => EntityPropertyTypeDescriptor.NewVector2Property(
					ParseUtils.TryReadVector2(property.DefaultValue, out Vector2 value) ? value : Vector2.Zero,
					ParseUtils.TryReadFloat(property.Step, out float step) ? step : 1,
					ParseUtils.TryReadFloat(property.MinValue, out float minValue) ? minValue : float.MinValue,
					ParseUtils.TryReadFloat(property.MaxValue, out float maxValue) ? maxValue : float.MaxValue),
				XmlEntityConfigPropertyType.Vector3 => EntityPropertyTypeDescriptor.NewVector3Property(
					ParseUtils.TryReadVector3(property.DefaultValue, out Vector3 value) ? value : Vector3.Zero,
					ParseUtils.TryReadFloat(property.Step, out float step) ? step : 1,
					ParseUtils.TryReadFloat(property.MinValue, out float minValue) ? minValue : float.MinValue,
					ParseUtils.TryReadFloat(property.MaxValue, out float maxValue) ? maxValue : float.MaxValue),
				XmlEntityConfigPropertyType.Vector4 => EntityPropertyTypeDescriptor.NewVector4Property(
					ParseUtils.TryReadVector4(property.DefaultValue, out Vector4 value) ? value : Vector4.Zero,
					ParseUtils.TryReadFloat(property.Step, out float step) ? step : 1,
					ParseUtils.TryReadFloat(property.MinValue, out float minValue) ? minValue : float.MinValue,
					ParseUtils.TryReadFloat(property.MaxValue, out float maxValue) ? maxValue : float.MaxValue),
				XmlEntityConfigPropertyType.String => EntityPropertyTypeDescriptor.NewStringProperty(property.DefaultValue ?? string.Empty),
				XmlEntityConfigPropertyType.Rgb => EntityPropertyTypeDescriptor.NewRgbProperty(ParseUtils.TryReadRgb(property.DefaultValue, out Rgb value) ? value : default),
				XmlEntityConfigPropertyType.Rgba => EntityPropertyTypeDescriptor.NewRgbaProperty(ParseUtils.TryReadRgba(property.DefaultValue, out Rgba value) ? value : default),
				_ => throw new InvalidOperationException($"Unknown property type: {property.Type}"),
			};
		}
	}
}
