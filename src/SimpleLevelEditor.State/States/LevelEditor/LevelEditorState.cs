using SimpleLevelEditor.Formats.Types.Level;
using SimpleLevelEditor.State.States.Level;
using System.Numerics;

namespace SimpleLevelEditor.State.States.LevelEditor;

public static class LevelEditorState
{
	private static readonly float[] _snapPoints = [0, 0.125f, 0.25f, 0.5f, 1, 2, 4, 8];

	private static int _selectedWorldObjectId = -1;
	private static int _selectedEntityId = -1;

	public static float TargetHeight;
	public static float GridCellFadeOutMinDistance = 64;
	public static float GridCellFadeOutMaxDistance = 128;
	public static int GridCellInterval = 8;
	public static Vector3? TargetPosition;

	public static EditMode Mode = EditMode.WorldObjects;

	public static int SnapIndex = 4;

	public static float Snap => SnapIndex >= 0 && SnapIndex < _snapPoints.Length ? _snapPoints[SnapIndex] : 0;

	public static IReadOnlyList<float> SnapPoints => _snapPoints;

	public enum EditMode
	{
		WorldObjects,
		Entities,
	}

	public static WorldObject? HighlightedObject { get; private set; }
	public static Entity? HighlightedEntity { get; private set; }

	public static WorldObject? SelectedWorldObject { get; private set; }
	public static Entity? SelectedEntity { get; private set; }

	public static Vector3? MoveTargetPosition { get; set; }

	public static bool ShouldRenderWorldObjects { get; set; } = true;

	// TODO: The entity filter should be based on the entity config.
	public static Dictionary<string, bool> EntityRenderFilter { get; } = [];

	public static bool ShouldRenderEntity(Entity entity)
	{
		return !EntityRenderFilter.TryGetValue(entity.Name, out bool value) || value;
	}

	public static bool IsAnyObjectHidden()
	{
		return !ShouldRenderWorldObjects || EntityRenderFilter.Values.Any(v => !v);
	}

	public static void ClearHighlight()
	{
		HighlightedObject = null;
		HighlightedEntity = null;
	}

	public static void SetHighlightedWorldObject(WorldObject worldObject)
	{
		HighlightedObject = worldObject;
		HighlightedEntity = null;
	}

	public static void SetHighlightedEntity(Entity entity)
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
		WorldObject? selectedWorldObject = LevelState.Level.WorldObjects.FirstOrDefault(o => o.Id == _selectedWorldObjectId);
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
		Entity? selectedEntity = LevelState.Level.Entities.FirstOrDefault(o => o.Id == _selectedEntityId);
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
