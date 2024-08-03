using ImGuiNET;
using SimpleLevelEditor.State.States.Level;
using SimpleLevelEditor.State.States.Windows;

namespace SimpleLevelEditor.Ui;

public static class MainMenuBar
{
	public static void Render()
	{
		if (ImGui.BeginMainMenuBar())
		{
			if (ImGui.BeginMenu("File"))
			{
				if (ImGui.MenuItem("New", Shortcuts.GetKeyDescription(Shortcuts.New)))
					LevelState.New(Graphics.Gl);

				if (ImGui.MenuItem("Open", Shortcuts.GetKeyDescription(Shortcuts.Open)))
					LevelState.Load();

				if (ImGui.MenuItem("Save", Shortcuts.GetKeyDescription(Shortcuts.Save)))
					LevelState.Save();

				if (ImGui.MenuItem("Save As", Shortcuts.GetKeyDescription(Shortcuts.SaveAs)))
					LevelState.SaveAs();

				ImGui.EndMenu();
			}

			if (ImGui.BeginMenu("Settings"))
			{
				if (ImGui.MenuItem("Settings"))
					WindowsState.ShowSettingsWindow = true;

				ImGui.EndMenu();
			}

			if (ImGui.BeginMenu("Debug"))
			{
				if (ImGui.MenuItem("Show ImGui Demo"))
					WindowsState.ShowDemoWindow = true;

				if (ImGui.MenuItem("Show Input Debug"))
					WindowsState.ShowInputDebugWindow = true;

				if (ImGui.MenuItem("Show Debug"))
					WindowsState.ShowDebugWindow = true;

				ImGui.EndMenu();
			}

			if (ImGui.BeginMenu("Help"))
			{
				if (ImGui.MenuItem("Controls & Shortcuts"))
					WindowsState.ShowControlsWindow = true;

				ImGui.EndMenu();
			}

			ImGui.EndMainMenuBar();
		}
	}
}
