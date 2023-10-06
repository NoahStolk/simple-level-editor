using SimpleLevelEditor.Model;

namespace SimpleLevelEditor.State;

public static class LevelEditorState
{
	public static float TargetHeight;
	public static int GridCellCount = 64;
	public static int GridCellSize = 1;
	public static Vector3? TargetPosition;
	public static WorldObject? HighlightedObject;
	public static LevelEditorMode Mode = LevelEditorMode.AddWorldObjects;
}
