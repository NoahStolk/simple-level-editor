using SimpleLevelEditor.Formats.Extensions;
using SimpleLevelEditor.Formats.Level.BinaryModel;
using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.Formats.Level.Model.EntityShapes;
using System.Diagnostics;

namespace SimpleLevelEditor.Formats.Level;

public static class LevelBinarySerializer
{
	private const int _version = 1;

	public static void WriteLevel(MemoryStream ms, Level3dData level)
	{
		using BinaryWriter bw = new(ms);

		// Header
		bw.Write(BinaryModelConstants.Header);
		bw.Write7BitEncodedInt(_version);

		// Entity config
		bw.Write(level.EntityConfigPath != null);
		if (level.EntityConfigPath != null)
			bw.Write(level.EntityConfigPath);

		// Sections
		List<Section> sections =
		[
			new(BinaryModelConstants.MeshesSectionId, WriteMeshesSection(level.Meshes)),
			new(BinaryModelConstants.TexturesSectionId, WriteTexturesSection(level.Textures)),
			new(BinaryModelConstants.WorldObjectsSectionId, WriteWorldObjectsSection(level.WorldObjects)),
			new(BinaryModelConstants.EntitiesSectionId, WriteEntitiesSection(level.Entities)),
		];

		bw.Write7BitEncodedInt(sections.Count);
		foreach (Section section in sections)
			section.Write(bw);
	}

	private static byte[] WriteMeshesSection(IReadOnlyCollection<string> meshPaths)
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);
		bw.Write7BitEncodedInt(meshPaths.Count);
		foreach (string path in meshPaths)
			bw.Write(path);

		return ms.ToArray();
	}

	private static byte[] WriteTexturesSection(IReadOnlyCollection<string> texturePaths)
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);
		bw.Write7BitEncodedInt(texturePaths.Count);
		foreach (string path in texturePaths)
			bw.Write(path);

		return ms.ToArray();
	}

	private static byte[] WriteWorldObjectsSection(IReadOnlyCollection<WorldObject> worldObjects)
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);
		bw.Write7BitEncodedInt(worldObjects.Count);
		foreach (WorldObject wo in worldObjects)
		{
			bw.Write(wo.Mesh);
			bw.Write(wo.Texture);
			bw.Write(wo.Position);
			bw.Write(wo.Rotation);
			bw.Write(wo.Scale);
			bw.Write7BitEncodedInt(wo.Flags.Count);
			foreach (string flag in wo.Flags)
				bw.Write(flag);
		}

		return ms.ToArray();
	}

	private static byte[] WriteEntitiesSection(IReadOnlyCollection<Entity> entities)
	{
		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);
		bw.Write7BitEncodedInt(entities.Count);
		foreach (Entity e in entities)
		{
			bw.Write(e.Name);
			bw.Write(e.Position);

			if (e.Shape.Value is Point)
			{
				bw.Write7BitEncodedInt(BinaryModelConstants.EntityShapePoint);
			}
			else if (e.Shape.Value is Sphere sphere)
			{
				bw.Write7BitEncodedInt(BinaryModelConstants.EntityShapeSphere);
				bw.Write(sphere.Radius);
			}
			else if (e.Shape.Value is Aabb aabb)
			{
				bw.Write7BitEncodedInt(BinaryModelConstants.EntityShapeAabb);
				bw.Write(aabb.Min);
				bw.Write(aabb.Max);
			}
			else
			{
				throw new UnreachableException($"Unknown entity shape: {e.Shape.Value}");
			}

			bw.Write7BitEncodedInt(e.Properties.Count);
			foreach (EntityProperty property in e.Properties)
			{
				bw.Write(property.Key);

				switch (property.Value.Value)
				{
					case bool value:
						bw.Write7BitEncodedInt(BinaryModelConstants.EntityPropertyTypeBool);
						bw.Write(value);
						break;
					case int value:
						bw.Write7BitEncodedInt(BinaryModelConstants.EntityPropertyTypeInt);
						bw.Write7BitEncodedInt(value);
						break;
					case float value:
						bw.Write7BitEncodedInt(BinaryModelConstants.EntityPropertyTypeFloat);
						bw.Write(value);
						break;
					case Vector2 value:
						bw.Write7BitEncodedInt(BinaryModelConstants.EntityPropertyTypeVector2);
						bw.Write(value);
						break;
					case Vector3 value:
						bw.Write7BitEncodedInt(BinaryModelConstants.EntityPropertyTypeVector3);
						bw.Write(value);
						break;
					case Vector4 value:
						bw.Write7BitEncodedInt(BinaryModelConstants.EntityPropertyTypeVector4);
						bw.Write(value);
						break;
					case string value:
						bw.Write7BitEncodedInt(BinaryModelConstants.EntityPropertyTypeString);
						bw.Write(value);
						break;
					case Rgb value:
						bw.Write7BitEncodedInt(BinaryModelConstants.EntityPropertyTypeRgb);
						bw.Write(value);
						break;
					case Rgba value:
						bw.Write7BitEncodedInt(BinaryModelConstants.EntityPropertyTypeRgba);
						bw.Write(value);
						break;
					default:
						throw new UnreachableException($"Unknown entity property type: {property.Value.Value.GetType().Name}");
				}
			}
		}

		return ms.ToArray();
	}
}
