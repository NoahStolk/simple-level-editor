using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLevelEditor.Formats.Core;
using SimpleLevelEditor.Formats.EntityConfig;
using System.Numerics;

namespace SimpleLevelEditor.Formats.Tests;

[TestClass]
public class EntityConfigDeserializationTests
{
	private static T AssertPropertyType<T>(EntityPropertyTypeDescriptor entityPropertyTypeDescriptor)
		where T : class
	{
		if (entityPropertyTypeDescriptor is T t)
			return t;

		Assert.Fail($"Property type is not {typeof(T).Name}");
		return null;
	}

	[TestMethod]
	public void DeserializeEntityConfig()
	{
		using FileStream fs = File.OpenRead(Path.Combine("Resources", "EntityConfig.json"));
		EntityConfigData? data = SimpleLevelEditorJsonSerializer.DeserializeEntityConfigFromStream(fs);
		Assert.IsNotNull(data);

		Assert.AreEqual(1, data.ModelPaths.Count);
		Assert.AreEqual(2, data.TexturePaths.Count);
		Assert.AreEqual(5, data.Entities.Count);

		EntityDescriptor entity = data.Entities[0];
		Assert.AreEqual("PlayerSpawn", entity.Name);
		Assert.AreEqual(new EntityShapeDescriptor.Point(new PointEntityVisualization.BillboardSprite("PlayerIcon.tga", 32)), entity.Shape);
		Assert.AreEqual(0, entity.Properties.Count);

		entity = data.Entities[1];
		Assert.AreEqual("Light", entity.Name);
		Assert.AreEqual(new EntityShapeDescriptor.Point(new PointEntityVisualization.SimpleSphere(new Rgb(255, 127, 255), 0.25f)), entity.Shape);
		Assert.AreEqual(3, entity.Properties.Count);

		EntityPropertyDescriptor property = entity.Properties[0];
		Assert.AreEqual("Color", property.Name);
		Assert.AreEqual("Point light color", property.Description);
		EntityPropertyTypeDescriptor.RgbProperty rgbPropertyType = AssertPropertyType<EntityPropertyTypeDescriptor.RgbProperty>(property.Type);
		Assert.AreEqual(255, rgbPropertyType.Default.R);
		Assert.AreEqual(255, rgbPropertyType.Default.G);
		Assert.AreEqual(255, rgbPropertyType.Default.B);

		property = entity.Properties[1];
		Assert.AreEqual("Radius", property.Name);
		Assert.AreEqual("Point light radius", property.Description);
		EntityPropertyTypeDescriptor.FloatProperty floatPropertyType = AssertPropertyType<EntityPropertyTypeDescriptor.FloatProperty>(property.Type);
		Assert.AreEqual(5, floatPropertyType.Default);
		Assert.AreEqual(1, floatPropertyType.Step);
		Assert.AreEqual(1, floatPropertyType.Min);
		Assert.AreEqual(1000, floatPropertyType.Max);

		property = entity.Properties[2];
		Assert.AreEqual("Shadow", property.Name);
		Assert.AreEqual("Whether the light casts shadows", property.Description);
		EntityPropertyTypeDescriptor.BoolProperty boolPropertyType = AssertPropertyType<EntityPropertyTypeDescriptor.BoolProperty>(property.Type);
		Assert.IsFalse(boolPropertyType.Default);

		entity = data.Entities[2];
		Assert.AreEqual("DynamicObject", entity.Name);
		Assert.AreEqual(new EntityShapeDescriptor.Point(new PointEntityVisualization.Model("Sphere.obj", 0.5f)), entity.Shape);
		Assert.AreEqual(4, entity.Properties.Count);

		property = entity.Properties[0];
		Assert.AreEqual("Mesh", property.Name);
		Assert.AreEqual("File name (without extension) of the mesh to be used", property.Description);
		EntityPropertyTypeDescriptor.StringProperty stringPropertyType = AssertPropertyType<EntityPropertyTypeDescriptor.StringProperty>(property.Type);
		Assert.AreEqual(string.Empty, stringPropertyType.Default);

		property = entity.Properties[1];
		Assert.AreEqual("Texture", property.Name);
		Assert.AreEqual("File name (without extension) of the texture to be used", property.Description);
		stringPropertyType = AssertPropertyType<EntityPropertyTypeDescriptor.StringProperty>(property.Type);
		Assert.AreEqual(string.Empty, stringPropertyType.Default);

		property = entity.Properties[2];
		Assert.AreEqual("Scale", property.Name);
		Assert.AreEqual("The scale of the mesh used", property.Description);
		EntityPropertyTypeDescriptor.Vector3Property vector3PropertyType = AssertPropertyType<EntityPropertyTypeDescriptor.Vector3Property>(property.Type);
		Assert.AreEqual(Vector3.One, vector3PropertyType.Default);
		Assert.AreEqual(0.1f, vector3PropertyType.Step);
		Assert.AreEqual(0.01f, vector3PropertyType.Min);
		Assert.AreEqual(1000, vector3PropertyType.Max);

		property = entity.Properties[3];
		Assert.AreEqual("Mass", property.Name);
		Assert.AreEqual("The mass of the dynamic physics object", property.Description);
		floatPropertyType = AssertPropertyType<EntityPropertyTypeDescriptor.FloatProperty>(property.Type);
		Assert.AreEqual(1, floatPropertyType.Default);
		Assert.AreEqual(0.1f, floatPropertyType.Step);
		Assert.AreEqual(0.01f, floatPropertyType.Min);
		Assert.AreEqual(1000, floatPropertyType.Max);

		entity = data.Entities[3];
		Assert.AreEqual("Sphere", entity.Name);
		Assert.AreEqual(new EntityShapeDescriptor.Sphere(new Rgb(240, 120, 60)), entity.Shape);
		Assert.AreEqual(0, entity.Properties.Count);

		entity = data.Entities[4];
		Assert.AreEqual("Aabb", entity.Name);
		Assert.AreEqual(new EntityShapeDescriptor.Aabb(new Rgb(60, 120, 240)), entity.Shape);
		Assert.AreEqual(0, entity.Properties.Count);
	}
}
