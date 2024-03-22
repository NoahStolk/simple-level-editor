using ImGuiNET;

namespace SimpleLevelEditor.Ui;

public static class SettingsWindow
{
	public static void Render(ref bool showSettingsWindow)
	{
		ImGui.SetNextWindowSize(new(300, 100), ImGuiCond.FirstUseEver);
		ImGui.SetNextWindowPos(new(50, 50), ImGuiCond.FirstUseEver);

		if (ImGui.Begin("Settings", ref showSettingsWindow))
		{
			bool startMaximized = User.UserSettings.Settings.StartMaximized;
			if (ImGui.Checkbox("Start Maximized", ref startMaximized))
			{
				User.UserSettings.Settings.StartMaximized = startMaximized;
				User.UserSettings.SaveSettings();
			}
		}

		ImGui.End();
	}
}
