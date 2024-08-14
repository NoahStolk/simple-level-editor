using SimpleLevelEditorV2.Formats.Level.Model;

namespace SimpleLevelEditorV2.States.LevelEditor;

public sealed class LevelEditorState
{
	public LevelModel Level { get; set; } = new("temp", []);

	public bool ShowShortcutsWindow;
}
