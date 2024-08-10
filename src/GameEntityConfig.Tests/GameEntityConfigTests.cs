using GameEntityConfig.Core;
using GameEntityConfig.Core.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;
using System.Text.Json;

namespace GameEntityConfig.Tests;

[TestClass]
public class GameEntityConfigTests
{
	private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true, IncludeFields = true };

	[TestMethod]
	public void BuildGameEntityConfig()
	{
		GameEntityConfigBuilder builder = new();
		Core.GameEntityConfig config = builder
			.WithName("Test")
			.WithDefaultComponentTypes()
			.WithComponentType<Health>()
			.WithComponentType<Radius>()
			.WithModelPath("Player.obj")
			.WithTexturePath("Audio.png")
			.WithEntityDescriptor(CreatePlayer())
			.WithEntityDescriptor(CreateLight())
			.WithEntityDescriptor(WorldObject())
			.Build();

		Assert.AreEqual("Test", config.Name);
		Assert.AreEqual(1, config.ModelPaths.Count);
		Assert.AreEqual("Player.obj", config.ModelPaths[0]);
		Assert.AreEqual(1, config.TexturePaths.Count);
		Assert.AreEqual("Audio.png", config.TexturePaths[0]);
		Assert.AreEqual(3, config.EntityDescriptors.Count);
		Assert.AreEqual("Player", config.EntityDescriptors[0].Name);
		Assert.AreEqual("Light", config.EntityDescriptors[1].Name);
		Assert.AreEqual("WorldObject", config.EntityDescriptors[2].Name);

		// TODO: We can't use JSON for this.
		// string json = JsonSerializer.Serialize(config, _jsonSerializerOptions);
		// Console.WriteLine(json);
	}

	private static EntityDescriptor CreatePlayer()
	{
		EntityDescriptorBuilder builder = new();
		return builder
			.WithName("Player")
			.WithFixedComponent(new Scale(Vector3.One))
			.WithFixedComponent(new DiffuseColor(0, 255, 90, 255))
			.WithFixedComponent(new Health(100))
			.WithFixedComponent<Visualizer>(new Visualizer.Model("Player.obj"))
			.WithVaryingComponent(new Position(Vector3.Zero), 0.1f, -100, 100)
			.Build();
	}

	private static EntityDescriptor CreateLight()
	{
		EntityDescriptorBuilder builder = new();
		return builder
			.WithName("Light")
			.WithFixedComponent(new Scale(Vector3.One))
			.WithFixedComponent(new DiffuseColor(255, 255, 255, 255))
			.WithFixedComponent<Visualizer>(new Visualizer.Billboard("Light.png"))
			.WithVaryingComponent(new Position(Vector3.Zero), 0.1f, -100, 100)
			.WithVaryingComponent(new Radius(10), 0.1f, 0, 100)
			.Build();
	}

	private static EntityDescriptor WorldObject()
	{
		EntityDescriptorBuilder builder = new();
		return builder
			.WithName("WorldObject")
			.WithVaryingComponent(new Position(Vector3.Zero), 0.1f, -100, 100)
			.WithVaryingComponent(new Rotation(Vector3.Zero), 0.1f, 0, 360)
			.WithVaryingComponent(new Scale(Vector3.One), 0.1f, 0.1f, 100)
			.WithVaryingComponent(new DiffuseColor(255, 255, 255, 255))
			.WithVaryingComponent<Visualizer>(new Visualizer.Model("Placeholder.obj"))
			.Build();
	}

	private readonly record struct Health(uint Value);
	private readonly record struct Radius(float Value);
}
