using Dunet;
using SimpleLevelEditor.Formats.Core;

namespace SimpleLevelEditor.Formats.EntityConfig;

[Union]
public partial record EntityShapeDescriptor
{
	public sealed partial record Point(PointEntityVisualization Visualization);
	public sealed partial record Sphere(Rgb Color);
	public sealed partial record Aabb(Rgb Color);

	// public EntityShape GetDefaultEntityShape()
	// {
	// 	return this switch
	// 	{
	// 		Point _ => new Point(new PointEntityVisualization.SimpleSphere(new Rgb(255, 255, 255), 1)),
	// 		Sphere _ => new Sphere(new Rgb(255, 255, 255)),
	// 		Aabb _ => new Aabb(new Rgb(255, 255, 255)),
	// 		_ => throw new InvalidOperationException()
	// 	};
	// }

	public string GetTypeId()
	{
		return this switch
		{
			Point => nameof(Point),
			Sphere => nameof(Sphere),
			Aabb => nameof(Aabb),
			_ => throw new InvalidOperationException(),
		};
	}

	public EntityShapeDescriptor DeepCopy()
	{
		return this switch
		{
			Point point => new Point(point.Visualization.DeepCopy()),
			Sphere sphere => new Sphere(sphere.Color),
			Aabb aabb => new Aabb(aabb.Color),
			_ => throw new InvalidOperationException(),
		};
	}
}
