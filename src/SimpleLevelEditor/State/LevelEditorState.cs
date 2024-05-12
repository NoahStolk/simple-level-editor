using SimpleLevelEditor.Formats.Types.Level;

namespace SimpleLevelEditor.State;

public static class LevelEditorState
{
	private static int _selectedWorldObjectId = -1;
	private static int _selectedEntityId = -1;

	public static float TargetHeight;
	public static int GridCellCount = 64;
	public static int GridCellSize = 1;
	public static Vector3? TargetPosition;

	public static EditMode Mode = EditMode.WorldObjects;

	public enum EditMode
	{
		WorldObjects,
		Entities,
	}

	public static WorldObject? HighlightedObject { get; private set; }
	public static Entity? HighlightedEntity { get; private set; }

	public static WorldObject? SelectedWorldObject { get; private set; }
	public static Entity? SelectedEntity { get; private set; }

	public static Dictionary<string, bool> RenderFilter { get; } = [];

	public static bool ShouldRenderWorldObjects()
	{
		return RenderFilter.TryGetValue("WorldObjects", out bool value) && value;
	}

	public static bool ShouldRenderEntity(Entity entity)
	{
		return !RenderFilter.TryGetValue($"Entities:{entity.Name}", out bool value) || value;
	}

	public static void ClearHighlight()
	{
		HighlightedObject = null;
		HighlightedEntity = null;
	}

	public static void SetHighlightedWorldObject(WorldObject? worldObject)
	{
		HighlightedObject = worldObject;
		HighlightedEntity = null;
	}

	public static void SetHighlightedEntity(Entity? entity)
	{
		HighlightedEntity = entity;
		HighlightedObject = null;
	}

	public static void ClearSelectedWorldObject()
	{
		SelectedWorldObject = null;
		_selectedWorldObjectId = -1;
	}

	public static void ClearSelectedEntity()
	{
		SelectedEntity = null;
		_selectedEntityId = -1;
	}

	public static void SetSelectedWorldObject(WorldObject? worldObject)
	{
		SelectedWorldObject = worldObject;
		_selectedWorldObjectId = SelectedWorldObject?.Id ?? -1;

		_selectedEntityId = -1;
		SelectedEntity = null;

		Mode = EditMode.WorldObjects;
	}

	public static void UpdateSelectedWorldObject()
	{
		WorldObject? selectedWorldObject = LevelState.Level.WorldObjects.Find(o => o.Id == _selectedWorldObjectId);
		if (selectedWorldObject != null)
		{
			SetSelectedWorldObject(selectedWorldObject);
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

		_selectedWorldObjectId = -1;
		SelectedWorldObject = null;

		Mode = EditMode.Entities;
	}

	public static void UpdateSelectedEntity()
	{
		Entity? selectedEntity = LevelState.Level.Entities.Find(o => o.Id == _selectedEntityId);
		if (selectedEntity != null)
		{
			SetSelectedEntity(selectedEntity);
		}
		else
		{
			_selectedEntityId = -1;
			SelectedEntity = null;
		}
	}
}
