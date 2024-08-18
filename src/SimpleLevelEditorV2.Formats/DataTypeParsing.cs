using System.Numerics;

namespace SimpleLevelEditorV2.Formats;

public static class DataTypeParsing
{
	public static bool TryParseVector2(string value, out Vector2 vector2)
	{
		vector2 = default;
		if (!TryGetElements(value, 2, out string[] split))
			return false;

		if (!PrimitiveParsing.TryParseF32(split[0], out float x) ||
		    !PrimitiveParsing.TryParseF32(split[1], out float y))
		{
			return false;
		}

		vector2 = new Vector2(x, y);
		return true;
	}

	public static bool TryParseVector3(string value, out Vector3 vector3)
	{
		vector3 = default;
		if (!TryGetElements(value, 2, out string[] split))
			return false;

		if (!PrimitiveParsing.TryParseF32(split[0], out float x) ||
		    !PrimitiveParsing.TryParseF32(split[1], out float y) ||
		    !PrimitiveParsing.TryParseF32(split[2], out float z))
		{
			return false;
		}

		vector3 = new Vector3(x, y, z);
		return true;
	}

	public static bool TryParseVector4(string value, out Vector4 vector4)
	{
		vector4 = default;
		if (!TryGetElements(value, 2, out string[] split))
			return false;

		if (!PrimitiveParsing.TryParseF32(split[0], out float x) ||
		    !PrimitiveParsing.TryParseF32(split[1], out float y) ||
		    !PrimitiveParsing.TryParseF32(split[2], out float z) ||
		    !PrimitiveParsing.TryParseF32(split[3], out float w))
		{
			return false;
		}

		vector4 = new Vector4(x, y, z, w);
		return true;
	}

	public static bool TryParseQuaternion(string value, out Quaternion quaternion)
	{
		quaternion = default;
		if (!TryGetElements(value, 2, out string[] split))
			return false;

		if (!PrimitiveParsing.TryParseF32(split[0], out float x) ||
		    !PrimitiveParsing.TryParseF32(split[1], out float y) ||
		    !PrimitiveParsing.TryParseF32(split[2], out float z) ||
		    !PrimitiveParsing.TryParseF32(split[3], out float w))
		{
			return false;
		}

		quaternion = new Quaternion(x, y, z, w);
		return true;
	}

	private static bool TryGetElements(string value, int expectedElements, out string[] split)
	{
		split = value.Split(FormattingConstants.Separator);
		return split.Length == expectedElements;
	}
}
