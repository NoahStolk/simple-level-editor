using SimpleLevelEditor.Formats.Level3d;
using SimpleLevelEditor.Formats.Level3d.Enums;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.Ui.ChildWindows;

namespace SimpleLevelEditor.State;

public static class LevelState
{
	public static string? LevelFilePath { get; private set; }
	public static Level3dData Level { get; private set; } = Level3dData.Default;

	public static void SetLevel(string levelFilePath, Level3dData level)
	{
		LevelFilePath = levelFilePath;
		Level = level;

		MeshContainer.Rebuild(levelFilePath);
		TextureContainer.Rebuild(levelFilePath);
	}

	public static void SetTestLevel()
	{
		DebugWindow.Warnings.Add("Created test level");

		Level3dData level = new(
			version: 1,
			meshes: new() { "mesh/Sphere.obj" },
			textures: new() { "tex/BricksGreen.tga" },
			worldObjects: new() { new WorldObject(0, 0, -1, Vector3.One, default, default, WorldObjectValues.None) },
			entities: new());

		SetLevel(@"C:\Users\NOAH\Desktop\vtx-test-level", level);
	}
}
