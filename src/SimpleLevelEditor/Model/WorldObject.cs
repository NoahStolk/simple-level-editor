using SimpleLevelEditor.Model.Enums;

namespace SimpleLevelEditor.Model;

public class WorldObject
{
	public required int MeshId { get; set; }

	public required int TextureId { get; set; }

	public required int BoundingMeshId { get; set; }

	public required Vector3 Scale { get; set; }

	public required Vector3 Rotation { get; set; }

	public required Vector3 Position { get; set; }

	public WorldObjectValues Values { get; set; }
}
