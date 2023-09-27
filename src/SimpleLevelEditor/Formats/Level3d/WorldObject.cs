using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Formats.Level3d.Enums;

namespace SimpleLevelEditor.Formats.Level3d;

public class WorldObject : IBinarySerializable<WorldObject>
{
	public WorldObject(int meshId, int textureId, int boundingMeshId, Vector3 scale, Vector3 rotation, Vector3 position, WorldObjectValues values)
	{
		MeshId = meshId;
		TextureId = textureId;
		BoundingMeshId = boundingMeshId;
		Scale = scale;
		Rotation = rotation;
		Position = position;
		Values = values;
	}

	public int MeshId { get; set; }

	public int TextureId { get; set; }

	public int BoundingMeshId { get; set; }

	public Vector3 Scale { get; set; }

	public Vector3 Rotation { get; set; }

	public Vector3 Position { get; set; }

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
		return new(meshId, textureId, boundingMeshId, scale, rotation, position, values);
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
