using SimpleLevelEditor.Formats.Level.Model;

namespace SimpleLevelEditor.State;

public static class LevelEditorState
{
	private static int _selectedWorldObjectHash = -1;
	private static int _selectedEntityHash = -1;

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
		_selectedWorldObjectHash = -1;
	}

	public static void ClearSelectedEntity()
	{
		SelectedEntity = null;
		_selectedEntityHash = -1;
	}

	public static void SetSelectedWorldObject(WorldObject? worldObject)
	{
		SelectedWorldObject = worldObject;
		_selectedWorldObjectHash = SelectedWorldObject?.GenerateHash() ?? -1;

		_selectedEntityHash = -1;
		SelectedEntity = null;

		Mode = EditMode.WorldObjects;
	}

	public static void UpdateSelectedWorldObject()
	{
		WorldObject? selectedWorldObject = LevelState.Level.WorldObjects.Find(o => o.GenerateHash() == _selectedWorldObjectHash);
		if (selectedWorldObject != null)
		{
			SetSelectedWorldObject(selectedWorldObject);
		}
		else
		{
			_selectedWorldObjectHash = -1;
			SelectedWorldObject = null;
		}
	}

	public static void SetSelectedEntity(Entity? entity)
	{
		SelectedEntity = entity;
		_selectedEntityHash = SelectedEntity?.GenerateHash() ?? -1;

		_selectedWorldObjectHash = -1;
		SelectedWorldObject = null;

		Mode = EditMode.Entities;
	}

	public static void UpdateSelectedEntity()
	{
		Entity? selectedEntity = LevelState.Level.Entities.Find(o => o.GenerateHash() == _selectedEntityHash);
		if (selectedEntity != null)
		{
			SetSelectedEntity(selectedEntity);
		}
		else
		{
			_selectedEntityHash = -1;
			SelectedEntity = null;
		}
	}
}
