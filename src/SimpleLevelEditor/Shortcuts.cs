using Silk.NET.GLFW;
using SimpleLevelEditor.State;

namespace SimpleLevelEditor;

public static class Shortcuts
{
	private static readonly Dictionary<string, Shortcut> _shortcuts = new()
	{
		["Go to Add World Objects Mode"] = new(Keys.Number1, false, false, () => LevelEditorState.Mode = LevelEditorMode.AddWorldObjects),
		["Go to Edit World Objects Mode"] = new(Keys.Number2, false, false, () => LevelEditorState.Mode = LevelEditorMode.EditWorldObjects),
	};

	public static IReadOnlyDictionary<string, Shortcut> ShortcutsDictionary => _shortcuts;

	public static void Handle()
	{
		bool shift = Input.IsKeyHeld(Keys.ShiftLeft) || Input.IsKeyHeld(Keys.ShiftRight);
		bool ctrl = Input.IsKeyHeld(Keys.ControlLeft) || Input.IsKeyHeld(Keys.ControlRight);

		foreach (Shortcut shortcut in _shortcuts.Values)
		{
			if (Input.IsKeyPressed(shortcut.Key) && shift == shortcut.Shift && ctrl == shortcut.Ctrl)
				shortcut.Action();
		}
	}
}
