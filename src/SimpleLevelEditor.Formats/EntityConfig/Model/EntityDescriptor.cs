namespace SimpleLevelEditor.Formats.EntityConfig.Model;

public record EntityDescriptor
{
	public required string Name;

	public required Types.EntityConfig.EntityShape Shape;

	public required List<EntityPropertyDescriptor> Properties;
}
