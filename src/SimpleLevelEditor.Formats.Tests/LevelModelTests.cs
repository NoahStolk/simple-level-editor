using Microsoft.FSharp.Collections;
using Microsoft.FSharp.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
		Assert.AreEqual(0, defaultLevel.Models.Length);
		Assert.AreEqual(0, defaultLevel.WorldObjects.Length);
		Assert.AreEqual(0, defaultLevel.Entities.Length);
	}

	[TestMethod]
	public void DeepCopy()
	{
		Entity entity = new(0, "test", new Vector3(1, 2, 3), EntityShape.NewSphere(0.5f), ListModule.OfSeq([new EntityProperty("test", EntityPropertyValue.NewFloat(0.5f))]));

		WorldObject worldObject = new(0, "Test.obj", Vector3.One, new Vector3(4, 5, 6), new Vector3(1, 2, 3), ListModule.OfSeq(["flag"]));

		Level3dData level = new(
			FSharpOption<string>.None,
			ListModule.OfSeq(["Test.obj"]),
			ListModule.OfSeq([worldObject]),
			ListModule.OfSeq([entity]));

		Level3dData copy = level.DeepCopy();

		// Test counts.
		Assert.AreEqual(level.Entities.Length, copy.Entities.Length);
		Assert.AreEqual(level.Models.Length, copy.Models.Length);
		Assert.AreEqual(level.WorldObjects.Length, copy.WorldObjects.Length);
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
