using ImGuiNET;
using Silk.NET.GLFW;
using SimpleLevelEditor.Logic;
using SimpleLevelEditor.State;
using SimpleLevelEditor.State.Level;

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
		new Shortcut(New, Keys.N, true, false, "New level", LevelState.New),
		new Shortcut(Open, Keys.O, true, false, "Open level", LevelState.Load),
		new Shortcut(Save, Keys.S, true, false, "Save level", LevelState.Save),
		new Shortcut(SaveAs, Keys.S, true, true, "Save level as", LevelState.SaveAs),
		new Shortcut(AddNewObject, Keys.C, false, false, "Add new object/entity", MainLogic.AddNew),
		new Shortcut(DeleteSelectedObjects, Keys.Delete, false, false, "Delete selected objects/entities", MainLogic.Remove),
		new Shortcut(FocusOnCurrentObject, Keys.Q, false, false, "Focus on current object/entity", MainLogic.Focus),
		new Shortcut(Undo, Keys.Z, true, false, "Undo", () => LevelState.SetHistoryIndex(LevelState.CurrentHistoryIndex - 1)),
		new Shortcut(Redo, Keys.Y, true, false, "Redo", () => LevelState.SetHistoryIndex(LevelState.CurrentHistoryIndex + 1)),
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
		if (ImGui.GetIO().WantTextInput)
			return;

		bool ctrl = Input.GlfwInput.IsKeyDown(Keys.ControlLeft) || Input.GlfwInput.IsKeyDown(Keys.ControlRight);
		bool shift = Input.GlfwInput.IsKeyDown(Keys.ShiftLeft) || Input.GlfwInput.IsKeyDown(Keys.ShiftRight);

		for (int i = 0; i < _shortcuts.Count; i++)
		{
			Shortcut shortcut = _shortcuts[i];
			if (Input.GlfwInput.IsKeyPressed(shortcut.Key) && shift == shortcut.Shift && ctrl == shortcut.Ctrl)
			{
				shortcut.Action();
				break;
			}
		}
	}
}
