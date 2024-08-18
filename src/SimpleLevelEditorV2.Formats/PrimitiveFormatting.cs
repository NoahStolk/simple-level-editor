using System.Globalization;

namespace SimpleLevelEditorV2.Formats;

public static class PrimitiveFormatting
{
	public static string FormatBool(bool value)
	{
		return value ? bool.TrueString : bool.FalseString;
	}

	public static string FormatI8(sbyte value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}

	public static string FormatI16(short value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}

	public static string FormatI32(int value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}

	public static string FormatI64(long value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}

	public static string FormatI128(Int128 value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}

	public static string FormatU8(byte value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}

	public static string FormatU16(ushort value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}

	public static string FormatU32(uint value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}

	public static string FormatU64(ulong value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}

	public static string FormatU128(UInt128 value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}

	public static string FormatF16(Half value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}

	public static string FormatF32(float value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}

	public static string FormatF64(double value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}

	public static string FormatStr(string value)
	{
		return value;
	}
}
