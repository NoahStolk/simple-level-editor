namespace SimpleLevelEditor.Formats.EntityConfig.Model;

public record EntityDescriptor
{
	public required string Name;

	public required EntityShape Shape;

	public required List<EntityPropertyDescriptor> Properties;
}
