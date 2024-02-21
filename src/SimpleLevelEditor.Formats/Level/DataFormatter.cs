using OneOf;
using SimpleLevelEditor.Formats.Level.Model.EntityShapes;
using SimpleLevelEditor.Formats.Utils;
using System.Diagnostics;
using System.Globalization;

namespace SimpleLevelEditor.Formats.Level;

internal static class DataFormatter
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

	public static OneOf<bool, int, float, Vector2, Vector3, Vector4, string, Rgb, Rgba> ReadProperty(string str, string type)
	{
		return type switch
		{
			_boolId => ParseUtils.ReadBool(str),
			_intId => ParseUtils.ReadInt(str),
			_floatId => ParseUtils.ReadFloat(str),
			_vector2Id => ParseUtils.ReadVector2(str),
			_vector3Id => ParseUtils.ReadVector3(str),
			_vector4Id => ParseUtils.ReadVector4(str),
			_stringId => ParseUtils.ReadString(str),
			_rgbId => ParseUtils.ReadRgb(str),
			_rgbaId => ParseUtils.ReadRgba(str),
			_ => throw new UnreachableException(),
		};
	}

	public static OneOf<Point, Sphere, Aabb> ReadShape(string str)
	{
		int indexOfFirstSpace = str.IndexOf(' ', StringComparison.Ordinal);
		string shapeId = indexOfFirstSpace == -1 ? str : str[..indexOfFirstSpace];
		string shapeData = indexOfFirstSpace == -1 ? string.Empty : str[(indexOfFirstSpace + 1)..];
		return shapeId switch
		{
			_pointId => new Point(),
			_sphereId => new Sphere(float.Parse(shapeData, CultureInfo.InvariantCulture)),
			_aabbId => ReadAabb(shapeData),
			_ => throw new FormatException(),
		};

		static Aabb ReadAabb(string str)
		{
			string[] parts = str.Split(' ');
			return new(new(float.Parse(parts[0], CultureInfo.InvariantCulture), float.Parse(parts[1], CultureInfo.InvariantCulture), float.Parse(parts[2], CultureInfo.InvariantCulture)), new(float.Parse(parts[3], CultureInfo.InvariantCulture), float.Parse(parts[4], CultureInfo.InvariantCulture), float.Parse(parts[5], CultureInfo.InvariantCulture)));
		}
	}

	public static string WriteProperty(OneOf<bool, int, float, Vector2, Vector3, Vector4, string, Rgb, Rgba> value)
	{
		return value.Value switch
		{
			bool b => ParseUtils.Write(b),
			int i => ParseUtils.Write(i),
			float f => ParseUtils.Write(f),
			Vector2 v => ParseUtils.Write(v),
			Vector3 v => ParseUtils.Write(v),
			Vector4 v => ParseUtils.Write(v),
			string s => ParseUtils.Write(s),
			Rgb rgb => ParseUtils.Write(rgb),
			Rgba rgba => ParseUtils.Write(rgba),
			_ => throw new UnreachableException(),
		};
	}

	public static string WriteShape(OneOf<Point, Sphere, Aabb> shape)
	{
		return shape.Value switch
		{
			Point => _pointId,
			Sphere sphere => $"{_sphereId} {ParseUtils.Write(sphere.Radius)}",
			Aabb aabb => $"{_aabbId} {ParseUtils.Write(aabb.Min.X)} {ParseUtils.Write(aabb.Min.Y)} {ParseUtils.Write(aabb.Min.Z)} {ParseUtils.Write(aabb.Max.X)} {ParseUtils.Write(aabb.Max.Y)} {ParseUtils.Write(aabb.Max.Z)}",
			_ => throw new UnreachableException(),
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
			_ => throw new UnreachableException(),
		};
	}
}
