namespace SimpleLevelEditor.Formats.EntityConfig.Model.PropertyTypes;

public record IntPropertyType
{
	public int DefaultValue;

	public int? Step;

	public int? MinValue;

	public int? MaxValue;
}
