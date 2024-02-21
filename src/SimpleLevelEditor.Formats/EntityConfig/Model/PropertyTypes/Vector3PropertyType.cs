namespace SimpleLevelEditor.Formats.EntityConfig.Model.PropertyTypes;

public record Vector3PropertyType
{
	public required Vector3 DefaultValue;

	public required float? Step;

	public required float? MinValue;

	public required float? MaxValue;
}
