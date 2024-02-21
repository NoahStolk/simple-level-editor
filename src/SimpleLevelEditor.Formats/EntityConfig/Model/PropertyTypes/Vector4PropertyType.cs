namespace SimpleLevelEditor.Formats.EntityConfig.Model.PropertyTypes;

public record Vector4PropertyType
{
	public required Vector4 DefaultValue;

	public required float? Step;

	public required float? MinValue;

	public required float? MaxValue;
}
