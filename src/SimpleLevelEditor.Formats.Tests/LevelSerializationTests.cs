using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLevelEditor.Formats.Level;
using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.Formats.Level.Model.EntityShapes;
using System.Text;
using System.Xml;

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
	];

	[TestMethod]
	public void TestLevelSerialization()
	{
		string levelXml = File.ReadAllText(Path.Combine("Resources", "Level.xml")).TrimEnd();
		using XmlReader xmlReader = XmlReader.Create(new StringReader(levelXml));
		Level3dData level = LevelXmlDeserializer.ReadLevel(xmlReader);

		Assert.AreEqual(1, level.Version);
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

		using MemoryStream ms = new();
		LevelXmlSerializer.WriteLevel(ms, level, false);
		string serializedLevel = Encoding.UTF8.GetString(ms.ToArray()).TrimEnd();
		Assert.AreEqual(levelXml, serializedLevel);
	}
}
