namespace SimpleLevelEditor.Formats.Level3d;

public class Level3dData : IBinarySerializable<Level3dData>
{
	public Level3dData(int version, List<string> meshes, List<string> textures, List<WorldObject> worldObjects, List<Entity> entities)
	{
		Version = version;
		Meshes = meshes;
		Textures = textures;
		WorldObjects = worldObjects;
		Entities = entities;
	}

	private static ReadOnlySpan<byte> Magic => "SLELVL3D"u8;

	public int Version { get; set; }

	public List<string> Meshes { get; set; }

	public List<string> Textures { get; set; }

	public List<WorldObject> WorldObjects { get; set; }

	public List<Entity> Entities { get; set; }

	public static Level3dData Default => new(1, new(), new(), new(), new());

	public static Level3dData Read(BinaryReader br)
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
			worldObjects.Add(WorldObject.Read(br));

		int entityCount = br.ReadInt32();
		List<Entity> entities = new();
		for (int i = 0; i < entityCount; i++)
			entities.Add(Entity.Read(br));

		return new(version, meshes, textures, worldObjects, entities);
	}

	public void Write(BinaryWriter bw)
	{
		bw.Write(Magic);
		bw.Write(Version);

		bw.Write(Meshes.Count);
		foreach (string mesh in Meshes)
			bw.Write(mesh);

		bw.Write(Textures.Count);
		foreach (string texture in Textures)
			bw.Write(texture);

		bw.Write(WorldObjects.Count);
		foreach (WorldObject worldObject in WorldObjects)
			worldObject.Write(bw);

		bw.Write(Entities.Count);
		foreach (Entity entity in Entities)
			entity.Write(bw);
	}

	public Level3dData DeepCopy()
	{
		return new(Version, Meshes, Textures, WorldObjects, Entities);
	}
}
