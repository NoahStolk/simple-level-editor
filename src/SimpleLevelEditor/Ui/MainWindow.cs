using ImGuiNET;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Ui.ChildWindows;

namespace SimpleLevelEditor.Ui;

public static class MainWindow
{
	private static bool _showDemoWindow;
	private static bool _showControlsWindow;

	public static void Render()
	{
		if (_showDemoWindow)
			ImGui.ShowDemoWindow(ref _showDemoWindow);

		if (_showControlsWindow)
			ControlsWindow.Render(ref _showControlsWindow);

		Vector2 viewportSize = ImGui.GetMainViewport().Size;
		ImGui.SetNextWindowSize(viewportSize);
		ImGui.SetNextWindowPos(Vector2.Zero);

		if (ImGui.Begin("3D Level Editor", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.MenuBar))
		{
			if (ImGui.BeginMenuBar())
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

					ImGui.EndMenu();
				}

				if (ImGui.BeginMenu("Help"))
				{
					if (ImGui.MenuItem("Controls & Shortcuts"))
						_showControlsWindow = true;

					ImGui.EndMenu();
				}

				ImGui.EndMenuBar();
			}

			const int leftWidth = 256;
			const int rightWidth = 512;
			const int bottomHeight = 256;
			float middleWidth = viewportSize.X - leftWidth - rightWidth;

			if (ImGui.BeginChild("Left", new(leftWidth, 0)))
			{
				const int levelInfoHeight = 192;
				LevelInfoWindow.Render(new(leftWidth, levelInfoHeight));
				LevelAssetsWindow.Render(new(leftWidth, viewportSize.Y - bottomHeight - levelInfoHeight));

				if (ImGui.BeginChild("History", default, true))
				{
					for (int i = 0; i < LevelState.History.Count; i++)
					{
						bool isCurrent = i == LevelState.CurrentHistoryIndex;
						LevelState.HistoryEntry entry = LevelState.History[i];
						ImGui.TextColored(isCurrent ? new(0, 1, 0, 1) : Vector4.One, entry.EditDescription);
					}
				}

				ImGui.EndChild(); // End History
			}

			ImGui.EndChild(); // End Left

			ImGui.SameLine();

			if (ImGui.BeginChild("Middle", new(middleWidth, 0)))
			{
				LevelEditorWindow.Render(new(middleWidth, viewportSize.Y - 27));
			}

			ImGui.EndChild(); // End Middle

			ImGui.SameLine();

			if (ImGui.BeginChild("Right", new(0, 0)))
			{
				WorldObjectEditorWindow.Render(new(0, viewportSize.Y - bottomHeight));
				DebugWindow.Render(default);
			}

			ImGui.EndChild(); // End Right
		}

		ImGui.End(); // End 3D Level Editor
	}
}
