using SimpleLevelEditor.Model.EntityConfig;

namespace SimpleLevelEditor.Formats;

public static class EnumFormatter
{
	public static string FormatEntityShape(EntityShape shape)
	{
		return shape switch
		{
			EntityShape.Point => FormatConstants.PointId,
			EntityShape.Sphere => FormatConstants.SphereId,
			EntityShape.Aabb => FormatConstants.AabbId,
			_ => throw new ArgumentOutOfRangeException(nameof(shape), shape, null),
		};
	}

	public static EntityShape? ParseEntityShape(string? shape)
	{
		return shape switch
		{
			FormatConstants.PointId => EntityShape.Point,
			FormatConstants.SphereId => EntityShape.Sphere,
			FormatConstants.AabbId => EntityShape.Aabb,
			_ => null,
		};
	}

	public static string FormatEntityPropertyType(EntityPropertyType propertyType)
	{
		return propertyType switch
		{
			EntityPropertyType.Bool => FormatConstants.BoolId,
			EntityPropertyType.Int => FormatConstants.IntId,
			EntityPropertyType.Float => FormatConstants.FloatId,
			EntityPropertyType.Vector2 => FormatConstants.Vector2Id,
			EntityPropertyType.Vector3 => FormatConstants.Vector3Id,
			EntityPropertyType.Vector4 => FormatConstants.Vector4Id,
			EntityPropertyType.String => FormatConstants.StringId,
			EntityPropertyType.Rgb => FormatConstants.RgbId,
			EntityPropertyType.Rgba => FormatConstants.RgbaId,
			_ => throw new ArgumentOutOfRangeException(nameof(propertyType), propertyType, null),
		};
	}

	public static EntityPropertyType? ParseEntityPropertyType(string? propertyType)
	{
		return propertyType switch
		{
			FormatConstants.BoolId => EntityPropertyType.Bool,
			FormatConstants.IntId => EntityPropertyType.Int,
			FormatConstants.FloatId => EntityPropertyType.Float,
			FormatConstants.Vector2Id => EntityPropertyType.Vector2,
			FormatConstants.Vector3Id => EntityPropertyType.Vector3,
			FormatConstants.Vector4Id => EntityPropertyType.Vector4,
			FormatConstants.StringId => EntityPropertyType.String,
			FormatConstants.RgbId => EntityPropertyType.Rgb,
			FormatConstants.RgbaId => EntityPropertyType.Rgba,
			_ => null,
		};
	}
}
