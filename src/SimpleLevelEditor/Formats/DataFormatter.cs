using OneOf;
using SimpleLevelEditor.Data;
using SimpleLevelEditor.Model.EntityShapes;

namespace SimpleLevelEditor.Formats;

public static class DataFormatter
{
	private const string _pointId = "point";
	private const string _sphereId = "sphere";
	private const string _aabbId = "aabb";

	private const string _boolId = "bool";
	private const string _intId = "s32";
	private const string _floatId = "float";
	private const string _vector2Id = "float2";
	private const string _vector3Id = "float3";
	private const string _vector4Id = "float4";
	private const string _stringId = "str";
	private const string _rgbId = "rgb";
	private const string _rgbaId = "rgba";

	public static bool ReadBool(string str)
	{
		return bool.Parse(str);
	}

	public static int ReadInt(string str)
	{
		return int.Parse(str);
	}

	public static float ReadFloat(string str)
	{
		return float.Parse(str);
	}

	public static Vector2 ReadVector2(string str)
	{
		string[] parts = str.Split(' ');
		return new(float.Parse(parts[0]), float.Parse(parts[1]));
	}

	public static Vector3 ReadVector3(string str)
	{
		string[] parts = str.Split(' ');
		return new(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]));
	}

	public static Vector4 ReadVector4(string str)
	{
		string[] parts = str.Split(' ');
		return new(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
	}

	public static string ReadString(string str)
	{
		return str;
	}

	public static Rgb ReadRgb(string str)
	{
		string[] parts = str.Split(' ');
		return new(byte.Parse(parts[0]), byte.Parse(parts[1]), byte.Parse(parts[2]));
	}

	public static Rgba ReadRgba(string str)
	{
		string[] parts = str.Split(' ');
		return new(byte.Parse(parts[0]), byte.Parse(parts[1]), byte.Parse(parts[2]), byte.Parse(parts[3]));
	}

	public static OneOf<bool, int, float, Vector2, Vector3, Vector4, string, Rgb, Rgba> ReadProperty(string str, string type)
	{
		return type switch
		{
			_boolId => ReadBool(str),
			_intId => ReadInt(str),
			_floatId => ReadFloat(str),
			_vector2Id => ReadVector2(str),
			_vector3Id => ReadVector3(str),
			_vector4Id => ReadVector4(str),
			_stringId => ReadString(str),
			_rgbId => ReadRgb(str),
			_rgbaId => ReadRgba(str),
			_ => throw new NotImplementedException(),
		};
	}

	public static IEntityShape ReadShape(string str)
	{
		int indexOfFirstSpace = str.IndexOf(' ');
		string shapeId = indexOfFirstSpace == -1 ? str : str[..indexOfFirstSpace];
		string shapeData = indexOfFirstSpace == -1 ? string.Empty : str[(indexOfFirstSpace + 1)..];
		return shapeId switch
		{
			_pointId => new Point(),
			_sphereId => new Sphere(float.Parse(shapeData)),
			_aabbId => ReadAabb(shapeData),
			_ => throw new FormatException(),
		};

		static Aabb ReadAabb(string str)
		{
			string[] parts = str.Split(' ');
			return new(new(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2])), new(float.Parse(parts[3]), float.Parse(parts[4]), float.Parse(parts[5])));
		}
	}

	public static string Write(bool data)
	{
		return data.ToString().ToLower();
	}

	public static string Write(int data)
	{
		return data.ToString();
	}

	public static string Write(float data)
	{
		return data.ToString();
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

	public static string Write(IEntityShape shape)
	{
		return shape switch
		{
			Point => _pointId,
			Sphere sphere => $"{_sphereId} {sphere.Radius}",
			Aabb aabb => $"{_aabbId} {aabb.Min.X} {aabb.Min.Y} {aabb.Min.Z} {aabb.Max.X} {aabb.Max.Y} {aabb.Max.Z}",
			_ => throw new NotImplementedException(),
		};
	}

	public static string WritePropertyType(OneOf<bool, int, float, Vector2, Vector3, Vector4, string, Rgb, Rgba> value)
	{
		return value.Value switch
		{
			bool => _boolId,
			int => _intId,
			float => _floatId,
			Vector2 => _vector2Id,
			Vector3 => _vector3Id,
			Vector4 => _vector4Id,
			string => _stringId,
			Rgb => _rgbId,
			Rgba => _rgbaId,
			_ => throw new NotImplementedException(),
		};
	}
}
