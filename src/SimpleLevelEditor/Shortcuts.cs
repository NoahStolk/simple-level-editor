using ImGuiGlfw;
using Silk.NET.GLFW;
using SimpleLevelEditor.Logic;
using SimpleLevelEditor.State;

namespace SimpleLevelEditor;

public static class Shortcuts
{
	public const string New = nameof(New);
	public const string Open = nameof(Open);
	public const string Save = nameof(Save);
	public const string SaveAs = nameof(SaveAs);
	public const string AddNewObject = nameof(AddNewObject);
	public const string FocusOnCurrentObject = nameof(FocusOnCurrentObject);
	public const string DeleteSelectedObjects = nameof(DeleteSelectedObjects);
	public const string Undo = nameof(Undo);
	public const string Redo = nameof(Redo);

	private static readonly List<Shortcut> _shortcuts =
	[
		new(New, Keys.N, true, false, "New level", LevelState.New),
		new(Open, Keys.O, true, false, "Open level", LevelState.Load),
		new(Save, Keys.S, true, false, "Save level", LevelState.Save),
		new(SaveAs, Keys.S, true, true, "Save level as", LevelState.SaveAs),
		new(AddNewObject, Keys.Space, false, false, "Add new object/entity", MainLogic.AddNew),
		new(DeleteSelectedObjects, Keys.Delete, false, false, "Delete selected objects/entities", MainLogic.Remove),
		new(FocusOnCurrentObject, Keys.A, false, false, "Focus on current object/entity", MainLogic.Focus),
		new(Undo, Keys.Z, true, false, "Undo", () => LevelState.SetHistoryIndex(LevelState.CurrentHistoryIndex - 1)),
		new(Redo, Keys.Y, true, false, "Redo", () => LevelState.SetHistoryIndex(LevelState.CurrentHistoryIndex + 1)),
	];

	public static IReadOnlyList<Shortcut> ShortcutsList => _shortcuts;

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
		bool ctrl = GlfwInput.IsKeyDown(Keys.ControlLeft) || GlfwInput.IsKeyDown(Keys.ControlRight);
		bool shift = GlfwInput.IsKeyDown(Keys.ShiftLeft) || GlfwInput.IsKeyDown(Keys.ShiftRight);

		for (int i = 0; i < _shortcuts.Count; i++)
		{
			Shortcut shortcut = _shortcuts[i];
			if (GlfwInput.IsKeyPressed(shortcut.Key) && shift == shortcut.Shift && ctrl == shortcut.Ctrl)
			{
				shortcut.Action();
				break;
			}
		}
	}
}
