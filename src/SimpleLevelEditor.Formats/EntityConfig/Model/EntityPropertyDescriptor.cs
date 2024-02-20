using OneOf;
using SimpleLevelEditor.Formats.EntityConfig.Model.PropertyTypes;

namespace SimpleLevelEditor.Formats.EntityConfig.Model;

public record EntityPropertyDescriptor
{
	public required string Name;

	public required OneOf<BoolPropertyType, IntPropertyType, FloatPropertyType, Vector2PropertyType, Vector3PropertyType, Vector4PropertyType, StringPropertyType, RgbPropertyType, RgbaPropertyType> Type;

	public required string? Description;
}
