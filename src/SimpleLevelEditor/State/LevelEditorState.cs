using SimpleLevelEditor.Model;

namespace SimpleLevelEditor.State;

#pragma warning disable S1104, S2223, SA1401, CA2211 // Public fields
public static class LevelEditorState
{
	public static float TargetHeight;
	public static int GridCellCount = 64;
	public static int GridCellSize = 1;
	public static Vector3 TargetPosition;
	public static WorldObject? HighlightedObject;
	public static LevelEditorMode Mode = LevelEditorMode.AddWorldObjects;
}

#pragma warning restore CA2211, SA1401, S2223, S1104 // Public fields
