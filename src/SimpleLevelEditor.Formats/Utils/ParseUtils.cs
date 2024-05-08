using System.Globalization;

namespace SimpleLevelEditor.Formats.Utils;

[Obsolete("Use DataFormatExtensions from the F# library instead.")]
internal static class ParseUtils
{
	private static bool TryReadFloat(string? str, out float result)
	{
		return float.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out result);
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
}
