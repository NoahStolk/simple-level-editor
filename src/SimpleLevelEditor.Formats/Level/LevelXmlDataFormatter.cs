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

	public static Types.Level.EntityPropertyValue ReadProperty(string str)
	{
		int indexOfSpace = str.IndexOf(' ', StringComparison.Ordinal);
		string type = str[..indexOfSpace];
		string value = str[(indexOfSpace + 1)..];

		return type switch
		{
			_boolId => Types.Level.EntityPropertyValue.NewBool(ParseUtils.ReadBool(value)),
			_intId => Types.Level.EntityPropertyValue.NewInt(ParseUtils.ReadInt(value)),
			_floatId => Types.Level.EntityPropertyValue.NewFloat(ParseUtils.ReadFloat(value)),
			_vector2Id => Types.Level.EntityPropertyValue.NewVector2(ParseUtils.ReadVector2(value)),
			_vector3Id => Types.Level.EntityPropertyValue.NewVector3(ParseUtils.ReadVector3(value)),
			_vector4Id => Types.Level.EntityPropertyValue.NewVector4(ParseUtils.ReadVector4(value)),
			_stringId => Types.Level.EntityPropertyValue.NewString(ParseUtils.ReadString(value)),
			_rgbId => Types.Level.EntityPropertyValue.NewRgb(ParseUtils.ReadRgb(value)),
			_rgbaId => Types.Level.EntityPropertyValue.NewRgba(ParseUtils.ReadRgba(value)),
			_ => throw new UnreachableException(),
		};
	}

	public static Types.Level.ShapeDescriptor ReadShape(string str)
	{
		int indexOfFirstSpace = str.IndexOf(' ', StringComparison.Ordinal);
		string shapeId = indexOfFirstSpace == -1 ? str : str[..indexOfFirstSpace];
		string shapeData = indexOfFirstSpace == -1 ? string.Empty : str[(indexOfFirstSpace + 1)..];
		return shapeId switch
		{
			_pointId => Types.Level.ShapeDescriptor.Point,
			_sphereId => Types.Level.ShapeDescriptor.NewSphere(float.Parse(shapeData, CultureInfo.InvariantCulture)),
			_aabbId => ReadAabb(shapeData),
			_ => throw new FormatException(),
		};

		static Types.Level.ShapeDescriptor ReadAabb(string str)
		{
			string[] parts = str.Split(' ');
			return Types.Level.ShapeDescriptor.NewAabb(
				new Vector3(float.Parse(parts[0], CultureInfo.InvariantCulture), float.Parse(parts[1], CultureInfo.InvariantCulture), float.Parse(parts[2], CultureInfo.InvariantCulture)),
				new Vector3(float.Parse(parts[3], CultureInfo.InvariantCulture), float.Parse(parts[4], CultureInfo.InvariantCulture), float.Parse(parts[5], CultureInfo.InvariantCulture)));
		}
	}

	public static string WriteProperty(Types.Level.EntityPropertyValue value)
	{
		return value switch
		{
			Types.Level.EntityPropertyValue.Bool b => ParseUtils.Write(b.Value),
			Types.Level.EntityPropertyValue.Int i => ParseUtils.Write(i.Value),
			Types.Level.EntityPropertyValue.Float f => ParseUtils.Write(f.Value),
			Types.Level.EntityPropertyValue.Vector2 v => ParseUtils.Write(v.Value),
			Types.Level.EntityPropertyValue.Vector3 v => ParseUtils.Write(v.Value),
			Types.Level.EntityPropertyValue.Vector4 v => ParseUtils.Write(v.Value),
			Types.Level.EntityPropertyValue.String s => ParseUtils.Write(s.Value),
			Types.Level.EntityPropertyValue.Rgb rgb => ParseUtils.Write(rgb.Value),
			Types.Level.EntityPropertyValue.Rgba rgba => ParseUtils.Write(rgba.Value),
			_ => throw new UnreachableException(),
		};
	}

	public static string WriteShape(Types.Level.ShapeDescriptor shape)
	{
		if (shape.IsPoint)
			return _pointId;

		return shape switch
		{
			Types.Level.ShapeDescriptor.Sphere sphere => $"{_sphereId} {ParseUtils.Write(sphere.Radius)}",
			Types.Level.ShapeDescriptor.Aabb aabb => $"{_aabbId} {ParseUtils.Write(aabb.Min.X)} {ParseUtils.Write(aabb.Min.Y)} {ParseUtils.Write(aabb.Min.Z)} {ParseUtils.Write(aabb.Max.X)} {ParseUtils.Write(aabb.Max.Y)} {ParseUtils.Write(aabb.Max.Z)}",
			_ => throw new UnreachableException(),
		};
	}

	public static string WritePropertyType(Types.Level.EntityPropertyValue value)
	{
		return value switch
		{
			Types.Level.EntityPropertyValue.Bool => _boolId,
			Types.Level.EntityPropertyValue.Int => _intId,
			Types.Level.EntityPropertyValue.Float => _floatId,
			Types.Level.EntityPropertyValue.Vector2 => _vector2Id,
			Types.Level.EntityPropertyValue.Vector3 => _vector3Id,
			Types.Level.EntityPropertyValue.Vector4 => _vector4Id,
			Types.Level.EntityPropertyValue.String => _stringId,
			Types.Level.EntityPropertyValue.Rgb => _rgbId,
			Types.Level.EntityPropertyValue.Rgba => _rgbaId,
			_ => throw new UnreachableException(),
		};
	}
}
