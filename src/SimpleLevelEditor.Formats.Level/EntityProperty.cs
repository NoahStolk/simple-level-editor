using System.Diagnostics.CodeAnalysis;

namespace SimpleLevelEditor.Formats.Level;

public sealed record EntityProperty
{
	public required string Key { get; init; }

	public required EntityPropertyValue Value { get; set; }

	[SetsRequiredMembers]
	public EntityProperty(string key, EntityPropertyValue value)
	{
		Key = key;
		Value = value;
	}

	public EntityProperty DeepCopy()
	{
		return this with { Value = Value.DeepCopy() };
	}
}
