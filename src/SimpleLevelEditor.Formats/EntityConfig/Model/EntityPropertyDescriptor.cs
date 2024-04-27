using SimpleLevelEditor.Formats.Types.EntityConfig;

namespace SimpleLevelEditor.Formats.EntityConfig.Model;

public record EntityPropertyDescriptor
{
	public required string Name;

	public required EntityPropertyTypeDescriptor Type;

	public required string? Description;
}
