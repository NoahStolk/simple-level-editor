using FluentAssertions;
using Microsoft.FSharp.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleLevelEditor.Formats.Types;
using SimpleLevelEditor.Formats.Types.Level;
using System.Numerics;
using System.Text;
using System.Text.Json;

namespace SimpleLevelEditor.Formats.Tests;

[TestClass]
public class LevelSerializationTests
{
	private static readonly string[] _expectedMeshes = [@"..\Meshes\Crate.obj", @"..\Meshes\Cube.obj", @"..\Meshes\Sphere.obj"];
	private static readonly string[] _expectedTextures = [@"..\Textures\Blank.tga", @"..\Textures\StoneBlue.tga", @"..\Textures\TilesColor.tga"];
	private static readonly WorldObject[] _expectedWorldObjects =
	[
		new WorldObject(
			id: 1,
			mesh: @"..\Meshes\Cube.obj",
			texture: @"..\Textures\Blank.tga",
			scale: new Vector3(64, 8, 64),
			rotation: new Vector3(0, 0, 0),
			position: new Vector3(16, -4, 0),
			flags: ListModule.OfSeq(["Transparent"])),
		new WorldObject(
			id: 2,
			mesh: @"..\Meshes\Crate.obj",
			texture: @"..\Textures\TilesColor.tga",
			scale: new Vector3(1, 1, 1),
			rotation: new Vector3(0, 0, 0),
			position: new Vector3(-4, 0.5f, 1),
			flags: ListModule.OfSeq(["Dynamic"])),
		new WorldObject(
			id: 3,
			mesh: @"..\Meshes\Crate.obj",
			texture: @"..\Textures\StoneBlue.tga",
			scale: new Vector3(1, 1, 1),
			rotation: new Vector3(0, 0, 0),
			position: new Vector3(-0.5f, 0.5f, -4.5f),
			flags: ListModule.OfSeq(["Transparent", "Dynamic"])),
		new WorldObject(
			id: 4,
			mesh: @"..\Meshes\Crate.obj",
			texture: @"..\Textures\StoneBlue.tga",
			scale: new Vector3(1, 1, 1),
			rotation: new Vector3(90, 0, 0),
			position: new Vector3(-4.5f, 0.5f, -3f),
			flags: ListModule.Empty<string>()),
		new WorldObject(
			id: 5,
			mesh: @"..\Meshes\Sphere.obj",
			texture: @"..\Textures\StoneBlue.tga",
			scale: new Vector3(1, 1, 1),
			rotation: new Vector3(0, 90, 0),
			position: new Vector3(6, 1, 2),
			flags: ListModule.Empty<string>()),
		new WorldObject(
			id: 6,
			mesh: @"..\Meshes\Cube.obj",
			texture: @"..\Textures\Blank.tga",
			scale: new Vector3(16, 4, 1),
			rotation: new Vector3(0, 0, 0),
			position: new Vector3(0, 2, -7.5f),
			flags: ListModule.Empty<string>()),
	];
	private static readonly Entity[] _expectedEntities =
	[
		new Entity(1, "PlayerSpawn", new Vector3(0, 1, 0), ShapeDescriptor.Point, ListModule.Empty<EntityProperty>()),
		new Entity(2, "Light", new Vector3(20, 3, 2), ShapeDescriptor.Point, ListModule.OfSeq([new EntityProperty("Color", EntityPropertyValue.NewRgb(new Rgb(255, 255, 255))), new EntityProperty("Radius", EntityPropertyValue.NewFloat(20f))])),
		new Entity(3, "Light", new Vector3(27, 1, 12), ShapeDescriptor.Point, ListModule.OfSeq([new EntityProperty("Color", EntityPropertyValue.NewRgb(new Rgb(255, 47, 0))), new EntityProperty("Radius", EntityPropertyValue.NewFloat(6f))])),
		new Entity(4, "Light", new Vector3(33, 1, 2), ShapeDescriptor.Point, ListModule.OfSeq([new EntityProperty("Color", EntityPropertyValue.NewRgb(new Rgb(0, 40, 255))), new EntityProperty("Radius", EntityPropertyValue.NewFloat(20f))])),
		new Entity(5, "Field", new Vector3(1, 1, 1), ShapeDescriptor.NewSphere(10), ListModule.Empty<EntityProperty>()),
	];

