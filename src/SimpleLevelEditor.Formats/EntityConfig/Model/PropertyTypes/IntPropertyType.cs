namespace SimpleLevelEditor.Formats.EntityConfig.Model.PropertyTypes;

public record IntPropertyType
{
	public required int DefaultValue;

	public required int? Step;

	public required int? MinValue;

	public required int? MaxValue;
}
