using ImGuiNET;
using SimpleLevelEditorV2.States.App;

namespace SimpleLevelEditorV2.Ui.Main;

public sealed class MainWindow
{
	public void Render(AppState appState)
	{
		ImGuiIOPtr io = ImGui.GetIO();
		Vector2 screenCenter = new(io.DisplaySize.X / 2, io.DisplaySize.Y / 2);
		Vector2 windowSize = new(1024, 768);
		ImGui.SetNextWindowPos(screenCenter - windowSize / 2);
		ImGui.SetNextWindowSize(windowSize);
		if (ImGui.Begin("Simple Level Editor", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse))
		{
			if (ImGui.Button("Entity Config Editor", new Vector2(256, 128)))
			{
				appState.CurrentView = AppView.EntityConfigEditor;
			}

			if (ImGui.Button("Level Editor", new Vector2(256, 128)))
			{
				appState.CurrentView = AppView.LevelEditor;
			}
		}

		ImGui.End();
	}
}
