using Silk.NET.GLFW;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.State;

namespace SimpleLevelEditor;

public static class Shortcuts
{
	public const string New = nameof(New);
	public const string Open = nameof(Open);
	public const string Save = nameof(Save);
	public const string SaveAs = nameof(SaveAs);
	public const string AddWorldObjectsMode = nameof(AddWorldObjectsMode);
	public const string EditWorldObjectsMode = nameof(EditWorldObjectsMode);
	public const string FocusOnCurrentObject = nameof(FocusOnCurrentObject);
	public const string DeleteSelectedObjects = nameof(DeleteSelectedObjects);

	private static readonly List<Shortcut> _shortcuts = new()
	{
		new(New, Keys.N, true, false, "New level", LevelState.New),
		new(Open, Keys.O, true, false, "Open level", LevelState.Load),
		new(Save, Keys.S, true, false, "Save level", LevelState.Save),
		new(SaveAs, Keys.S, true, true, "Save level as", LevelState.SaveAs),
		new(AddWorldObjectsMode, Keys.F1, false, false, "Add world objects", () => LevelEditorState.Mode = LevelEditorMode.AddWorldObjects),
		new(EditWorldObjectsMode, Keys.F2, false, false, "Edit world objects", () => LevelEditorState.Mode = LevelEditorMode.EditWorldObjects),
		new(FocusOnCurrentObject, Keys.A, false, false, "Focus on current object", () =>
		{
			if (ObjectEditorState.SelectedWorldObject != null)
				Camera3d.SetFocusPoint(ObjectEditorState.SelectedWorldObject.Position);
		}),
		new(DeleteSelectedObjects, Keys.Delete, false, false, "Delete selected objects", () =>
		{
			if (ObjectEditorState.SelectedWorldObject == null)
				return;

			LevelState.Level.WorldObjects.Remove(ObjectEditorState.SelectedWorldObject);
			ObjectEditorState.SelectedWorldObject = null;
		}),
	};

	public static IReadOnlyList<Shortcut> ShortcutsList => _shortcuts;

	public static string GetDescription(string shortcutName)
	{
		for (int i = 0; i < _shortcuts.Count; i++)
		{
			Shortcut shortcut = _shortcuts[i];
			if (shortcut.Id == shortcutName)
				return shortcut.Description;
		}

		return "?";
	}

	public static string GetKeyDescription(string shortcutName)
	{
		for (int i = 0; i < _shortcuts.Count; i++)
		{
			Shortcut shortcut = _shortcuts[i];
			if (shortcut.Id == shortcutName)
				return shortcut.KeyDescription;
		}

		return "?";
	}

	public static void Handle()
	{
		bool ctrl = Input.IsKeyHeld(Keys.ControlLeft) || Input.IsKeyHeld(Keys.ControlRight);
		bool shift = Input.IsKeyHeld(Keys.ShiftLeft) || Input.IsKeyHeld(Keys.ShiftRight);

		for (int i = 0; i < _shortcuts.Count; i++)
		{
			Shortcut shortcut = _shortcuts[i];
			if (Input.IsKeyPressed(shortcut.Key) && shift == shortcut.Shift && ctrl == shortcut.Ctrl)
			{
				shortcut.Action();
				break;
			}
		}
	}
}
