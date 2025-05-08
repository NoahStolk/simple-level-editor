using Dunet;
using SimpleLevelEditor.Formats.Core;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace SimpleLevelEditor.Formats.EntityConfig;

[Union]
[JsonDerivedType(typeof(Point), typeDiscriminator: nameof(Point))]
[JsonDerivedType(typeof(Sphere), typeDiscriminator: nameof(Sphere))]
[JsonDerivedType(typeof(Aabb), typeDiscriminator: nameof(Aabb))]
public partial record EntityShapeDescriptor
{
	public sealed partial record Point(PointEntityVisualization Visualization);
	public sealed partial record Sphere(Rgb Color);
	public sealed partial record Aabb(Rgb Color);

	public string GetTypeId()
	{
		return this switch
		{
			Point => nameof(Point),
			Sphere => nameof(Sphere),
			Aabb => nameof(Aabb),
			_ => throw new UnreachableException($"Unknown type: {GetType().FullName}"),
		};
	}

	public EntityShapeDescriptor DeepCopy()
	{
		return this switch
		{
			Point point => new Point(point.Visualization.DeepCopy()),
			Sphere sphere => new Sphere(sphere.Color),
			Aabb aabb => new Aabb(aabb.Color),
			_ => throw new UnreachableException($"Unknown type: {GetType().FullName}"),
		};
	}
}
