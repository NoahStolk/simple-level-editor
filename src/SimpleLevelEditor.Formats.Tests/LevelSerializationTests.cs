using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLevelEditor.Formats.Core;
using SimpleLevelEditor.Formats.Level;
using System.Numerics;
using System.Text;

namespace SimpleLevelEditor.Formats.Tests;

[TestClass]
public class LevelSerializationTests
{
	private static readonly string[] _expectedModels = [@"..\Models\Crate.obj", @"..\Models\Cube.obj", @"..\Models\Sphere.obj"];
	private static readonly WorldObject[] _expectedWorldObjects =
	[
		new WorldObject(
			id: 1,
			modelPath: @"..\Models\Cube.obj",
			scale: new Vector3(64, 8, 64),
			rotation: new Vector3(0, 0, 0),
			position: new Vector3(16, -4, 0),
			flags: ["Transparent"]),
		new WorldObject(
			id: 2,
			modelPath: @"..\Models\Crate.obj",
			scale: new Vector3(1, 1, 1),
			rotation: new Vector3(0, 0, 0),
			position: new Vector3(-4, 0.5f, 1),
			flags: ["Dynamic"]),
		new WorldObject(
			id: 3,
			modelPath: @"..\Models\Crate.obj",
			scale: new Vector3(1, 1, 1),
			rotation: new Vector3(0, 0, 0),
			position: new Vector3(-0.5f, 0.5f, -4.5f),
			flags: ["Transparent", "Dynamic"]),
		new WorldObject(
			id: 4,
			modelPath: @"..\Models\Crate.obj",
			scale: new Vector3(1, 1, 1),
			rotation: new Vector3(90, 0, 0),
			position: new Vector3(-4.5f, 0.5f, -3f),
			flags: []),
		new WorldObject(
			id: 5,
			modelPath: @"..\Models\Sphere.obj",
			scale: new Vector3(1, 1, 1),
			rotation: new Vector3(0, 90, 0),
			position: new Vector3(6, 1, 2),
			flags: []),
		new WorldObject(
			id: 6,
			modelPath: @"..\Models\Cube.obj",
			scale: new Vector3(16, 4, 1),
			rotation: new Vector3(0, 0, 0),
			position: new Vector3(0, 2, -7.5f),
			flags: []),
	];
	private static readonly Entity[] _expectedEntities =
	[
		new Entity(1, "PlayerSpawn", new Vector3(0, 1, 0), new EntityShape.Point(), []),
		new Entity(2, "Light", new Vector3(20, 3, 2), new EntityShape.Point(), [new EntityProperty("Color", new EntityPropertyValue.Rgb(new Rgb(255, 255, 255))), new EntityProperty("Radius", new EntityPropertyValue.Float(20f))]),
		new Entity(3, "Light", new Vector3(27, 1, 12), new EntityShape.Point(), [new EntityProperty("Color", new EntityPropertyValue.Rgb(new Rgb(255, 47, 0))), new EntityProperty("Radius", new EntityPropertyValue.Float(6f))]),
		new Entity(4, "Light", new Vector3(33, 1, 2), new EntityShape.Point(), [new EntityProperty("Color", new EntityPropertyValue.Rgb(new Rgb(0, 40, 255))), new EntityProperty("Radius", new EntityPropertyValue.Float(20f))]),
		new Entity(5, "Field", new Vector3(1, 1, 1), new EntityShape.Sphere(10), []),
	];

	[TestMethod]
	public void SerializeAndDeserializeLevel()
	{
		string levelPath = Path.Combine("Resources", "Level.json");

		string levelJson = SanitizeString(File.ReadAllText(levelPath));

		using FileStream fsV2 = File.OpenRead(levelPath);
		Level3dData? levelV2 = SimpleLevelEditorJsonSerializer.DeserializeLevelFromStream(fsV2);
		Assert.IsNotNull(levelV2);
		AssertLevelValues(levelV2);

		using MemoryStream msV2 = new();
		SimpleLevelEditorJsonSerializer.SerializeLevelToStream(msV2, levelV2);
		string serializedLevel = SanitizeString(Encoding.UTF8.GetString(msV2.ToArray()));
		serializedLevel.Should().BeEquivalentTo(levelJson);
	}

	private static void AssertLevelValues(Level3dData level)
	{
		Assert.AreEqual("..\\EntityConfig.json", level.EntityConfigPath);

		Assert.AreEqual(_expectedModels.Length, level.ModelPaths.Count);
		for (int i = 0; i < _expectedModels.Length; i++)
			Assert.AreEqual(_expectedModels[i], level.ModelPaths[i]);

		Assert.AreEqual(_expectedWorldObjects.Length, level.WorldObjects.Count);
		for (int i = 0; i < _expectedWorldObjects.Length; i++)
		{
			Assert.AreEqual(_expectedWorldObjects[i].Id, level.WorldObjects[i].Id);
			Assert.AreEqual(_expectedWorldObjects[i].ModelPath, level.WorldObjects[i].ModelPath);
			Assert.AreEqual(_expectedWorldObjects[i].Scale, level.WorldObjects[i].Scale);
			Assert.AreEqual(_expectedWorldObjects[i].Rotation, level.WorldObjects[i].Rotation);
			Assert.AreEqual(_expectedWorldObjects[i].Position, level.WorldObjects[i].Position);

			Assert.AreEqual(_expectedWorldObjects[i].Flags.Count, level.WorldObjects[i].Flags.Count);
			for (int j = 0; j < _expectedWorldObjects[i].Flags.Count; j++)
				Assert.AreEqual(_expectedWorldObjects[i].Flags[j], level.WorldObjects[i].Flags[j]);
		}

		Assert.AreEqual(_expectedEntities.Length, level.Entities.Count);
		for (int i = 0; i < _expectedEntities.Length; i++)
		{
			Assert.AreEqual(_expectedEntities[i].Id, level.Entities[i].Id);
			Assert.AreEqual(_expectedEntities[i].Name, level.Entities[i].Name);
			Assert.AreEqual(_expectedEntities[i].Position, level.Entities[i].Position);
			Assert.AreEqual(_expectedEntities[i].Shape, level.Entities[i].Shape);
			Assert.AreEqual(_expectedEntities[i].Properties.Count, level.Entities[i].Properties.Count);
			for (int j = 0; j < _expectedEntities[i].Properties.Count; j++)
			{
				Assert.AreEqual(_expectedEntities[i].Properties[j].Key, level.Entities[i].Properties[j].Key);
				Assert.AreEqual(_expectedEntities[i].Properties[j].Value, level.Entities[i].Properties[j].Value);
			}
		}
	}

	private static string SanitizeString(string input)
	{
		return input.Replace("\r", string.Empty, StringComparison.Ordinal).Trim();
	}
}