	[TestMethod]
	public void SerializeAndDeserializeLevel()
	{
		string levelV2Path = Path.Combine("Resources", "LevelV2.xml");

		string levelV2Xml = SanitizeString(File.ReadAllText(levelV2Path));

		using FileStream fsV2 = File.OpenRead(levelV2Path);
		Level3dData? levelV2 = JsonSerializer.Deserialize<Level3dData>(fsV2, SimpleLevelEditorJsonSerializer.DefaultSerializerOptions);
		Assert.IsNotNull(levelV2);
		AssertLevelValues(levelV2);

		using MemoryStream msV2 = new();
		JsonSerializer.Serialize(msV2, levelV2, SimpleLevelEditorJsonSerializer.DefaultSerializerOptions);
		string serializedLevel = SanitizeString(Encoding.UTF8.GetString(msV2.ToArray()));
		serializedLevel.Should().BeEquivalentTo(levelV2Xml);
	}

	private static void AssertLevelValues(Level3dData level)
	{
		Assert.AreEqual("..\\EntityConfig.xml", level.EntityConfigPath);

		Assert.AreEqual(_expectedMeshes.Length, level.Meshes.Length);
		for (int i = 0; i < _expectedMeshes.Length; i++)
			Assert.AreEqual(_expectedMeshes[i], level.Meshes[i]);

		Assert.AreEqual(_expectedTextures.Length, level.Textures.Length);
		for (int i = 0; i < _expectedTextures.Length; i++)
			Assert.AreEqual(_expectedTextures[i], level.Textures[i]);

		Assert.AreEqual(_expectedWorldObjects.Length, level.WorldObjects.Length);
		for (int i = 0; i < _expectedWorldObjects.Length; i++)
		{
			Assert.AreEqual(_expectedWorldObjects[i].Id, level.WorldObjects[i].Id);
			Assert.AreEqual(_expectedWorldObjects[i].Mesh, level.WorldObjects[i].Mesh);
			Assert.AreEqual(_expectedWorldObjects[i].Texture, level.WorldObjects[i].Texture);
			Assert.AreEqual(_expectedWorldObjects[i].Scale, level.WorldObjects[i].Scale);
			Assert.AreEqual(_expectedWorldObjects[i].Rotation, level.WorldObjects[i].Rotation);
			Assert.AreEqual(_expectedWorldObjects[i].Position, level.WorldObjects[i].Position);

			Assert.AreEqual(_expectedWorldObjects[i].Flags.Length, level.WorldObjects[i].Flags.Length);
			for (int j = 0; j < _expectedWorldObjects[i].Flags.Length; j++)
				Assert.AreEqual(_expectedWorldObjects[i].Flags[j], level.WorldObjects[i].Flags[j]);
		}

		Assert.AreEqual(_expectedEntities.Length, level.Entities.Length);
		for (int i = 0; i < _expectedEntities.Length; i++)
		{
			Assert.AreEqual(_expectedEntities[i].Id, level.Entities[i].Id);
			Assert.AreEqual(_expectedEntities[i].Name, level.Entities[i].Name);
			Assert.AreEqual(_expectedEntities[i].Position, level.Entities[i].Position);
			Assert.AreEqual(_expectedEntities[i].Shape, level.Entities[i].Shape);
			Assert.AreEqual(_expectedEntities[i].Properties.Length, level.Entities[i].Properties.Length);
			for (int j = 0; j < _expectedEntities[i].Properties.Length; j++)
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
