namespace SimpleLevelEditor.Formats.EntityConfig.Model.PropertyTypes;

public record Vector3PropertyType
{
	public Vector3 DefaultValue;

	public float? Step;

	public float? MinValue;

	public float? MaxValue;
}
