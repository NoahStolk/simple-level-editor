using ImGuiNET;
using NativeFileDialogUtils;
using SimpleLevelEditorV2.Formats.Level;
using SimpleLevelEditorV2.Formats.Level.Model;
using SimpleLevelEditorV2.States.App;
using SimpleLevelEditorV2.States.LevelEditor;

namespace SimpleLevelEditorV2.Ui.LevelEditor;

public sealed class LevelMainMenuBar
{
	public void Render(AppState appState, LevelEditorState levelEditorState)
	{
		if (ImGui.BeginMainMenuBar())
		{
			if (ImGui.BeginMenu("File"))
			{
				if (ImGui.MenuItem("New"))
				{
					levelEditorState.Level = new LevelModel("temp", []);
				}

				if (ImGui.MenuItem("Open"))
				{
					DialogWrapper.FileOpen(
						s =>
						{
							if (s == null)
								return;

							string json = File.ReadAllText(s);
							levelEditorState.Level = LevelSerializer.Deserialize(json);
						},
						"json");
				}

				if (ImGui.MenuItem("Save"))
				{
					DialogWrapper.FileSave(
						s =>
						{
							if (s == null)
								return;

							string json = LevelSerializer.Serialize(levelEditorState.Level);
							File.WriteAllText(Path.ChangeExtension(s, ".json"), json);
						},
						"json");
				}

				ImGui.Separator();

				if (ImGui.MenuItem("Exit"))
				{
					appState.CurrentView = AppView.Main;
				}

				ImGui.EndMenu();
			}

			if (ImGui.BeginMenu("Help"))
			{
				if (ImGui.MenuItem("Shortcuts"))
				{
					levelEditorState.ShowShortcutsWindow = true;
				}

				ImGui.EndMenu();
			}

			ImGui.EndMainMenuBar();
		}
	}
}
