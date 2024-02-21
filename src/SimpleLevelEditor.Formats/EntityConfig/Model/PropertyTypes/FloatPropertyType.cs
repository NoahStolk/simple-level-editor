namespace SimpleLevelEditor.Formats.EntityConfig.Model.PropertyTypes;

public record FloatPropertyType
{
	public required float DefaultValue;

	public required float? Step;

	public required float? MinValue;

	public required float? MaxValue;
}
