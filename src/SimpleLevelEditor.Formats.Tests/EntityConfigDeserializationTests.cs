using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLevelEditor.Formats.EntityConfig;
using SimpleLevelEditor.Formats.EntityConfig.Model;
using SimpleLevelEditor.Formats.Types;
using SimpleLevelEditor.Formats.Types.EntityConfig;
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
		using FileStream fs = File.OpenRead(Path.Combine("Resources", "EntityConfig.xml"));
		EntityConfigData data = EntityConfigXmlDeserializer.ReadEntityConfig(fs);

		Assert.AreEqual(1, data.Version);
		Assert.AreEqual(3, data.Entities.Count);

		EntityDescriptor entity = data.Entities[0];
		Assert.AreEqual("PlayerSpawn", entity.Name);
		Assert.AreEqual(EntityShape.NewPoint(PointEntityVisualization.NewBillboardSprite("PlayerIcon", 32)), entity.Shape);
		Assert.AreEqual(0, entity.Properties.Count);

		entity = data.Entities[1];
		Assert.AreEqual("Light", entity.Name);
		Assert.AreEqual(EntityShape.NewPoint(PointEntityVisualization.NewSimpleSphere(new Rgb(255, 127, 255), 0.25f)), entity.Shape);
		Assert.AreEqual(3, entity.Properties.Count);

		EntityPropertyDescriptor property = entity.Properties[0];
		Assert.AreEqual("Color", property.Name);
		Assert.AreEqual("Point light color", property.Description);
		EntityPropertyTypeDescriptor.RgbProperty rgbPropertyType = AssertPropertyType<EntityPropertyTypeDescriptor.RgbProperty>(property.Type);
		Assert.AreEqual(255, rgbPropertyType.DefaultValue.R);
		Assert.AreEqual(255, rgbPropertyType.DefaultValue.G);
		Assert.AreEqual(255, rgbPropertyType.DefaultValue.B);

		property = entity.Properties[1];
		Assert.AreEqual("Radius", property.Name);
		Assert.AreEqual("Point light radius", property.Description);
		EntityPropertyTypeDescriptor.FloatProperty floatPropertyType = AssertPropertyType<EntityPropertyTypeDescriptor.FloatProperty>(property.Type);
		Assert.AreEqual(5, floatPropertyType.DefaultValue);
		Assert.AreEqual(1, floatPropertyType.Step);
		Assert.AreEqual(1, floatPropertyType.MinValue);
		Assert.AreEqual(1000, floatPropertyType.MaxValue);

		property = entity.Properties[2];
		Assert.AreEqual("Shadow", property.Name);
		Assert.AreEqual("Whether the light casts shadows", property.Description);
		EntityPropertyTypeDescriptor.BoolProperty boolPropertyType = AssertPropertyType<EntityPropertyTypeDescriptor.BoolProperty>(property.Type);
		Assert.IsFalse(boolPropertyType.DefaultValue);

		entity = data.Entities[2];
		Assert.AreEqual("DynamicObject", entity.Name);
		Assert.AreEqual(EntityShape.NewPoint(PointEntityVisualization.NewMesh("Sphere", "Checkerboard", 0.5f)), entity.Shape);
		Assert.AreEqual(4, entity.Properties.Count);

		property = entity.Properties[0];
		Assert.AreEqual("Mesh", property.Name);
		Assert.AreEqual("File name (without extension) of the mesh to be used", property.Description);
		EntityPropertyTypeDescriptor.StringProperty stringPropertyType = AssertPropertyType<EntityPropertyTypeDescriptor.StringProperty>(property.Type);
		Assert.AreEqual(string.Empty, stringPropertyType.DefaultValue);

		property = entity.Properties[1];
		Assert.AreEqual("Texture", property.Name);
		Assert.AreEqual("File name (without extension) of the texture to be used", property.Description);
		stringPropertyType = AssertPropertyType<EntityPropertyTypeDescriptor.StringProperty>(property.Type);
		Assert.AreEqual(string.Empty, stringPropertyType.DefaultValue);

		property = entity.Properties[2];
		Assert.AreEqual("Scale", property.Name);
		Assert.AreEqual("The scale of the mesh used", property.Description);
		EntityPropertyTypeDescriptor.Vector3Property vector3PropertyType = AssertPropertyType<EntityPropertyTypeDescriptor.Vector3Property>(property.Type);
		Assert.AreEqual(Vector3.One, vector3PropertyType.DefaultValue);
		Assert.AreEqual(0.1f, vector3PropertyType.Step);
		Assert.AreEqual(0.01f, vector3PropertyType.MinValue);
		Assert.AreEqual(1000, vector3PropertyType.MaxValue);

		property = entity.Properties[3];
		Assert.AreEqual("Mass", property.Name);
		Assert.AreEqual("The mass of the dynamic physics object", property.Description);
		floatPropertyType = AssertPropertyType<EntityPropertyTypeDescriptor.FloatProperty>(property.Type);
		Assert.AreEqual(1, floatPropertyType.DefaultValue);
		Assert.AreEqual(0.1f, floatPropertyType.Step);
		Assert.AreEqual(0.01f, floatPropertyType.MinValue);
		Assert.AreEqual(1000, floatPropertyType.MaxValue);
	}
}
