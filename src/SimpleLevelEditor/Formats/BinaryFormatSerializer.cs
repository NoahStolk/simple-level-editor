using OneOf;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Model.EntityTypes;
using SimpleLevelEditor.Model.Enums;

namespace SimpleLevelEditor.Formats;

public static class BinaryFormatSerializer
{
	private static ReadOnlySpan<byte> Magic => "SLELVL3D"u8;

	public static Level3dData ReadLevel(BinaryReader br)
	{
		byte[] magic = br.ReadBytes(8);
		if (!Magic.SequenceEqual(magic))
			throw new InvalidDataException("Invalid magic.");

		int version = br.ReadInt32();

		int meshCount = br.ReadInt32();
		List<string> meshes = new();
		for (int i = 0; i < meshCount; i++)
			meshes.Add(br.ReadString());

		int textureCount = br.ReadInt32();
		List<string> textures = new();
		for (int i = 0; i < textureCount; i++)
			textures.Add(br.ReadString());

		int worldObjectCount = br.ReadInt32();
		List<WorldObject> worldObjects = new();
		for (int i = 0; i < worldObjectCount; i++)
			worldObjects.Add(ReadWorldObject(br));

		int entityCount = br.ReadInt32();
		List<Entity> entities = new();
		for (int i = 0; i < entityCount; i++)
			entities.Add(ReadEntity(br));

		return new()
		{
			Version = version,
			Meshes = meshes,
			Textures = textures,
			WorldObjects = worldObjects,
			Entities = entities,
		};
	}

	private static WorldObject ReadWorldObject(BinaryReader br)
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

	private static Entity ReadEntity(BinaryReader br)
	{
		string name = br.ReadString();

		EntityType shapeType = (EntityType)br.ReadByte();
		OneOf<Point, Sphere, Aabb, StandingCylinder> shape = shapeType switch
		{
			EntityType.Point => new Point(br.ReadVector3()),
			EntityType.Sphere => new Sphere(br.ReadVector3(), br.ReadSingle()),
			EntityType.Aabb => new Aabb(br.ReadVector3(), br.ReadVector3()),
			EntityType.StandingCylinder => new StandingCylinder(br.ReadVector3(), br.ReadSingle(), br.ReadSingle()),
			_ => throw new InvalidDataException("Invalid shape type."),
		};

		ushort propertyCount = br.ReadUInt16();
		List<EntityProperty> properties = new();
		for (int j = 0; j < propertyCount; j++)
			properties.Add(ReadEntityProperty(br));

		return new()
		{
			Name = name,
			Shape = shape,
			Properties = properties,
		};
	}

	private static EntityProperty ReadEntityProperty(BinaryReader br)
	{
		string key = br.ReadString();
		PropertyValueType propertyType = (PropertyValueType)br.ReadByte();
		return new()
		{
			Key = key,
			Value = propertyType switch
			{
				PropertyValueType.Boolean => br.ReadBoolean(),
				PropertyValueType.UInt8 => br.ReadByte(),
				PropertyValueType.UInt16 => br.ReadUInt16(),
				PropertyValueType.Int32 => br.ReadInt32(),
				PropertyValueType.Float32 => br.ReadSingle(),
				PropertyValueType.Vector2Float32 => br.ReadVector2(),
				PropertyValueType.Vector3Float32 => br.ReadVector3(),
				PropertyValueType.Vector4Float32 => br.ReadVector4(),
				PropertyValueType.QuaternionFloat32 => br.ReadQuaternion(),
				PropertyValueType.String => br.ReadString(),
				_ => throw new InvalidDataException("Invalid property type."),
			},
		};
	}

	public static void WriteLevel(Level3dData level, BinaryWriter bw)
	{
		bw.Write(Magic);
		bw.Write(level.Version);

		bw.Write(level.Meshes.Count);
		foreach (string mesh in level.Meshes)
			bw.Write(mesh);

		bw.Write(level.Textures.Count);
		foreach (string texture in level.Textures)
			bw.Write(texture);

		bw.Write(level.WorldObjects.Count);
		foreach (WorldObject worldObject in level.WorldObjects)
			WriteWorldObject(worldObject, bw);

		bw.Write(level.Entities.Count);
		foreach (Entity entity in level.Entities)
			WriteEntity(entity, bw);
	}

	private static void WriteWorldObject(WorldObject worldObject, BinaryWriter bw)
	{
		bw.Write(worldObject.MeshId);
		bw.Write(worldObject.TextureId);
		bw.Write(worldObject.BoundingMeshId);
		bw.Write(worldObject.Scale);
		bw.Write(worldObject.Rotation);
		bw.Write(worldObject.Position);
		bw.Write((byte)worldObject.Values);
	}

	private static void WriteEntity(Entity entity, BinaryWriter bw)
	{
		bw.Write(entity.Name);
		switch (entity.Shape.Value)
		{
			case Point p:
				bw.Write((byte)EntityType.Point);
				bw.Write(p.Position);
				break;
			case Sphere s:
				bw.Write((byte)EntityType.Sphere);
				bw.Write(s.Position);
				bw.Write(s.Radius);
				break;
			case Aabb a:
				bw.Write((byte)EntityType.Aabb);
				bw.Write(a.Min);
				bw.Write(a.Max);
				break;
			case StandingCylinder sc:
				bw.Write((byte)EntityType.StandingCylinder);
				bw.Write(sc.Position);
				bw.Write(sc.Radius);
				bw.Write(sc.Height);
				break;
		}

		bw.Write((ushort)entity.Properties.Count);
		foreach (EntityProperty property in entity.Properties)
			WriteEntityProperty(property, bw);
	}

	private static void WriteEntityProperty(EntityProperty entityProperty, BinaryWriter bw)
	{
		bw.Write(entityProperty.Key);

		switch (entityProperty.Value.Value)
		{
			case bool b:
				bw.Write((byte)PropertyValueType.Boolean);
				bw.Write(b);
				break;
			case byte u8:
				bw.Write((byte)PropertyValueType.UInt8);
				bw.Write(u8);
				break;
			case ushort u16:
				bw.Write((byte)PropertyValueType.UInt16);
				bw.Write(u16);
				break;
			case int i32:
				bw.Write((byte)PropertyValueType.Int32);
				bw.Write(i32);
				break;
			case float f32:
				bw.Write((byte)PropertyValueType.Float32);
				bw.Write(f32);
				break;
			case Vector2 v2:
				bw.Write((byte)PropertyValueType.Vector2Float32);
				bw.Write(v2);
				break;
			case Vector3 v3:
				bw.Write((byte)PropertyValueType.Vector3Float32);
				bw.Write(v3);
				break;
			case Vector4 v4:
				bw.Write((byte)PropertyValueType.Vector4Float32);
				bw.Write(v4);
				break;
			case Quaternion q:
				bw.Write((byte)PropertyValueType.QuaternionFloat32);
				bw.Write(q);
				break;
			case string s:
				bw.Write((byte)PropertyValueType.String);
				bw.Write(s);
				break;
		}
	}
}
