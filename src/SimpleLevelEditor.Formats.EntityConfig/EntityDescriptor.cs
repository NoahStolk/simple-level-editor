namespace SimpleLevelEditor.Formats.EntityConfig;

public record EntityDescriptor
{
	public required string Name { get; init; }

	public required EntityShapeDescriptor Shape { get; init; }

	public required List<EntityPropertyDescriptor> Properties { get; init; }

	public EntityDescriptor DeepCopy()
	{
		return this with
		{
			Shape = Shape.DeepCopy(),
			Properties = Properties.Select(p => p.DeepCopy()).ToList(),
		};
	}
}