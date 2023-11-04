using OneOf;
using SimpleLevelEditor.Model.EntityTypes;

namespace SimpleLevelEditor.Model;

public record Entity
{
	/// <summary>
	/// The Id is only used to keep track of the object in the editor.
	/// </summary>
	public required int Id;

	public required string Name;
	public required OneOf<Point, Sphere, Aabb> Shape;
	public required List<EntityProperty> Properties;

	public Vector3 GetPosition()
	{
		return Shape.Value switch
		{
			Point p => p.Position,
			Sphere s => s.Position,
			Aabb a => (a.Min + a.Max) / 2,
			_ => throw new($"Unknown shape: {Shape.Value}"),
		};
	}

	public Entity DeepCopy()
	{
		List<EntityProperty> newEntityProperties = new();
		for (int i = 0; i < Properties.Count; i++)
			newEntityProperties.Add(Properties[i].DeepCopy());

		return this with
		{
			Properties = newEntityProperties,
		};
	}
}
