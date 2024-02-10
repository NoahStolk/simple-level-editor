using ImGuiNET;

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
		}
	}

	public static void SaveImGuiIni()
	{
		Directory.CreateDirectory(_fileDirectory);

		string iniData = ImGui.SaveIniSettingsToMemory(out _);
		File.WriteAllText(_filePath, iniData);
	}
}
