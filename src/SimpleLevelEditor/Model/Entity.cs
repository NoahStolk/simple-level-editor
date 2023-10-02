using OneOf;
using SimpleLevelEditor.Model.EntityTypes;

namespace SimpleLevelEditor.Model;

public record Entity
{
	public required string Name;
	public required OneOf<Point, Sphere, Aabb, StandingCylinder> Shape;
	public required List<EntityProperty> Properties;
}
