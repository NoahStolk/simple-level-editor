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
}
