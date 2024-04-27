using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLevelEditor.Formats.Level;
using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.Formats.Types;
using System.Numerics;
using System.Text;

namespace SimpleLevelEditor.Formats.Tests;

[TestClass]
public class LevelSerializationTests
{
	private static readonly string[] _expectedMeshes = [@"..\Meshes\Crate.obj", @"..\Meshes\Cube.obj", @"..\Meshes\Sphere.obj"];
	private static readonly string[] _expectedTextures = [@"..\Textures\Blank.tga", @"..\Textures\StoneBlue.tga", @"..\Textures\TilesColor.tga"];
	private static readonly WorldObject[] _expectedWorldObjects =
	[
		new WorldObject
		{
			Id = 1,
			Mesh = @"..\Meshes\Cube.obj",
			Texture = @"..\Textures\Blank.tga",
			Position = new Vector3(16, -4, 0),
			Rotation = new Vector3(0, 0, 0),
			Scale = new Vector3(64, 8, 64),
			Flags = ["Transparent"],
		},
		new WorldObject
		{
			Id = 2,
			Mesh = @"..\Meshes\Crate.obj",
			Texture = @"..\Textures\TilesColor.tga",
			Position = new Vector3(-4, 0.5f, 1),
			Rotation = new Vector3(0, 0, 0),
			Scale = new Vector3(1, 1, 1),
			Flags = ["Dynamic"],
		},
		new WorldObject
		{
			Id = 3,
			Mesh = @"..\Meshes\Crate.obj",
			Texture = @"..\Textures\StoneBlue.tga",
			Position = new Vector3(-0.5f, 0.5f, -4.5f),
			Rotation = new Vector3(0, 0, 0),
			Scale = new Vector3(1, 1, 1),
			Flags = ["Transparent", "Dynamic"],
		},
		new WorldObject
		{
			Id = 4,
			Mesh = @"..\Meshes\Crate.obj",
			Texture = @"..\Textures\StoneBlue.tga",
			Position = new Vector3(-4.5f, 0.5f, -3f),
			Rotation = new Vector3(90, 0, 0),
			Scale = new Vector3(1, 1, 1),
			Flags = [],
		},
		new WorldObject
		{
			Id = 5,
			Mesh = @"..\Meshes\Sphere.obj",
			Texture = @"..\Textures\StoneBlue.tga",
			Position = new Vector3(6, 1, 2),
			Rotation = new Vector3(0, 90, 0),
			Scale = new Vector3(1, 1, 1),
			Flags = [],
		},
		new WorldObject
		{
			Id = 6,
			Mesh = @"..\Meshes\Cube.obj",
			Texture = @"..\Textures\Blank.tga",
			Position = new Vector3(0, 2, -7.5f),
			Rotation = new Vector3(0, 0, 0),
			Scale = new Vector3(16, 4, 1),
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
			Shape = Types.Level.ShapeDescriptor.Point,
			Properties = [],
		},
		new Entity
		{
			Id = 2,
			Name = "Light",
			Position = new Vector3(20, 3, 2),
			Shape = Types.Level.ShapeDescriptor.Point,
			Properties =
			[
				new EntityProperty
				{
					Key = "Color",
					Value = Types.Level.EntityPropertyValue.NewRgb(new Color.Rgb(255, 255, 255)),
				},
				new EntityProperty
				{
					Key = "Radius",
					Value = Types.Level.EntityPropertyValue.NewFloat(20f),
				},
			],
		},
		new Entity
		{
			Id = 3,
			Name = "Light",
			Position = new Vector3(27, 1, 12),
			Shape = Types.Level.ShapeDescriptor.Point,
			Properties =
			[
				new EntityProperty
				{
					Key = "Color",
					Value = Types.Level.EntityPropertyValue.NewRgb(new Color.Rgb(255, 47, 0)),
				},
				new EntityProperty
				{
					Key = "Radius",
					Value = Types.Level.EntityPropertyValue.NewFloat(6f),
				},
			],
		},
		new Entity
		{
			Id = 4,
			Name = "Light",
			Position = new Vector3(33, 1, 2),
			Shape = Types.Level.ShapeDescriptor.Point,
			Properties =
			[
				new EntityProperty
				{
					Key = "Color",
					Value = Types.Level.EntityPropertyValue.NewRgb(new Color.Rgb(0, 40, 255)),
				},
				new EntityProperty
				{
					Key = "Radius",
					Value = Types.Level.EntityPropertyValue.NewFloat(20f),
				},
			],
		},
		new Entity
		{
			Id = 5,
			Name = "Field",
			Position = new Vector3(1, 1, 1),
			Shape = Types.Level.ShapeDescriptor.NewSphere(10),
			Properties = [],
		},
	];

	[TestMethod]
	public void SerializeAndDeserializeLevel()
	{
		string levelV2Path = Path.Combine("Resources", "LevelV2.xml");

		string levelV2Xml = SanitizeString(File.ReadAllText(levelV2Path));

		using FileStream fsV2 = File.OpenRead(levelV2Path);
		Level3dData levelV2 = LevelXmlDeserializer.ReadLevel(fsV2);
		AssertLevelValues(levelV2);

		using MemoryStream msV2 = new();
		LevelXmlSerializer.WriteLevel(msV2, levelV2);
		string serializedLevel = SanitizeString(Encoding.UTF8.GetString(msV2.ToArray()));
		serializedLevel.Should().BeEquivalentTo(levelV2Xml);
	}

	private static void AssertLevelValues(Level3dData level)
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
