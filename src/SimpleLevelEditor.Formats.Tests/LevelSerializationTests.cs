using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLevelEditor.Formats.Level;
using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.Formats.Level.Model.EntityShapes;
using System.Text;

namespace SimpleLevelEditor.Formats.Tests;

[TestClass]
public class LevelSerializationTests
{
	private static readonly string[] _expectedMeshes = [@"..\Meshes\Crate.obj", @"..\Meshes\Cube.obj", @"..\Meshes\Sphere.obj"];
	private static readonly string[] _expectedTextures = [@"..\Textures\Blank.tga", @"..\Textures\StoneBlue.tga", @"..\Textures\TilesColor.tga"];
	private static readonly WorldObject[] _expectedWorldObjects =
	[
		new()
		{
			Id = 1,
			Mesh = @"..\Meshes\Cube.obj",
			Texture = @"..\Textures\Blank.tga",
			Position = new(16, -4, 0),
			Rotation = new(0, 0, 0),
			Scale = new(64, 8, 64),
			Flags = ["Transparent"],
		},
		new()
		{
			Id = 2,
			Mesh = @"..\Meshes\Crate.obj",
			Texture = @"..\Textures\TilesColor.tga",
			Position = new(-4, 0.5f, 1),
			Rotation = new(0, 0, 0),
			Scale = new(1, 1, 1),
			Flags = ["Dynamic"],
		},
		new()
		{
			Id = 3,
			Mesh = @"..\Meshes\Crate.obj",
			Texture = @"..\Textures\StoneBlue.tga",
			Position = new(-0.5f, 0.5f, -4.5f),
			Rotation = new(0, 0, 0),
			Scale = new(1, 1, 1),
			Flags = ["Transparent", "Dynamic"],
		},
		new()
		{
			Id = 4,
			Mesh = @"..\Meshes\Crate.obj",
			Texture = @"..\Textures\StoneBlue.tga",
			Position = new(-4.5f, 0.5f, -3f),
			Rotation = new(90, 0, 0),
			Scale = new(1, 1, 1),
			Flags = [],
		},
		new()
		{
			Id = 5,
			Mesh = @"..\Meshes\Sphere.obj",
			Texture = @"..\Textures\StoneBlue.tga",
			Position = new(6, 1, 2),
			Rotation = new(0, 90, 0),
			Scale = new(1, 1, 1),
			Flags = [],
		},
		new()
		{
			Id = 6,
			Mesh = @"..\Meshes\Cube.obj",
			Texture = @"..\Textures\Blank.tga",
			Position = new(0, 2, -7.5f),
			Rotation = new(0, 0, 0),
			Scale = new(16, 4, 1),
			Flags = [],
		},
	];
	private static readonly Entity[] _expectedEntities =
	[
		new()
		{
			Id = 1,
			Name = "PlayerSpawn",
			Position = new(0, 1, 0),
			Shape = new Point(),
			Properties = [],
		},
		new()
		{
			Id = 2,
			Name = "Light",
			Position = new(20, 3, 2),
			Shape = new Point(),
			Properties =
			[
				new()
				{
					Key = "Color",
					Value = new Rgb(255, 255, 255),
				},
				new()
				{
					Key = "Radius",
					Value = 20f,
				},
			],
		},
		new()
		{
			Id = 3,
			Name = "Light",
			Position = new(27, 1, 12),
			Shape = new Point(),
			Properties =
			[
				new()
				{
					Key = "Color",
					Value = new Rgb(255, 47, 0),
				},
				new()
				{
					Key = "Radius",
					Value = 6f,
				},
			],
		},
		new()
		{
			Id = 4,
			Name = "Light",
			Position = new(33, 1, 2),
			Shape = new Point(),
			Properties =
			[
				new()
				{
					Key = "Color",
					Value = new Rgb(0, 40, 255),
				},
				new()
				{
					Key = "Radius",
					Value = 20f,
				},
			],
		},
		new()
		{
			Id = 5,
			Name = "Field",
			Position = new(1, 1, 1),
			Shape = new Sphere(10),
			Properties = [],
		},
	];

	[TestMethod]
	public void SerializeAndDeserializeLevel()
	{
		string levelV1Path = Path.Combine("Resources", "LevelV1.xml");
		string levelV2Path = Path.Combine("Resources", "LevelV2.xml");

		string levelV2Xml = SanitizeString(File.ReadAllText(levelV2Path));

		using FileStream fsV1 = File.OpenRead(levelV1Path);
		Level3dData levelV1 = LevelXmlDeserializer.ReadLevel(fsV1);
		AssertLevelValues(1, levelV1);

		using FileStream fsV2 = File.OpenRead(levelV2Path);
		Level3dData levelV2 = LevelXmlDeserializer.ReadLevel(fsV2);
		AssertLevelValues(2, levelV2);

		using MemoryStream msV1 = new();
		LevelXmlSerializer.WriteLevel(msV1, levelV1);
		string serializedLevel = SanitizeString(Encoding.UTF8.GetString(msV1.ToArray()));
		serializedLevel.Should().BeEquivalentTo(levelV2Xml); // Should always write the latest format.

		using MemoryStream msV2 = new();
		LevelXmlSerializer.WriteLevel(msV2, levelV2);
		serializedLevel = SanitizeString(Encoding.UTF8.GetString(msV2.ToArray()));
		serializedLevel.Should().BeEquivalentTo(levelV2Xml);
	}

	private static void AssertLevelValues(int expectedVersion, Level3dData level)
	{
		Assert.AreEqual("..\\EntityConfig.xml", level.EntityConfigPath);
		CollectionAssert.AreEqual(_expectedMeshes, level.Meshes);
		CollectionAssert.AreEqual(_expectedTextures, level.Textures);

		Assert.AreEqual(_expectedWorldObjects.Length, level.WorldObjects.Count);
		for (int i = 0; i < _expectedWorldObjects.Length; i++)
		{
			Assert.AreEqual(_expectedWorldObjects[i].Id, level.WorldObjects[i].Id);
			Assert.AreEqual(_expectedWorldObjects[i].Mesh, level.WorldObjects[i].Mesh);
			Assert.AreEqual(_expectedWorldObjects[i].Texture, level.WorldObjects[i].Texture);
			Assert.AreEqual(_expectedWorldObjects[i].Position, level.WorldObjects[i].Position);
			Assert.AreEqual(_expectedWorldObjects[i].Rotation, level.WorldObjects[i].Rotation);
			Assert.AreEqual(_expectedWorldObjects[i].Scale, level.WorldObjects[i].Scale);
			CollectionAssert.AreEqual(_expectedWorldObjects[i].Flags, level.WorldObjects[i].Flags);
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
