namespace SimpleLevelEditor.Formats.EntityConfig.Model.PropertyTypes;

public record Vector4PropertyType
{
	public Vector4 DefaultValue;

	public float? Step;

	public float? MinValue;

	public float? MaxValue;
}
