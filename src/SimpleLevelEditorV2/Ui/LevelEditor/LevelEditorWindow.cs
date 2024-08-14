using ImGuiNET;
using SimpleLevelEditorV2.States.App;
using SimpleLevelEditorV2.States.LevelEditor;

namespace SimpleLevelEditorV2.Ui.LevelEditor;

public sealed class LevelEditorWindow
{
	public void Render(AppState appState, LevelEditorState levelEditorState)
	{
		if (ImGui.Begin("Level Editor"))
		{
			ImGui.Text("Hello, world!");
		}

		ImGui.End();
	}
}
