using SimpleLevelEditor.Formats.Extensions;
using SimpleLevelEditor.Formats.Level.BinaryModel;
using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.Formats.Level.Model.EntityShapes;

namespace SimpleLevelEditor.Formats.Level;

public static class LevelBinaryDeserializer
{
	private const int _version = 1;

	public static Level3dData ReadLevel(Stream stream)
	{
		stream.Position = 0;

		// Header
		using BinaryReader br = new(stream);
		Span<byte> header = br.ReadBytes(4).AsSpan();
		if (!header.SequenceEqual(BinaryModelConstants.Header))
			throw new InvalidDataException("Invalid header");

		int version = br.Read7BitEncodedInt();
		if (version != _version)
			throw new NotSupportedException("Unsupported version");

		// Entity config
		string? entityConfigPath = null;
		if (br.ReadBoolean())
			entityConfigPath = br.ReadString();

		// Sections
		List<string> meshes = [];
		List<string> textures = [];
		List<WorldObject> worldObjects = [];
		List<Entity> entities = [];

		int sectionCount = br.Read7BitEncodedInt();
		for (int i = 0; i < sectionCount; i++)
		{
			int sectionId = br.Read7BitEncodedInt();
			int sectionLength = br.Read7BitEncodedInt();
			byte[] sectionData = br.ReadBytes(sectionLength);
			switch (sectionId)
			{
				case BinaryModelConstants.MeshesSectionId:
					meshes = ReadStringListSection(sectionData);
					break;
				case BinaryModelConstants.TexturesSectionId:
					textures = ReadStringListSection(sectionData);
					break;
				case BinaryModelConstants.WorldObjectsSectionId:
					worldObjects = ReadWorldObjectsSection(sectionData);
					break;
				case BinaryModelConstants.EntitiesSectionId:
					entities = ReadEntitiesSection(sectionData);
					break;
			}
		}

		return new()
		{
			EntityConfigPath = entityConfigPath,
			Entities = entities,
			Meshes = meshes,
			Textures = textures,
			WorldObjects = worldObjects,
		};
	}

	private static List<string> ReadStringListSection(byte[] data)
	{
		using MemoryStream ms = new(data);
		using BinaryReader br = new(ms);
		int count = br.Read7BitEncodedInt();
		List<string> result = [];
		for (int i = 0; i < count; i++)
			result.Add(br.ReadString());
		return result;
	}

	private static List<WorldObject> ReadWorldObjectsSection(byte[] data)
	{
		using MemoryStream ms = new(data);
		using BinaryReader br = new(ms);
		int count = br.Read7BitEncodedInt();
		List<WorldObject> worldObjects = [];
		for (int i = 0; i < count; i++)
		{
			string mesh = br.ReadString();
			string texture = br.ReadString();
			Vector3 position = br.ReadVector3();
			Vector3 rotation = br.ReadVector3();
			Vector3 scale = br.ReadVector3();
			int flagCount = br.Read7BitEncodedInt();
			List<string> flags = [];
			for (int j = 0; j < flagCount; j++)
				flags.Add(br.ReadString());

			WorldObject wo = new()
			{
				Id = i + 1,
				Mesh = mesh,
				Texture = texture,
				Position = position,
				Rotation = rotation,
				Scale = scale,
				Flags = flags,
			};
			worldObjects.Add(wo);
		}

		return worldObjects;
	}

	private static List<Entity> ReadEntitiesSection(byte[] data)
	{
		using MemoryStream ms = new(data);
		using BinaryReader br = new(ms);
		int count = br.Read7BitEncodedInt();
		List<Entity> entities = [];
		for (int i = 0; i < count; i++)
		{
			string name = br.ReadString();
			Vector3 position = br.ReadVector3();

			int shapeType = br.Read7BitEncodedInt();
			OneOf.OneOf<Point, Sphere, Aabb> shape = shapeType switch
			{
				BinaryModelConstants.EntityShapePoint => new Point(),
				BinaryModelConstants.EntityShapeSphere => new Sphere(br.ReadSingle()),
				BinaryModelConstants.EntityShapeAabb => new Aabb(br.ReadVector3(), br.ReadVector3()),
				_ => throw new NotSupportedException($"Unsupported shape type: {shapeType}"),
			};

			int propertyCount = br.Read7BitEncodedInt();
			List<EntityProperty> properties = [];
			for (int j = 0; j < propertyCount; j++)
			{
				string key = br.ReadString();
				int propertyType = br.Read7BitEncodedInt();
				OneOf.OneOf<bool, int, float, Vector2, Vector3, Vector4, string, Rgb, Rgba> value = propertyType switch
				{
					BinaryModelConstants.EntityPropertyTypeBool => br.ReadBoolean(),
					BinaryModelConstants.EntityPropertyTypeInt => br.Read7BitEncodedInt(),
					BinaryModelConstants.EntityPropertyTypeFloat => br.ReadSingle(),
					BinaryModelConstants.EntityPropertyTypeVector2 => br.ReadVector2(),
					BinaryModelConstants.EntityPropertyTypeVector3 => br.ReadVector3(),
					BinaryModelConstants.EntityPropertyTypeVector4 => br.ReadVector4(),
					BinaryModelConstants.EntityPropertyTypeString => br.ReadString(),
					BinaryModelConstants.EntityPropertyTypeRgb => br.ReadRgb(),
					BinaryModelConstants.EntityPropertyTypeRgba => br.ReadRgba(),
					_ => throw new NotSupportedException($"Unsupported property type: {propertyType}"),
				};
				properties.Add(new()
				{
					Key = key,
					Value = value,
				});
			}

			Entity entity = new()
			{
				Id = i + 1,
				Name = name,
				Position = position,
				Shape = shape,
				Properties = properties,
			};
			entities.Add(entity);
		}

		return entities;
	}
}
