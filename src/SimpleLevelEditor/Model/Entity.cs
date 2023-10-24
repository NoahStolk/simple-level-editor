using OneOf;
using SimpleLevelEditor.Model.EntityTypes;

namespace SimpleLevelEditor.Model;

public record Entity
{
	public required string Name;
	public required OneOf<Point, Sphere, Aabb, StandingCylinder> Shape;
	public required List<EntityProperty> Properties;

	public Entity DeepCopy()
	{
		List<EntityProperty> newEntityProperties = new();
		for (int i = 0; i < Properties.Count; i++)
			newEntityProperties.Add(Properties[i].DeepCopy());

		return new()
		{
			Name = Name,
			Shape = Shape,
			Properties = newEntityProperties,
		};
	}
}
