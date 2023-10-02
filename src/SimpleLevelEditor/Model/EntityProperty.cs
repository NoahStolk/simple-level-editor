using OneOf;

namespace SimpleLevelEditor.Model;

public record EntityProperty
{
	public required string Key { get; set; }

	public required OneOf<bool, byte, ushort, int, float, Vector2, Vector3, Vector4, Quaternion, string> Value { get; set; }
}
