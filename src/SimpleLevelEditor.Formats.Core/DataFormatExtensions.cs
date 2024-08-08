using System.Globalization;
using System.Numerics;

namespace SimpleLevelEditor.Formats.Core;

public static class DataFormatExtensions
{
	public static string ToDisplayString(this bool value)
	{
		return value ? "true" : "false";
	}

	public static string ToDisplayString(this byte value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}

	public static string ToDisplayString(this int value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}

	public static string ToDisplayString(this float value)
	{
		return value.ToString(CultureInfo.InvariantCulture);
	}

	public static string ToDisplayString(this Vector2 value)
	{
		return $"{value.X.ToDisplayString()} {value.Y.ToDisplayString()}";
	}

	public static string ToDisplayString(this Vector3 value)
	{
		return $"{value.X.ToDisplayString()} {value.Y.ToDisplayString()} {value.Z.ToDisplayString()}";
	}

	public static string ToDisplayString(this Vector4 value)
	{
		return $"{value.X.ToDisplayString()} {value.Y.ToDisplayString()} {value.Z.ToDisplayString()} {value.W.ToDisplayString()}";
	}
}
