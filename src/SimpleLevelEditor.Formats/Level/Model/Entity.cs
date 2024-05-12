using SimpleLevelEditor.Formats.Types.Level;

namespace SimpleLevelEditor.Formats.Level.Model;

public record Entity
{
	/// <summary>
	/// The Id is only used to keep track of the object in the editor.
	/// </summary>
	// TODO: Move to UI layer.
	public required int Id;

	public required string Name;
	public required Vector3 Position;
	public required ShapeDescriptor Shape;
	public required List<EntityProperty> Properties;

	public Entity DeepCopy()
	{
		ShapeDescriptor newShape = Shape.DeepCopy();

		List<EntityProperty> newEntityProperties = [];
		newEntityProperties.AddRange(Properties.Select(p => p.DeepCopy()));

		return this with
		{
			Shape = newShape,
			Properties = newEntityProperties,
		};
	}
}
