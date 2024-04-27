namespace SimpleLevelEditor.Formats.EntityConfig.Model;

public record EntityPropertyDescriptor
{
	public required string Name;

	public required Types.EntityConfig.EntityPropertyTypeDescriptor Type;

	public required string? Description;
}
