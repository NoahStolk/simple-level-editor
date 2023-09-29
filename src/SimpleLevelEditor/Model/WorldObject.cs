using SimpleLevelEditor.Model.Enums;

namespace SimpleLevelEditor.Model;

public class WorldObject
{
	public required string Mesh { get; set; }

	public required string Texture { get; set; }

	public required string BoundingMesh { get; set; }

	public required Vector3 Scale { get; set; }

	public required Vector3 Rotation { get; set; }

	public required Vector3 Position { get; set; }

	public WorldObjectValues Values { get; set; }
}
