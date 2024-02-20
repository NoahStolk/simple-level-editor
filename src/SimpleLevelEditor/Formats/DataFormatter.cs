using OneOf;
using SimpleLevelEditor.Data;
using SimpleLevelEditor.Model.Level.EntityShapes;
using System.Globalization;

namespace SimpleLevelEditor.Formats;

public static class DataFormatter
{
	public static bool TryReadBool(string? str, out bool result)
	{
		return bool.TryParse(str, out result);
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

		return float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out result.X) && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out result.Y);
	}

	public static bool TryReadVector3(string? str, out Vector3 result)
	{
		result = default;

		if (str == null)
			return false;

		string[] parts = str.Split(' ');
		if (parts.Length != 3)
			return false;

		return float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out result.X) && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out result.Y) && float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out result.Z);
	}

	public static bool TryReadVector4(string? str, out Vector4 result)
	{
		result = default;

		if (str == null)
			return false;

		string[] parts = str.Split(' ');
		if (parts.Length != 4)
			return false;

		return float.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out result.X) && float.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out result.Y) && float.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out result.Z) && float.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out result.W);
	}

	public static bool TryReadRgb(string? str, out Rgb result)
	{
		result = default;

		if (str == null)
			return false;

		string[] parts = str.Split(' ');
		if (parts.Length != 3)
			return false;

		return byte.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out result.R) && byte.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out result.G) && byte.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out result.B);
	}

	public static bool TryReadRgba(string? str, out Rgba result)
	{
		result = default;

		if (str == null)
			return false;

		string[] parts = str.Split(' ');
		if (parts.Length != 4)
			return false;

		return byte.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out result.R) && byte.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out result.G) && byte.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out result.B) && byte.TryParse(parts[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out result.A);
	}

	public static bool ReadBool(string str)
	{
		return bool.Parse(str);
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

	public static Rgb ReadRgb(string str)
	{
		string[] parts = str.Split(' ');
		return new(byte.Parse(parts[0], CultureInfo.InvariantCulture), byte.Parse(parts[1], CultureInfo.InvariantCulture), byte.Parse(parts[2], CultureInfo.InvariantCulture));
	}

	public static Rgba ReadRgba(string str)
	{
		string[] parts = str.Split(' ');
		return new(byte.Parse(parts[0], CultureInfo.InvariantCulture), byte.Parse(parts[1], CultureInfo.InvariantCulture), byte.Parse(parts[2], CultureInfo.InvariantCulture), byte.Parse(parts[3], CultureInfo.InvariantCulture));
	}

	public static OneOf<bool, int, float, Vector2, Vector3, Vector4, string, Rgb, Rgba> ReadProperty(string str, string type)
	{
		return type switch
		{
			FormatConstants.BoolId => ReadBool(str),
			FormatConstants.IntId => ReadInt(str),
			FormatConstants.FloatId => ReadFloat(str),
			FormatConstants.Vector2Id => ReadVector2(str),
			FormatConstants.Vector3Id => ReadVector3(str),
			FormatConstants.Vector4Id => ReadVector4(str),
			FormatConstants.StringId => ReadString(str),
			FormatConstants.RgbId => ReadRgb(str),
			FormatConstants.RgbaId => ReadRgba(str),
			_ => throw new NotImplementedException(),
		};
	}

	public static OneOf<Point, Sphere, Aabb> ReadShape(string str)
	{
		int indexOfFirstSpace = str.IndexOf(' ', StringComparison.Ordinal);
		string shapeId = indexOfFirstSpace == -1 ? str : str[..indexOfFirstSpace];
		string shapeData = indexOfFirstSpace == -1 ? string.Empty : str[(indexOfFirstSpace + 1)..];
		return shapeId switch
		{
			FormatConstants.PointId => new Point(),
			FormatConstants.SphereId => new Sphere(float.Parse(shapeData, CultureInfo.InvariantCulture)),
			FormatConstants.AabbId => ReadAabb(shapeData),
			_ => throw new FormatException(),
		};

		static Aabb ReadAabb(string str)
		{
			string[] parts = str.Split(' ');
			return new(new(float.Parse(parts[0], CultureInfo.InvariantCulture), float.Parse(parts[1], CultureInfo.InvariantCulture), float.Parse(parts[2], CultureInfo.InvariantCulture)), new(float.Parse(parts[3], CultureInfo.InvariantCulture), float.Parse(parts[4], CultureInfo.InvariantCulture), float.Parse(parts[5], CultureInfo.InvariantCulture)));
		}
	}

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
		return $"{data.X} {data.Y}";
	}

	public static string Write(Vector3 data)
	{
		return $"{data.X} {data.Y} {data.Z}";
	}

	public static string Write(Vector4 data)
	{
		return $"{data.X} {data.Y} {data.Z} {data.W}";
	}

	public static string Write(string data)
	{
		return data;
	}

	public static string Write(Rgb data)
	{
		return $"{data.R} {data.G} {data.B}";
	}

	public static string Write(Rgba data)
	{
		return $"{data.R} {data.G} {data.B} {data.A}";
	}

	public static string Write(OneOf<bool, int, float, Vector2, Vector3, Vector4, string, Rgb, Rgba> value)
	{
		return value.Value switch
		{
			bool b => Write(b),
			int i => Write(i),
			float f => Write(f),
			Vector2 v => Write(v),
			Vector3 v => Write(v),
			Vector4 v => Write(v),
			string s => Write(s),
			Rgb rgb => Write(rgb),
			Rgba rgba => Write(rgba),
			_ => throw new NotImplementedException(),
		};
	}

	public static string Write(OneOf<Point, Sphere, Aabb> shape)
	{
		return shape.Value switch
		{
			Point => FormatConstants.PointId,
			Sphere sphere => $"{FormatConstants.SphereId} {sphere.Radius}",
			Aabb aabb => $"{FormatConstants.AabbId} {aabb.Min.X} {aabb.Min.Y} {aabb.Min.Z} {aabb.Max.X} {aabb.Max.Y} {aabb.Max.Z}",
			_ => throw new NotImplementedException(),
		};
	}

	public static string WritePropertyType(OneOf<bool, int, float, Vector2, Vector3, Vector4, string, Rgb, Rgba> value)
	{
		return value.Value switch
		{
			bool => FormatConstants.BoolId,
			int => FormatConstants.IntId,
			float => FormatConstants.FloatId,
			Vector2 => FormatConstants.Vector2Id,
			Vector3 => FormatConstants.Vector3Id,
			Vector4 => FormatConstants.Vector4Id,
			string => FormatConstants.StringId,
			Rgb => FormatConstants.RgbId,
			Rgba => FormatConstants.RgbaId,
			_ => throw new NotImplementedException(),
		};
	}
}
