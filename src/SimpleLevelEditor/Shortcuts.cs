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

	private static readonly Dictionary<string, Shortcut> _shortcuts = new()
	{
		[New] = new(Keys.N, true, false, LevelState.New, "New level"),
		[Open] = new(Keys.O, true, false, LevelState.Load, "Open level"),
		[Save] = new(Keys.S, true, false, LevelState.Save, "Save level"),
		[SaveAs] = new(Keys.S, true, true, LevelState.SaveAs, "Save level as"),
		[AddWorldObjectsMode] = new(Keys.F1, false, false, () => LevelEditorState.Mode = LevelEditorMode.AddWorldObjects, "Add world objects"),
		[EditWorldObjectsMode] = new(Keys.F2, false, false, () => LevelEditorState.Mode = LevelEditorMode.EditWorldObjects, "Edit world objects"),
	};

	public static IReadOnlyDictionary<string, Shortcut> ShortcutsDictionary => _shortcuts;

	public static string GetDescription(string shortcutName)
	{
		return _shortcuts.TryGetValue(shortcutName, out Shortcut? shortcut) ? shortcut.Description : "?";
	}

	public static string GetKeyDescription(string shortcutName)
	{
		return _shortcuts.TryGetValue(shortcutName, out Shortcut? shortcut) ? shortcut.KeyDescription : "?";
	}

	public static void Handle()
	{
		bool ctrl = Input.IsKeyHeld(Keys.ControlLeft) || Input.IsKeyHeld(Keys.ControlRight);
		bool shift = Input.IsKeyHeld(Keys.ShiftLeft) || Input.IsKeyHeld(Keys.ShiftRight);

		foreach (Shortcut shortcut in _shortcuts.Values)
		{
			if (Input.IsKeyPressed(shortcut.Key) && shift == shortcut.Shift && ctrl == shortcut.Ctrl)
			{
				shortcut.Action();
				break;
			}
		}
	}
}
