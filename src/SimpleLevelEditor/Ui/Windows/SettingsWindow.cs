using ImGuiNET;

namespace SimpleLevelEditor.Ui.Windows;

public static class SettingsWindow
{
	public static void Render(ref bool showSettingsWindow)
	{
		ImGui.SetNextWindowSize(new Vector2(300, 100), ImGuiCond.FirstUseEver);
		ImGui.SetNextWindowPos(new Vector2(50, 50), ImGuiCond.FirstUseEver);

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
