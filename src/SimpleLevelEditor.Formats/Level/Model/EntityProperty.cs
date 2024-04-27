using SimpleLevelEditor.Formats.Types.Level;

namespace SimpleLevelEditor.Formats.Level.Model;

public record EntityProperty
{
	public required string Key;
	public required EntityPropertyValue Value;

	public EntityProperty DeepCopy()
	{
		return new EntityProperty
		{
			Key = Key,
			Value = Value,
		};
	}
}
