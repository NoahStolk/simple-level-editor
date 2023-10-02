using SimpleLevelEditor.Model.Enums;

namespace SimpleLevelEditor.Model;

public record WorldObject
{
	public required string Mesh;
	public required string Texture;
	public required string BoundingMesh;
	public required Vector3 Scale;
	public required Vector3 Rotation;
	public required Vector3 Position;
	public required WorldObjectValues Values;

	public WorldObject DeepCopy()
	{
		return new()
		{
			Mesh = Mesh,
			Position = Position,
			Scale = Scale,
			Rotation = Rotation,
			Texture = Texture,
			BoundingMesh = BoundingMesh,
			Values = Values,
		};
	}
}
