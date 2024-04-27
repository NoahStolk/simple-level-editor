using SimpleLevelEditor.Formats.Utils;
using System.Diagnostics;
using System.Globalization;

namespace SimpleLevelEditor.Formats.Level;

internal static class LevelXmlDataFormatter
{
	private const string _pointId = "point";
	private const string _sphereId = "sphere";
	private const string _aabbId = "aabb";

	public static Types.Level.EntityPropertyValue ReadProperty(string str)
	{
		int indexOfSpace = str.IndexOf(' ', StringComparison.Ordinal);
		string type = str[..indexOfSpace];
		string value = str[(indexOfSpace + 1)..];

		return Types.Level.EntityPropertyValue.FromTypeId(type, value);
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
}
