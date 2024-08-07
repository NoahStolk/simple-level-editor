using Microsoft.FSharp.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLevelEditor.Formats.Types;
using SimpleLevelEditor.Formats.Types.EntityConfig;

namespace SimpleLevelEditor.Formats.Tests;

[TestClass]
public class EntityConfigModelTests
{
	[TestMethod]
	public void CreateDefault()
	{
		EntityConfigData defaultEntityConfig = EntityConfigData.CreateDefault();
		Assert.AreEqual(0, defaultEntityConfig.ModelPaths.Length);
		Assert.AreEqual(0, defaultEntityConfig.TexturePaths.Length);
		Assert.AreEqual(0, defaultEntityConfig.Entities.Length);
	}

	[TestMethod]
	public void DeepCopy()
	{
		EntityDescriptor entity = new(
			"TestEntity",
			EntityShapeDescriptor.NewSphere(new Rgb(255, 0, 127)),
			ListModule.OfSeq([new EntityPropertyDescriptor("TestProperty", EntityPropertyTypeDescriptor.NewFloatProperty(10, 1, 1000, 0.5f), "Test description")]));

		EntityConfigData entityConfig = new(
			ListModule.OfSeq(["Test.obj"]),
			ListModule.OfSeq(["Test.png"]),
			ListModule.OfSeq([entity]));

		EntityConfigData copy = entityConfig.DeepCopy();

		// Test counts.
		Assert.AreEqual(entityConfig.Entities.Length, copy.Entities.Length);
		Assert.AreEqual(entityConfig.ModelPaths.Length, copy.ModelPaths.Length);
		Assert.AreEqual(entityConfig.TexturePaths.Length, copy.TexturePaths.Length);

		// Test lists.
		Assert.AreNotSame(entityConfig.Entities, copy.Entities);
		Assert.AreNotSame(entityConfig.ModelPaths, copy.ModelPaths);
		Assert.AreNotSame(entityConfig.TexturePaths, copy.TexturePaths);

		// Test list contents.
		Assert.AreEqual(entityConfig.Entities[0], copy.Entities[0]);
		Assert.AreEqual(entityConfig.ModelPaths[0], copy.ModelPaths[0]);
		Assert.AreEqual(entityConfig.TexturePaths[0], copy.TexturePaths[0]);

		// Test entity references.
		Assert.AreSame(entity, entityConfig.Entities[0]);
		Assert.AreNotSame(entity, copy.Entities[0]);

		// Test entity property references.
		Assert.AreSame(entity.Properties[0], entityConfig.Entities[0].Properties[0]);
		Assert.AreNotSame(entity.Properties[0], copy.Entities[0].Properties[0]);

		Assert.AreSame(entity.Properties[0].Type, entityConfig.Entities[0].Properties[0].Type);
		Assert.AreNotSame(entity.Properties[0].Type, copy.Entities[0].Properties[0].Type);

		// Test entity property values.
		Assert.AreEqual(entity.Properties[0].Name, entityConfig.Entities[0].Properties[0].Name);
		Assert.AreEqual(entity.Properties[0].Type, entityConfig.Entities[0].Properties[0].Type);
		Assert.AreEqual(entity.Properties[0].Description, copy.Entities[0].Properties[0].Description);
	}
}
