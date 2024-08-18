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
		new WorldObject
		{
			Id = 1,
			ModelPath = @"..\Models\Cube.obj",
			Scale = new Vector3(64, 8, 64),
			Rotation = new Vector3(0, 0, 0),
			Position = new Vector3(16, -4, 0),
			Flags = ["Transparent"],
		},
		new WorldObject
		{
			Id = 2,
			ModelPath = @"..\Models\Crate.obj",
			Scale = new Vector3(1, 1, 1),
			Rotation = new Vector3(0, 0, 0),
			Position = new Vector3(-4, 0.5f, 1),
			Flags = ["Dynamic"],
		},
		new WorldObject
		{
			Id = 3,
			ModelPath = @"..\Models\Crate.obj",
			Scale = new Vector3(1, 1, 1),
			Rotation = new Vector3(0, 0, 0),
			Position = new Vector3(-0.5f, 0.5f, -4.5f),
			Flags = ["Transparent", "Dynamic"],
		},
		new WorldObject
		{
			Id = 4,
			ModelPath = @"..\Models\Crate.obj",
			Scale = new Vector3(1, 1, 1),
			Rotation = new Vector3(90, 0, 0),
			Position = new Vector3(-4.5f, 0.5f, -3f),
			Flags = [],
		},
		new WorldObject
		{
			Id = 5,
			ModelPath = @"..\Models\Sphere.obj",
			Scale = new Vector3(1, 1, 1),
			Rotation = new Vector3(0, 90, 0),
			Position = new Vector3(6, 1, 2),
			Flags = [],
		},
		new WorldObject
		{
			Id = 6,
			ModelPath = @"..\Models\Cube.obj",
			Scale = new Vector3(16, 4, 1),
			Rotation = new Vector3(0, 0, 0),
			Position = new Vector3(0, 2, -7.5f),
			Flags = [],
		},
	];
	private static readonly Entity[] _expectedEntities =
	[
		new Entity
		{
			Id = 1,
			Name = "PlayerSpawn",
			Position = new Vector3(0, 1, 0),
			Shape = new EntityShape.Point(),
			Properties = [],
		},
		new Entity
		{
			Id = 2,
			Name = "Light",
			Position = new Vector3(20, 3, 2),
			Shape = new EntityShape.Point(),
			Properties = [
				new EntityProperty
				{
					Key = "Color",
					Value = new EntityPropertyValue.Rgb(new Rgb(255, 255, 255)),
				},
				new EntityProperty
				{
					Key = "Radius",
					Value = new EntityPropertyValue.Float(20f),
				},
			],
		},
		new Entity
		{
			Id = 3,
			Name = "Light",
			Position = new Vector3(27, 1, 12),
			Shape = new EntityShape.Point(),
			Properties = [
				new EntityProperty
				{
					Key = "Color",
					Value = new EntityPropertyValue.Rgb(new Rgb(255, 47, 0)),
				},
				new EntityProperty
				{
					Key = "Radius",
					Value = new EntityPropertyValue.Float(6f),
				},
			],
		},
		new Entity
		{
			Id = 4,
			Name = "Light",
			Position = new Vector3(33, 1, 2),
			Shape = new EntityShape.Point(),
			Properties = [
				new EntityProperty
				{
					Key = "Color",
					Value = new EntityPropertyValue.Rgb(new Rgb(0, 40, 255)),
				},
				new EntityProperty
				{
					Key = "Radius",
					Value = new EntityPropertyValue.Float(20f),
				},
			],
		},
		new Entity
		{
			Id = 5,
			Name = "Field",
			Position = new Vector3(1, 1, 1),
			Shape = new EntityShape.Sphere(10),
			Properties = [],
		},
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
