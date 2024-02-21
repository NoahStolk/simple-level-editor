using SimpleLevelEditor.Formats.Level.Model;

namespace SimpleLevelEditor.State;

public static class LevelEditorState
{
	// Store immutable ids to keep track of selected objects when undoing/redoing.
	private static readonly Dictionary<WorldObject, int> _worldObjectsById = [];
	private static readonly Dictionary<Entity, int> _entitiesById = [];

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

	public static void AddWorldObject(WorldObject worldObject)
	{
		_worldObjectsById[worldObject] = _worldObjectsById.Count + 1;
	}

	public static void AddEntity(Entity entity)
	{
		_entitiesById[entity] = _entitiesById.Count + 1;
	}

	public static int GetWorldObjectIdForDebugging(WorldObject worldObject)
	{
		return _worldObjectsById[worldObject];
	}

	public static int GetEntityIdForDebugging(Entity entity)
	{
		return _entitiesById[entity];
	}

	public static void SetSelectedWorldObject(WorldObject? worldObject)
	{
		SelectedWorldObject = worldObject;
		_selectedWorldObjectId = worldObject == null ? -1 : _worldObjectsById[worldObject];

		_selectedEntityId = -1;
		SelectedEntity = null;

		Mode = EditMode.WorldObjects;
	}

	public static void UpdateSelectedWorldObject()
	{
		WorldObject? selectedWorldObject = LevelState.Level.WorldObjects.Find(o => _worldObjectsById.TryGetValue(o, out int id) && id == _selectedWorldObjectId);
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
		_selectedEntityId = entity == null ? -1 : _entitiesById[entity];

		_selectedWorldObjectId = -1;
		SelectedWorldObject = null;

		Mode = EditMode.Entities;
	}

	public static void UpdateSelectedEntity()
	{
		Entity? selectedEntity = LevelState.Level.Entities.Find(o => _entitiesById.TryGetValue(o, out int id) && id == _selectedEntityId);
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
