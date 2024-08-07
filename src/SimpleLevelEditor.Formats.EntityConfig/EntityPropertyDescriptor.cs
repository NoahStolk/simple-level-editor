namespace SimpleLevelEditor.Formats.EntityConfig;

public sealed record EntityPropertyDescriptor
{
	public required string Name { get; init; }

	public required EntityPropertyTypeDescriptor Type { get; init; }

	public required string? Description { get; init; }

	public EntityPropertyDescriptor DeepCopy()
	{
		return this with { Type = Type.DeepCopy() };
	}
}
