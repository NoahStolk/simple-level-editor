using OneOf;
using SimpleLevelEditor.Data;

namespace SimpleLevelEditor.Model;

public record EntityProperty
{
	public required string Key;
	public required OneOf<bool, int, float, Vector2, Vector3, Vector4, string, Rgb, Rgba> Value;

	public EntityProperty DeepCopy()
	{
		return new()
		{
			Key = Key,
			Value = Value,
		};
	}
}
