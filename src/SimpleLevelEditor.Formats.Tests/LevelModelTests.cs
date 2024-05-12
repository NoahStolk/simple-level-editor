using Microsoft.FSharp.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.Formats.Types.Level;
using System.Numerics;

namespace SimpleLevelEditor.Formats.Tests;

[TestClass]
public class LevelModelTests
{
	[TestMethod]
	public void CreateDefault()
	{
		Level3dData defaultLevel = Level3dData.CreateDefault();
		Assert.IsNull(defaultLevel.EntityConfigPath);
		Assert.AreEqual(0, defaultLevel.Meshes.Count);
		Assert.AreEqual(0, defaultLevel.Textures.Count);
		Assert.AreEqual(0, defaultLevel.WorldObjects.Count);
		Assert.AreEqual(0, defaultLevel.Entities.Count);
	}

	[TestMethod]
	public void DeepCopy()
	{
		Entity entity = new(0, "test", new Vector3(1, 2, 3), ShapeDescriptor.NewSphere(0.5f), ListModule.OfSeq([new EntityProperty("test", EntityPropertyValue.NewFloat(0.5f))]));

		WorldObject worldObject = new(0, "Test.obj", "Test.tga", Vector3.One, new Vector3(4, 5, 6), new Vector3(1, 2, 3), ListModule.OfSeq(["flag"]));

		Level3dData level = new()
		{
			Entities = [entity],
			Meshes = ["Test.obj"],
			Textures = ["Test.tga"],
			WorldObjects = [worldObject],
			EntityConfigPath = null,
		};

		Level3dData copy = level.DeepCopy();

		// Test counts.
		Assert.AreEqual(level.Entities.Count, copy.Entities.Count);
		Assert.AreEqual(level.Meshes.Count, copy.Meshes.Count);
		Assert.AreEqual(level.Textures.Count, copy.Textures.Count);
		Assert.AreEqual(level.WorldObjects.Count, copy.WorldObjects.Count);
		Assert.AreEqual(level.EntityConfigPath, copy.EntityConfigPath);

		// Test entity and world object references.
		Assert.AreSame(entity, level.Entities[0]);
		Assert.AreNotSame(entity, copy.Entities[0]);

		Assert.AreSame(worldObject, level.WorldObjects[0]);
		Assert.AreNotSame(worldObject, copy.WorldObjects[0]);

		// Test entity property references.
		Assert.AreSame(entity.Properties[0], level.Entities[0].Properties[0]);
		Assert.AreNotSame(entity.Properties[0], copy.Entities[0].Properties[0]);

		Assert.AreSame(entity.Properties[0].Value, level.Entities[0].Properties[0].Value);
		Assert.AreNotSame(entity.Properties[0].Value, copy.Entities[0].Properties[0].Value);

		// Test entity property values.
		Assert.AreEqual(entity.Properties[0].Key, level.Entities[0].Properties[0].Key);
		Assert.AreEqual(entity.Properties[0].Value, level.Entities[0].Properties[0].Value);
		Assert.AreEqual(entity.Properties[0].Key, copy.Entities[0].Properties[0].Key);
		Assert.AreEqual(entity.Properties[0].Value, copy.Entities[0].Properties[0].Value);

		// Test entity shape references.
		Assert.AreSame(entity.Shape, level.Entities[0].Shape);
		Assert.AreNotSame(entity.Shape, copy.Entities[0].Shape);

		// Test world object flags.
		Assert.AreSame(worldObject.Flags, level.WorldObjects[0].Flags);
		Assert.AreNotSame(worldObject.Flags, copy.WorldObjects[0].Flags);

		Assert.AreEqual(worldObject.Flags[0], level.WorldObjects[0].Flags[0]);
		Assert.AreEqual(worldObject.Flags[0], copy.WorldObjects[0].Flags[0]);

		// Test world object mesh and texture references. Strings are immutable, so it doesn't matter that they're technically not deep copied.
		Assert.AreEqual(worldObject.Mesh, level.WorldObjects[0].Mesh);
		Assert.AreEqual(worldObject.Mesh, copy.WorldObjects[0].Mesh);

		Assert.AreEqual(worldObject.Texture, level.WorldObjects[0].Texture);
		Assert.AreEqual(worldObject.Texture, copy.WorldObjects[0].Texture);
	}
}
