namespace SimpleLevelEditor.Formats.EntityConfig.Model.PropertyTypes;

public record FloatPropertyType
{
	public float DefaultValue;

	public float? Step;

	public float? MinValue;

	public float? MaxValue;
}
