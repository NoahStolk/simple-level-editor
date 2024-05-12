namespace SimpleLevelEditor.Formats.Level.Model;

public record EntityProperty
{
	public required string Key;
	public required Types.Level.EntityPropertyValue Value;

	public EntityProperty DeepCopy()
	{
		return this with
		{
			Value = Value.DeepCopy(),
		};
	}
}
