using SimpleLevelEditor.Formats.EntityConfig;
using SimpleLevelEditor.Formats.Level;
using System.Diagnostics;

namespace SimpleLevelEditor.Formats;

public static class EntityPropertyTypeDescriptorExtensions
{
	public static EntityPropertyValue GetDefaultValue(this EntityPropertyTypeDescriptor entityPropertyTypeDescriptor)
	{
		return entityPropertyTypeDescriptor switch
		{
			EntityPropertyTypeDescriptor.BoolProperty boolProperty => new EntityPropertyValue.Bool(boolProperty.Default),
			EntityPropertyTypeDescriptor.IntProperty intProperty => new EntityPropertyValue.Int(intProperty.Default),
			EntityPropertyTypeDescriptor.FloatProperty floatProperty => new EntityPropertyValue.Float(floatProperty.Default),
			EntityPropertyTypeDescriptor.Vector2Property vector2Property => new EntityPropertyValue.Vector2(vector2Property.Default),
			EntityPropertyTypeDescriptor.Vector3Property vector3Property => new EntityPropertyValue.Vector3(vector3Property.Default),
			EntityPropertyTypeDescriptor.Vector4Property vector4Property => new EntityPropertyValue.Vector4(vector4Property.Default),
			EntityPropertyTypeDescriptor.StringProperty stringProperty => new EntityPropertyValue.String(stringProperty.Default),
			EntityPropertyTypeDescriptor.RgbProperty rgbProperty => new EntityPropertyValue.Rgb(rgbProperty.Default),
			EntityPropertyTypeDescriptor.RgbaProperty rgbaProperty => new EntityPropertyValue.Rgba(rgbaProperty.Default),
			_ => throw new UnreachableException($"Unknown entity property type descriptor: {entityPropertyTypeDescriptor.GetType().FullName}"),
		};
	}
}
