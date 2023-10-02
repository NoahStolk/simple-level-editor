using Silk.NET.GLFW;
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

	private static readonly List<Shortcut> _shortcuts = new()
	{
		new(New, Keys.N, true, false, LevelState.New, "New level"),
		new(Open, Keys.O, true, false, LevelState.Load, "Open level"),
		new(Save, Keys.S, true, false, LevelState.Save, "Save level"),
		new(SaveAs, Keys.S, true, true, LevelState.SaveAs, "Save level as"),
		new(AddWorldObjectsMode, Keys.F1, false, false, () => LevelEditorState.Mode = LevelEditorMode.AddWorldObjects, "Add world objects"),
		new(EditWorldObjectsMode, Keys.F2, false, false, () => LevelEditorState.Mode = LevelEditorMode.EditWorldObjects, "Edit world objects"),
	};

	public static IReadOnlyList<Shortcut> ShortcutsList => _shortcuts;

	public static string GetDescription(string shortcutName)
	{
		Shortcut? shortcut = _shortcuts.Find(s => s.Id == shortcutName);
		return shortcut?.Description ?? "?";
	}

	public static string GetKeyDescription(string shortcutName)
	{
		Shortcut? shortcut = _shortcuts.Find(s => s.Id == shortcutName);
		return shortcut?.KeyDescription ?? "?";
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
