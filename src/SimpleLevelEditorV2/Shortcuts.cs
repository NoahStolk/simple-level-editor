using ImGuiNET;
using Silk.NET.GLFW;

namespace SimpleLevelEditorV2;

public sealed class Shortcuts
{
	private readonly List<Shortcut> _shortcuts;

	public Shortcuts(List<Shortcut> shortcuts)
	{
		_shortcuts = shortcuts;
	}

	public IReadOnlyList<Shortcut> ShortcutsList => _shortcuts;

	public static Shortcuts Empty { get; } = new([]);

	public string GetKeyDescription(string shortcutName)
	{
		for (int i = 0; i < _shortcuts.Count; i++)
		{
			Shortcut shortcut = _shortcuts[i];
			if (shortcut.Id == shortcutName)
				return shortcut.KeyDescription;
		}

		return "?";
	}

	public void Handle()
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
