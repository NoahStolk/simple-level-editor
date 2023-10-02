using SimpleLevelEditor.Model;
using SimpleLevelEditor.Model.Enums;

namespace SimpleLevelEditor.State;

public static class ObjectCreatorState
{
	private static readonly WorldObject _default = new()
	{
		Mesh = string.Empty,
		Position = default,
		Rotation = default,
		Scale = Vector3.One,
		Texture = string.Empty,
		Values = WorldObjectValues.None,
		BoundingMesh = string.Empty,
	};

	public static WorldObject NewWorldObject { get; private set; } = _default.DeepCopy();

	public static bool IsValid()
	{
		return NewWorldObject.Mesh != string.Empty && NewWorldObject.Texture != string.Empty;
	}

	public static void Reset()
	{
		NewWorldObject = _default.DeepCopy();
	}
}
