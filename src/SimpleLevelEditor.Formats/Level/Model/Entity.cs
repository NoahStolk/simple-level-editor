using OneOf;
using SimpleLevelEditor.Formats.Level.Model.EntityShapes;
using System.Diagnostics;

namespace SimpleLevelEditor.Formats.Level.Model;

public record Entity
{
	public required string Name;
	public required Vector3 Position;
	public required OneOf<Point, Sphere, Aabb> Shape;
	public required List<EntityProperty> Properties;

	public Entity DeepCopy()
	{
		OneOf<Point, Sphere, Aabb> newShape = Shape.Value switch
		{
			Point point => point.DeepCopy(),
			Sphere sphere => sphere.DeepCopy(),
			Aabb aabb => aabb.DeepCopy(),
			_ => throw new UnreachableException(),
		};

		List<EntityProperty> newEntityProperties = [];
		for (int i = 0; i < Properties.Count; i++)
			newEntityProperties.Add(Properties[i].DeepCopy());

		return this with
		{
			Shape = newShape,
			Properties = newEntityProperties,
		};
	}
}
