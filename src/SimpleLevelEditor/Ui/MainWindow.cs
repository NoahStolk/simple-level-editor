using ImGuiNET;
using SimpleLevelEditor.State;

namespace SimpleLevelEditor.Ui;

public static class MainWindow
{
	private static bool _showDemoWindow;
	private static bool _showControlsWindow;
	private static bool _showInputDebugWindow;
	private static bool _showPerformanceWindow;

	public static void Render()
	{
		if (_showDemoWindow)
			ImGui.ShowDemoWindow(ref _showDemoWindow);

		if (_showControlsWindow)
			ControlsWindow.Render(ref _showControlsWindow);

		if (_showInputDebugWindow)
			InputDebugWindow.Render(ref _showInputDebugWindow);

		if (_showPerformanceWindow)
			PerformanceWindow.Render(ref _showPerformanceWindow);

		Vector2 viewportSize = ImGui.GetMainViewport().Size;
		ImGui.SetNextWindowSize(viewportSize);
		ImGui.SetNextWindowPos(Vector2.Zero);

		if (ImGui.BeginMainMenuBar())
		{
			if (ImGui.BeginMenu("File"))
			{
				if (ImGui.MenuItem("New", Shortcuts.GetKeyDescription(Shortcuts.New)))
					LevelState.New();

				if (ImGui.MenuItem("Open", Shortcuts.GetKeyDescription(Shortcuts.Open)))
					LevelState.Load();

				if (ImGui.MenuItem("Save", Shortcuts.GetKeyDescription(Shortcuts.Save)))
					LevelState.Save();

				if (ImGui.MenuItem("Save As", Shortcuts.GetKeyDescription(Shortcuts.SaveAs)))
					LevelState.SaveAs();

				ImGui.EndMenu();
			}

			if (ImGui.BeginMenu("Debug"))
			{
				if (ImGui.MenuItem("Show ImGui Demo"))
					_showDemoWindow = true;

				if (ImGui.MenuItem("Show Input Debug"))
					_showInputDebugWindow = true;

				if (ImGui.MenuItem("Show Performance"))
					_showPerformanceWindow = true;

				ImGui.EndMenu();
			}

			if (ImGui.BeginMenu("Help"))
			{
				if (ImGui.MenuItem("Controls & Shortcuts"))
					_showControlsWindow = true;

				ImGui.EndMenu();
			}

			ImGui.EndMainMenuBar();
		}

		LevelInfoWindow.Render();
		LevelAssetsWindow.Render();
		HistoryWindow.Render();
		LevelEditorWindow.Render();
		ObjectEditorWindow.Render();
		DebugWindow.Render();
	}
}
