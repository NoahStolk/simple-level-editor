using Format.GameEntityConfig;
using Format.GameEntityConfig.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GameEntityConfig.Tests;

[TestClass]
public class GameEntityConfigTests
{
	private static readonly DataType _health = new("Health", [new DataTypeField("Value", Primitive.U32)]);
	private static readonly DataType _radius = new("Radius", [new DataTypeField("Value", Primitive.F32)]);

	[TestMethod]
	public void SerDes()
	{
		GameEntityConfigBuilder builder = new();
		Format.GameEntityConfig.Model.GameEntityConfig config = builder
			.WithDefaultDataTypes()
			.WithDataType(_health)
			.WithDataType(_radius)
			.WithModelPath("Player.obj")
			.WithTexturePath("Audio.png")
			.WithEntityDescriptor(CreatePlayer())
			.WithEntityDescriptor(CreateLight())
			.WithEntityDescriptor(WorldObject())
			.Build();

		string json = GameEntityConfigSerializer.Serialize(config);
		Format.GameEntityConfig.Model.GameEntityConfig deserializedConfig = GameEntityConfigSerializer.Deserialize(json);

		Assert.AreEqual(config.ModelPaths.Count, deserializedConfig.ModelPaths.Count);
		for (int i = 0; i < config.ModelPaths.Count; i++)
			Assert.AreEqual(config.ModelPaths[i], deserializedConfig.ModelPaths[i]);

		Assert.AreEqual(config.TexturePaths.Count, deserializedConfig.TexturePaths.Count);
		for (int i = 0; i < config.TexturePaths.Count; i++)
			Assert.AreEqual(config.TexturePaths[i], deserializedConfig.TexturePaths[i]);

		Assert.AreEqual(config.DataTypes.Count, deserializedConfig.DataTypes.Count);
		for (int i = 0; i < config.DataTypes.Count; i++)
			Assert.AreEqual(config.DataTypes[i].Name, deserializedConfig.DataTypes[i].Name);

		Assert.AreEqual(config.EntityDescriptors.Count, deserializedConfig.EntityDescriptors.Count);
		for (int i = 0; i < config.EntityDescriptors.Count; i++)
			Assert.AreEqual(config.EntityDescriptors[i].Name, deserializedConfig.EntityDescriptors[i].Name);
	}

	[TestMethod]
	public void FailOnIncorrectOrder()
	{
		GameEntityConfigBuilder builder = new();
		Assert.ThrowsException<ArgumentException>(() =>
		{
			builder
				.WithDefaultDataTypes()
				.WithEntityDescriptor(CreatePlayer())
				.WithDataType(_health)
				.Build();
		});
	}

	[TestMethod]
	public void FailOnMissingComponentTypes()
	{
		GameEntityConfigBuilder builder = new();
		Assert.ThrowsException<ArgumentException>(() =>
		{
			builder
				.WithEntityDescriptor(CreatePlayer())
				.Build();
		});
	}

	[TestMethod]
	public void FailOnDuplicateComponentTypes()
	{
		GameEntityConfigBuilder builder = new();
		Assert.ThrowsException<ArgumentException>(() =>
		{
			builder
				.WithDefaultDataTypes()
				.WithDataType(_health)
				.WithDataType(_radius)
				.WithDataType(_health)
				.Build();
		});
	}

	[TestMethod]
	public void FailOnDuplicateModelPaths()
	{
		GameEntityConfigBuilder builder = new();
		Assert.ThrowsException<ArgumentException>(() =>
		{
			builder
				.WithModelPath("Player.obj")
				.WithModelPath("Player.obj")
				.Build();
		});
	}

	[TestMethod]
	public void FailOnDuplicateTexturePaths()
	{
		GameEntityConfigBuilder builder = new();
		Assert.ThrowsException<ArgumentException>(() =>
		{
			builder
				.WithTexturePath("Audio.png")
				.WithTexturePath("Audio.png")
				.Build();
		});
	}

	[TestMethod]
	public void BuildGameEntityConfig()
	{
		GameEntityConfigBuilder builder = new();
		Format.GameEntityConfig.Model.GameEntityConfig config = builder
			.WithDefaultDataTypes()
			.WithDataType(_health)
			.WithDataType(_radius)
			.WithModelPath("Player.obj")
			.WithTexturePath("Audio.png")
			.WithEntityDescriptor(CreatePlayer())
			.WithEntityDescriptor(CreateLight())
			.WithEntityDescriptor(WorldObject())
			.Build();

		Assert.AreEqual(1, config.ModelPaths.Count);
		Assert.AreEqual("Player.obj", config.ModelPaths[0]);
		Assert.AreEqual(1, config.TexturePaths.Count);
		Assert.AreEqual("Audio.png", config.TexturePaths[0]);
		Assert.AreEqual(9, config.DataTypes.Count);
		Assert.AreEqual("DiffuseColor", config.DataTypes[0].Name);
		Assert.AreEqual("Position", config.DataTypes[1].Name);
		Assert.AreEqual("Rotation", config.DataTypes[2].Name);
		Assert.AreEqual("Scale", config.DataTypes[3].Name);
		Assert.AreEqual("Model", config.DataTypes[4].Name);
		Assert.AreEqual("Billboard", config.DataTypes[5].Name);
		Assert.AreEqual("Wireframe", config.DataTypes[6].Name);
		Assert.AreEqual("Health", config.DataTypes[7].Name);
		Assert.AreEqual("Radius", config.DataTypes[8].Name);
		Assert.AreEqual(3, config.EntityDescriptors.Count);
		Assert.AreEqual("Player", config.EntityDescriptors[0].Name);
		Assert.AreEqual("Light", config.EntityDescriptors[1].Name);
		Assert.AreEqual("WorldObject", config.EntityDescriptors[2].Name);
	}

	private static EntityDescriptor CreatePlayer()
	{
		EntityDescriptorBuilder builder = new();
		return builder
			.WithName("Player")
			.WithFixedComponent(DataType.Scale, "1;1;1")
			.WithFixedComponent(DataType.DiffuseColor, "0;255;90;255")
			.WithFixedComponent(_health, "100")
			.WithFixedComponent(DataType.Model, "Player.obj")
			.WithVaryingComponent(DataType.Position, "0;0;0", 0.1f, -100, 100)
			.Build();
	}

	private static EntityDescriptor CreateLight()
	{
		EntityDescriptorBuilder builder = new();
		return builder
			.WithName("Light")
			.WithFixedComponent(DataType.Scale, "1;1;1")
			.WithFixedComponent(DataType.DiffuseColor, "255;255;255;255")
			.WithFixedComponent(DataType.Billboard, "Light.png")
			.WithVaryingComponent(DataType.Position, "0;0;0", 0.1f, -100, 100)
			.WithVaryingComponent(_health, "10", 0.1f, 0, 100)
			.Build();
	}

	private static EntityDescriptor WorldObject()
	{
		EntityDescriptorBuilder builder = new();
		return builder
			.WithName("WorldObject")
			.WithVaryingComponent(DataType.Position, "0;0;0", 0.1f, -100, 100)
			.WithVaryingComponent(DataType.Rotation, "0;0;0", 0.1f, 0, 360)
			.WithVaryingComponent(DataType.Scale, "1;1;1", 0.1f, 0.1f, 100)
			.WithVaryingComponent(DataType.DiffuseColor, "255;255;255;255")
			.WithVaryingComponent(DataType.Model, "Placeholder.obj")
			.Build();
	}
}
