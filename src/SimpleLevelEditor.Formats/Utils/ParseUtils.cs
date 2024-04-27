using SimpleLevelEditor.Formats.Types;
using System.Globalization;

namespace SimpleLevelEditor.Formats.Utils;

internal static class ParseUtils
{
	#region TryRead

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

		if (!TryReadByte(parts[0], out byte r) ||
		    !TryReadByte(parts[1], out byte g) ||
		    !TryReadByte(parts[2], out byte b))
		{
			return false;
		}

		result = new Color.Rgb(r, g, b);
		return true;
	}

	public static bool TryReadRgba(string? str, out Color.Rgba result)
	{
		result = default;

		if (str == null)
			return false;

		string[] parts = str.Split(' ');
		if (parts.Length != 4)
			return false;

		if (!TryReadByte(parts[0], out byte r) ||
		    !TryReadByte(parts[1], out byte g) ||
		    !TryReadByte(parts[2], out byte b) ||
		    !TryReadByte(parts[3], out byte a))
		{
			return false;
		}

		result = new Color.Rgba(r, g, b, a);
		return true;
	}

	#endregion TryRead

	#region Read

	public static bool ReadBool(string str)
	{
		return bool.Parse(str);
	}

	public static byte ReadByte(string str)
	{
		return byte.Parse(str, CultureInfo.InvariantCulture);
	}

	public static int ReadInt(string str)
	{
		return int.Parse(str, CultureInfo.InvariantCulture);
	}

	public static float ReadFloat(string str)
	{
		return float.Parse(str, CultureInfo.InvariantCulture);
	}

	public static Vector2 ReadVector2(string str)
	{
		string[] parts = str.Split(' ');
		return new(float.Parse(parts[0], CultureInfo.InvariantCulture), float.Parse(parts[1], CultureInfo.InvariantCulture));
	}

	public static Vector3 ReadVector3(string str)
	{
		string[] parts = str.Split(' ');
		return new(float.Parse(parts[0], CultureInfo.InvariantCulture), float.Parse(parts[1], CultureInfo.InvariantCulture), float.Parse(parts[2], CultureInfo.InvariantCulture));
	}

	public static Vector4 ReadVector4(string str)
	{
		string[] parts = str.Split(' ');
		return new(float.Parse(parts[0], CultureInfo.InvariantCulture), float.Parse(parts[1], CultureInfo.InvariantCulture), float.Parse(parts[2], CultureInfo.InvariantCulture), float.Parse(parts[3], CultureInfo.InvariantCulture));
	}

	public static string ReadString(string str)
	{
		return str;
	}

	public static Color.Rgb ReadRgb(string str)
	{
		string[] parts = str.Split(' ');
		return new(byte.Parse(parts[0], CultureInfo.InvariantCulture), byte.Parse(parts[1], CultureInfo.InvariantCulture), byte.Parse(parts[2], CultureInfo.InvariantCulture));
	}

	public static Color.Rgba ReadRgba(string str)
	{
		string[] parts = str.Split(' ');
		return new(byte.Parse(parts[0], CultureInfo.InvariantCulture), byte.Parse(parts[1], CultureInfo.InvariantCulture), byte.Parse(parts[2], CultureInfo.InvariantCulture), byte.Parse(parts[3], CultureInfo.InvariantCulture));
	}

	#endregion Read

	#region Write

	public static string Write(bool data)
	{
		return data.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
	}

	public static string Write(int data)
	{
		return data.ToString(CultureInfo.InvariantCulture);
	}

	public static string Write(float data)
	{
		return data.ToString(CultureInfo.InvariantCulture);
	}

	public static string Write(Vector2 data)
	{
		return $"{Write(data.X)} {Write(data.Y)}";
	}

	public static string Write(Vector3 data)
	{
		return $"{Write(data.X)} {Write(data.Y)} {Write(data.Z)}";
	}

	public static string Write(Vector4 data)
	{
		return $"{Write(data.X)} {Write(data.Y)} {Write(data.Z)} {Write(data.W)}";
	}

	public static string Write(string data)
	{
		return data;
	}

	public static string Write(Color.Rgb data)
	{
		return $"{Write(data.R)} {Write(data.G)} {Write(data.B)}";
	}

	public static string Write(Color.Rgba data)
	{
		return $"{Write(data.R)} {Write(data.G)} {Write(data.B)} {Write(data.A)}";
	}

	#endregion Write
}
