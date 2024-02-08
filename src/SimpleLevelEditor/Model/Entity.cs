using SimpleLevelEditor.Model.EntityShapes;

namespace SimpleLevelEditor.Model;

public record Entity
{
	/// <summary>
	/// The Id is only used to keep track of the object in the editor.
	/// </summary>
	public required int Id;

	public required string Name;
	public required Vector3 Position;
	public required IEntityShape Shape;
	public required List<EntityProperty> Properties;

	public Entity DeepCopy()
	{
		IEntityShape newShape = Shape switch
		{
			Sphere sphere => sphere.DeepCopy(),
			Point point => point.DeepCopy(),
			Aabb aabb => aabb.DeepCopy(),
			_ => throw new NotImplementedException(),
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
