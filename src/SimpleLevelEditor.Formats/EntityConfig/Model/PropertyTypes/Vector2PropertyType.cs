namespace SimpleLevelEditor.Formats.EntityConfig.Model.PropertyTypes;

public record Vector2PropertyType
{
	public required Vector2 DefaultValue;

	public required float? Step;

	public required float? MinValue;

	public required float? MaxValue;
}
