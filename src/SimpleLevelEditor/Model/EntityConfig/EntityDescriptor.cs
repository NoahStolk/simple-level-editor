namespace SimpleLevelEditor.Model.EntityConfig;

public record EntityDescriptor
{
	public required string Name;
	public required EntityShape Shape;
	public required List<EntityPropertyDescriptor> Properties;
}
