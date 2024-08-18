using System.Globalization;

namespace SimpleLevelEditorV2.Formats;

public static class PrimitiveParsing
{
	public static bool TryParseBool(string value, out bool result)
	{
		return bool.TryParse(value, out result);
	}

	public static bool TryParseI8(string value, out sbyte result)
	{
		return sbyte.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
	}

	public static bool TryParseI16(string value, out short result)
	{
		return short.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
	}

	public static bool TryParseI32(string value, out int result)
	{
		return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
	}

	public static bool TryParseI64(string value, out long result)
	{
		return long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
	}

	public static bool TryParseI128(string value, out Int128 result)
	{
		return Int128.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
	}

	public static bool TryParseU8(string value, out byte result)
	{
		return byte.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
	}

	public static bool TryParseU16(string value, out ushort result)
	{
		return ushort.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
	}

	public static bool TryParseU32(string value, out uint result)
	{
		return uint.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
	}

	public static bool TryParseU64(string value, out ulong result)
	{
		return ulong.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
	}

	public static bool TryParseU128(string value, out UInt128 result)
	{
		return UInt128.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
	}

	public static bool TryParseF16(string value, out Half result)
	{
		return Half.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
	}

	public static bool TryParseF32(string value, out float result)
	{
		return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
	}

	public static bool TryParseF64(string value, out double result)
	{
		return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
	}

	public static bool TryParseStr(string value, out string result)
	{
		result = value;
		return true;
	}
}
