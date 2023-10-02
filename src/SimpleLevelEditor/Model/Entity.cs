using OneOf;
using SimpleLevelEditor.Model.EntityTypes;

namespace SimpleLevelEditor.Model;

public record Entity
{
	public required string Name { get; set; }

	public required OneOf<Point, Sphere, Aabb, StandingCylinder> Shape { get; set; }

	public required List<EntityProperty> Properties { get; set; }
}
