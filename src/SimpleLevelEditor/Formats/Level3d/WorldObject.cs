using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Formats.Level3d.Enums;

namespace SimpleLevelEditor.Formats.Level3d;

public class WorldObject : IBinarySerializable<WorldObject>
{
	public required int MeshId { get; set; }

	public required int TextureId { get; set; }

	public required int BoundingMeshId { get; set; }

	public required Vector3 Scale { get; set; }

	public required Vector3 Rotation { get; set; }

	public required Vector3 Position { get; set; }

	public WorldObjectValues Values { get; set; }

	public static WorldObject Read(BinaryReader br)
	{
		int meshId = br.ReadInt32();
		int textureId = br.ReadInt32();
		int boundingMeshId = br.ReadInt32();
		Vector3 scale = br.ReadVector3();
		Vector3 rotation = br.ReadVector3();
		Vector3 position = br.ReadVector3();
		WorldObjectValues values = (WorldObjectValues)br.ReadByte();
		return new()
		{
			MeshId = meshId,
			TextureId = textureId,
			BoundingMeshId = boundingMeshId,
			Scale = scale,
			Rotation = rotation,
			Position = position,
			Values = values,
		};
	}

	public void Write(BinaryWriter bw)
	{
		bw.Write(MeshId);
		bw.Write(TextureId);
		bw.Write(BoundingMeshId);
		bw.Write(Scale);
		bw.Write(Rotation);
		bw.Write(Position);
		bw.Write((byte)Values);
	}
}
