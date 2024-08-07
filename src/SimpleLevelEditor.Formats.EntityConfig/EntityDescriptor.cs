using System.Diagnostics.CodeAnalysis;

namespace SimpleLevelEditor.Formats.EntityConfig;

public record EntityDescriptor
{
	public required string Name { get; init; }

	public required EntityShapeDescriptor Shape { get; init; }

	public required List<EntityPropertyDescriptor> Properties { get; init; }

	[SetsRequiredMembers]
	public EntityDescriptor(string name, EntityShapeDescriptor shape, List<EntityPropertyDescriptor> properties)
	{
		Name = name;
		Shape = shape;
		Properties = properties;
	}

	public EntityDescriptor DeepCopy()
	{
		return this with
		{
			Shape = Shape.DeepCopy(),
			Properties = Properties.Select(p => p.DeepCopy()).ToList(),
		};
	}
}
