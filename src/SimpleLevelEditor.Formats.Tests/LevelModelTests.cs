using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLevelEditor.Formats.Level;
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
		Assert.AreEqual(0, defaultLevel.ModelPaths.Count);
		Assert.AreEqual(0, defaultLevel.WorldObjects.Count);
		Assert.AreEqual(0, defaultLevel.Entities.Count);
	}

	[TestMethod]
	public void DeepCopy()
	{
		Entity entity = new()
		{
			Id = 0,
			Name = "test",
			Position = new Vector3(1, 2, 3),
			Shape = new EntityShape.Sphere(0.5f),
			Properties = [
				new EntityProperty
				{
					Key = "test",
					Value = new EntityPropertyValue.Float(0.5f),
				},
			],
		};

		WorldObject worldObject = new()
		{
			Id = 0,
			ModelPath = "Test.obj",
			Scale = Vector3.One,
			Rotation = new Vector3(4, 5, 6),
			Position = new Vector3(1, 2, 3),
			Flags = ["flag"],
		};

		Level3dData level = new()
		{
			EntityConfigPath = null,
			ModelPaths = ["Test.obj"],
			WorldObjects = [worldObject],
			Entities = [entity],
		};

		Level3dData copy = level.DeepCopy();

		// Test counts.
		Assert.AreEqual(level.Entities.Count, copy.Entities.Count);
		Assert.AreEqual(level.ModelPaths.Count, copy.ModelPaths.Count);
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

		// Test world object model references. Strings are immutable, so it doesn't matter that they're technically not deep copied.
		Assert.AreEqual(worldObject.ModelPath, level.WorldObjects[0].ModelPath);
		Assert.AreEqual(worldObject.ModelPath, copy.WorldObjects[0].ModelPath);
	}
}
