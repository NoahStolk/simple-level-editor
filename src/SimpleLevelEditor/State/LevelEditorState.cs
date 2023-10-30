using SimpleLevelEditor.Model;

namespace SimpleLevelEditor.State;

public static class LevelEditorState
{
	private static int _selectedWorldObjectId = -1;

	public static float TargetHeight;
	public static int GridCellCount = 64;
	public static int GridCellSize = 1;
	public static Vector3? TargetPosition;
	public static bool RenderBoundingBoxes;

	public static WorldObject? HighlightedObject;
	public static WorldObject? SelectedWorldObject { get; private set; }

	public static void SetSelectedWorldObject(WorldObject? worldObject)
	{
		SelectedWorldObject = worldObject;
		_selectedWorldObjectId = SelectedWorldObject?.Id ?? -1;
	}

	public static void UpdateSelectedWorldObject()
	{
		WorldObject? selectedWorldObject = LevelState.Level.WorldObjects.Find(o => o.Id == _selectedWorldObjectId);
		if (selectedWorldObject != null)
		{
			SelectedWorldObject = selectedWorldObject;
		}
		else
		{
			_selectedWorldObjectId = -1;
			SelectedWorldObject = null;
		}
	}
}
