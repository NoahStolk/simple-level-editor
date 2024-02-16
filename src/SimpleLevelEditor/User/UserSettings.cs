using ImGuiNET;
using SimpleLevelEditor.State;

namespace SimpleLevelEditor.User;

public static class UserSettings
{
	private static readonly string _fileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "simple-level-editor");
	private static readonly string _filePath = Path.Combine(_fileDirectory, "imgui.ini");

	public static void LoadImGuiIni()
	{
		if (File.Exists(_filePath))
		{
			ImGui.LoadIniSettingsFromMemory(File.ReadAllText(_filePath));

			DebugState.AddWarning("Loaded ImGui settings from " + _filePath);
		}
	}

	public static void SaveImGuiIni(ImGuiIOPtr io)
	{
		Directory.CreateDirectory(_fileDirectory);

		string iniData = ImGui.SaveIniSettingsToMemory(out _);
		File.WriteAllText(_filePath, iniData);

		DebugState.AddWarning("Saved ImGui settings to " + _filePath);

		io.WantSaveIniSettings = false;
	}
}
