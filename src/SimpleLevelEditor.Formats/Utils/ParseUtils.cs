using SimpleLevelEditor.Formats.Types;
using System.Globalization;

namespace SimpleLevelEditor.Formats.Utils;

internal static class ParseUtils
{
	public static bool TryReadBool(string? str, out bool result)
	{
		return bool.TryParse(str, out result);
	}

	public static bool TryReadByte(string? str, out byte result)
	{
		return byte.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
	}

	public static bool TryReadInt(string? str, out int result)
	{
		return int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
	}

	public static bool TryReadFloat(string? str, out float result)
	{
		return float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
	}

	public static bool TryReadVector2(string? str, out Vector2 result)
	{
		result = default;

		if (str == null)
			return false;

		string[] parts = str.Split(' ');
		if (parts.Length != 2)
			return false;

		return TryReadFloat(parts[0], out result.X) && TryReadFloat(parts[1], out result.Y);
	}

	public static bool TryReadVector3(string? str, out Vector3 result)
	{
		result = default;

		if (str == null)
			return false;

		string[] parts = str.Split(' ');
		if (parts.Length != 3)
			return false;

		return TryReadFloat(parts[0], out result.X) && TryReadFloat(parts[1], out result.Y) && TryReadFloat(parts[2], out result.Z);
	}

	public static bool TryReadVector4(string? str, out Vector4 result)
	{
		result = default;

		if (str == null)
			return false;

		string[] parts = str.Split(' ');
		if (parts.Length != 4)
			return false;

		return TryReadFloat(parts[0], out result.X) && TryReadFloat(parts[1], out result.Y) && TryReadFloat(parts[2], out result.Z) && TryReadFloat(parts[3], out result.W);
	}

	public static bool TryReadRgb(string? str, out Color.Rgb result)
	{
		result = default;

		if (str == null)
			return false;

		string[] parts = str.Split(' ');
		if (parts.Length != 3)
			return false;

		return TryReadByte(parts[0], out result.R) && TryReadByte(parts[1], out result.G) && TryReadByte(parts[2], out result.B);
	}

	public static bool TryReadRgba(string? str, out Color.Rgba result)
	{
		result = default;

		if (str == null)
			return false;

		string[] parts = str.Split(' ');
		if (parts.Length != 4)
			return false;

		return TryReadByte(parts[0], out result.R) && TryReadByte(parts[1], out result.G) && TryReadByte(parts[2], out result.B) && TryReadByte(parts[3], out result.A);
	}
}
