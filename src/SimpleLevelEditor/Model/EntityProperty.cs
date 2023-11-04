using OneOf;

namespace SimpleLevelEditor.Model;

public record EntityProperty
{
	public required string Key;
	public required OneOf<bool, int, float, Vector2, Vector3, Vector4, string> Value;

	public EntityProperty DeepCopy()
	{
		return new()
		{
			Key = Key,
			Value = Value,
		};
	}
}
