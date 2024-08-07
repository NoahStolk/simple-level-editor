using Dunet;
using SimpleLevelEditor.Formats.Core;

namespace SimpleLevelEditor.Formats.EntityConfig;

[Union]
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
