using Editor.States.Messages;
using ImGuiNET;

namespace Editor.User;

public static class UserSettings
{
	private static readonly string _fileDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "simple-level-editor");
	private static readonly string _imGuiFilePath = Path.Combine(_fileDirectory, "imgui.ini");
	private static readonly string _settingsFilePath = Path.Combine(_fileDirectory, "settings.ini");

	public static SettingsModel Settings { get; } = new();

	public static void LoadImGuiIni()
	{
		if (!File.Exists(_imGuiFilePath))
			return;

		try
		{
			ImGui.LoadIniSettingsFromMemory(File.ReadAllText(_imGuiFilePath));
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or PathTooLongException)
		{
			MessagesState.AddError("Failed to load ImGui settings from " + _imGuiFilePath);
		}

		MessagesState.AddInfo("Loaded ImGui settings from " + _imGuiFilePath);
	}

	public static void LoadSettings()
	{
		if (!File.Exists(_settingsFilePath))
			return;

		try
		{
			Settings.Read(File.ReadAllBytes(_settingsFilePath));
		}
		catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or PathTooLongException)
		{
			MessagesState.AddError("Failed to load settings from " + _settingsFilePath);
		}

		MessagesState.AddInfo("Loaded settings from " + _settingsFilePath);
	}

	public static void SaveImGuiIni(ImGuiIOPtr io)
	{
		Directory.CreateDirectory(_fileDirectory);

		string iniData = ImGui.SaveIniSettingsToMemory(out _);
		File.WriteAllText(_imGuiFilePath, iniData);

		MessagesState.AddInfo("Saved ImGui settings to " + _imGuiFilePath);

		io.WantSaveIniSettings = false;
	}

	public static void SaveSettings()
	{
		Directory.CreateDirectory(_fileDirectory);

		File.WriteAllBytes(_settingsFilePath, Settings.Write());

		MessagesState.AddInfo("Saved settings to " + _settingsFilePath);
	}
}
