namespace SimpleLevelEditor.Formats.Level;

public sealed record EntityProperty
{
	public required string Key { get; init; }

	public required EntityPropertyValue Value { get; set; }

	public EntityProperty DeepCopy()
	{
		return this with { Value = Value.DeepCopy() };
	}
}
