using SimpleLevelEditor.Model;

namespace SimpleLevelEditor.State;

public static class LevelEditorState
{
	private static int _selectedWorldObjectId = -1;
	private static int _selectedEntityId = -1;

	public static float TargetHeight;
	public static int GridCellCount = 64;
	public static int GridCellSize = 1;
	public static Vector3? TargetPosition;
	public static bool RenderBoundingBoxes;

	public static EditMode Mode = EditMode.WorldObjects;

	public static WorldObject? HighlightedObject;
	public static Entity? HighlightedEntity;

	public enum EditMode
	{
		WorldObjects,
		Entities,
	}

	public static WorldObject? SelectedWorldObject { get; private set; }
	public static Entity? SelectedEntity { get; private set; }

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

	public static void SetSelectedEntity(Entity? entity)
	{
		SelectedEntity = entity;
		_selectedEntityId = SelectedEntity?.Id ?? -1;
	}

	public static void UpdateSelectedEntity()
	{
		Entity? selectedEntity = LevelState.Level.Entities.Find(o => o.Id == _selectedEntityId);
		if (selectedEntity != null)
		{
			SelectedEntity = selectedEntity;
		}
		else
		{
			_selectedEntityId = -1;
			SelectedEntity = null;
		}
	}
}
