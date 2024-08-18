using System.Numerics;

namespace SimpleLevelEditorV2.Formats;

public static class DataTypeFormatting
{
	public static string FormatVector2(Vector2 vector2)
	{
		return Format(PrimitiveFormatting.FormatF32(vector2.X), PrimitiveFormatting.FormatF32(vector2.Y));
	}

	public static string FormatVector3(Vector3 vector3)
	{
		return Format(PrimitiveFormatting.FormatF32(vector3.X), PrimitiveFormatting.FormatF32(vector3.Y), PrimitiveFormatting.FormatF32(vector3.Z));
	}

	public static string FormatVector4(Vector4 vector4)
	{
		return Format(PrimitiveFormatting.FormatF32(vector4.X), PrimitiveFormatting.FormatF32(vector4.Y), PrimitiveFormatting.FormatF32(vector4.Z), PrimitiveFormatting.FormatF32(vector4.W));
	}

	public static string FormatQuaternion(Quaternion quaternion)
	{
		return Format(PrimitiveFormatting.FormatF32(quaternion.X), PrimitiveFormatting.FormatF32(quaternion.Y), PrimitiveFormatting.FormatF32(quaternion.Z), PrimitiveFormatting.FormatF32(quaternion.W));
	}

	private static string Format(params string[] values)
	{
		return string.Join(FormattingConstants.Separator, values);
	}
}
