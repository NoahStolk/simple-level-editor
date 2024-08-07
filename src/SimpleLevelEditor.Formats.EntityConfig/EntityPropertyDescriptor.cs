using System.Diagnostics.CodeAnalysis;

namespace SimpleLevelEditor.Formats.EntityConfig;

public sealed record EntityPropertyDescriptor
{
	[SetsRequiredMembers]
	public EntityPropertyDescriptor(string name, EntityPropertyTypeDescriptor type, string? description)
	{
		Name = name;
		Type = type;
		Description = description;
	}

	public required string Name { get; init; }

	public required EntityPropertyTypeDescriptor Type { get; init; }

	public required string? Description { get; init; }

	public EntityPropertyDescriptor DeepCopy()
	{
		return this with { Type = Type.DeepCopy() };
	}
}
