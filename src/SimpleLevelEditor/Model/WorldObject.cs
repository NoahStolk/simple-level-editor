using SimpleLevelEditor.Model.Enums;

namespace SimpleLevelEditor.Model;

public record WorldObject
{
	public required string Mesh { get; set; }

	public required string Texture { get; set; }

	public required string BoundingMesh { get; set; }

	public required Vector3 Scale { get; set; }

	public required Vector3 Rotation { get; set; }

	public required Vector3 Position { get; set; }

	public required WorldObjectValues Values { get; set; }

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
