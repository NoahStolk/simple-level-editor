using SimpleLevelEditor.Formats.Types.Level;
using SimpleLevelEditor.Formats.Utils;
using System.Diagnostics;
using System.Globalization;

namespace SimpleLevelEditor.Formats.Level;

internal static class LevelXmlDataFormatter
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

	public static EntityPropertyValue ReadProperty(string str)
	{
		int indexOfSpace = str.IndexOf(' ', StringComparison.Ordinal);
		string type = str[..indexOfSpace];
		string value = str[(indexOfSpace + 1)..];

		return type switch
		{
			_boolId => EntityPropertyValue.NewBool(ParseUtils.ReadBool(value)),
			_intId => EntityPropertyValue.NewInt(ParseUtils.ReadInt(value)),
			_floatId => EntityPropertyValue.NewFloat(ParseUtils.ReadFloat(value)),
			_vector2Id => EntityPropertyValue.NewVector2(ParseUtils.ReadVector2(value)),
			_vector3Id => EntityPropertyValue.NewVector3(ParseUtils.ReadVector3(value)),
			_vector4Id => EntityPropertyValue.NewVector4(ParseUtils.ReadVector4(value)),
			_stringId => EntityPropertyValue.NewString(ParseUtils.ReadString(value)),
			_rgbId => EntityPropertyValue.NewRgb(ParseUtils.ReadRgb(value)),
			_rgbaId => EntityPropertyValue.NewRgba(ParseUtils.ReadRgba(value)),
			_ => throw new UnreachableException(),
		};
	}

	public static ShapeDescriptor ReadShape(string str)
	{
		int indexOfFirstSpace = str.IndexOf(' ', StringComparison.Ordinal);
		string shapeId = indexOfFirstSpace == -1 ? str : str[..indexOfFirstSpace];
		string shapeData = indexOfFirstSpace == -1 ? string.Empty : str[(indexOfFirstSpace + 1)..];
		return shapeId switch
		{
			_pointId => ShapeDescriptor.Point,
			_sphereId => ShapeDescriptor.NewSphere(float.Parse(shapeData, CultureInfo.InvariantCulture)),
			_aabbId => ReadAabb(shapeData),
			_ => throw new FormatException(),
		};

		static ShapeDescriptor ReadAabb(string str)
		{
			string[] parts = str.Split(' ');
			return ShapeDescriptor.NewAabb(
				new Vector3(float.Parse(parts[0], CultureInfo.InvariantCulture), float.Parse(parts[1], CultureInfo.InvariantCulture), float.Parse(parts[2], CultureInfo.InvariantCulture)),
				new Vector3(float.Parse(parts[3], CultureInfo.InvariantCulture), float.Parse(parts[4], CultureInfo.InvariantCulture), float.Parse(parts[5], CultureInfo.InvariantCulture)));
		}
	}

	public static string WriteProperty(EntityPropertyValue value)
	{
		return value switch
		{
			EntityPropertyValue.Bool b => ParseUtils.Write(b.Value),
			EntityPropertyValue.Int i => ParseUtils.Write(i.Value),
			EntityPropertyValue.Float f => ParseUtils.Write(f.Value),
			EntityPropertyValue.Vector2 v => ParseUtils.Write(v.Value),
			EntityPropertyValue.Vector3 v => ParseUtils.Write(v.Value),
			EntityPropertyValue.Vector4 v => ParseUtils.Write(v.Value),
			EntityPropertyValue.String s => ParseUtils.Write(s.Value),
			EntityPropertyValue.Rgb rgb => ParseUtils.Write(rgb.Value),
			EntityPropertyValue.Rgba rgba => ParseUtils.Write(rgba.Value),
			_ => throw new UnreachableException(),
		};
	}

	public static string WriteShape(ShapeDescriptor shape)
	{
		if (shape.IsPoint)
			return _pointId;

		return shape switch
		{
			ShapeDescriptor.Sphere sphere => $"{_sphereId} {ParseUtils.Write(sphere.Radius)}",
			ShapeDescriptor.Aabb aabb => $"{_aabbId} {ParseUtils.Write(aabb.Min.X)} {ParseUtils.Write(aabb.Min.Y)} {ParseUtils.Write(aabb.Min.Z)} {ParseUtils.Write(aabb.Max.X)} {ParseUtils.Write(aabb.Max.Y)} {ParseUtils.Write(aabb.Max.Z)}",
			_ => throw new UnreachableException(),
		};
	}

	public static string WritePropertyType(EntityPropertyValue value)
	{
		return value switch
		{
			EntityPropertyValue.Bool => _boolId,
			EntityPropertyValue.Int => _intId,
			EntityPropertyValue.Float => _floatId,
			EntityPropertyValue.Vector2 => _vector2Id,
			EntityPropertyValue.Vector3 => _vector3Id,
			EntityPropertyValue.Vector4 => _vector4Id,
			EntityPropertyValue.String => _stringId,
			EntityPropertyValue.Rgb => _rgbId,
			EntityPropertyValue.Rgba => _rgbaId,
			_ => throw new UnreachableException(),
		};
	}
}
