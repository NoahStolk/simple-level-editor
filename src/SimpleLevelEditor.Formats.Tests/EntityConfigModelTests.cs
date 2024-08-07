using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLevelEditor.Formats.Core;
using SimpleLevelEditor.Formats.EntityConfig;

namespace SimpleLevelEditor.Formats.Tests;

[TestClass]
public class EntityConfigModelTests
{
	[TestMethod]
	public void CreateDefault()
	{
		EntityConfigData defaultEntityConfig = EntityConfigData.CreateDefault();
		Assert.AreEqual(0, defaultEntityConfig.ModelPaths.Count);
		Assert.AreEqual(0, defaultEntityConfig.TexturePaths.Count);
		Assert.AreEqual(0, defaultEntityConfig.Entities.Count);
	}

	[TestMethod]
	public void DeepCopy()
	{
		EntityDescriptor entity = new(
			"TestEntity",
			new EntityShapeDescriptor.Sphere(new Rgb(255, 0, 127)),
			[new EntityPropertyDescriptor("TestProperty", new EntityPropertyTypeDescriptor.FloatProperty(10, 1, 1000, 0.5f), "Test description")]);

		EntityConfigData entityConfig = new(
			["Test.obj"],
			["Test.png"],
			[entity]);

		EntityConfigData copy = entityConfig.DeepCopy();

		// Test counts.
		Assert.AreEqual(entityConfig.Entities.Count, copy.Entities.Count);
		Assert.AreEqual(entityConfig.ModelPaths.Count, copy.ModelPaths.Count);
		Assert.AreEqual(entityConfig.TexturePaths.Count, copy.TexturePaths.Count);

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
