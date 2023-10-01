using ImGuiNET;
using NativeFileDialogSharp;
using SimpleLevelEditor.Formats;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Ui.ChildWindows;
using System.Text;
using System.Xml;

namespace SimpleLevelEditor.Ui;

public static class MainWindow
{
	private const string _fileExtension = "xml";
	private static readonly XmlWriterSettings _xmlWriterSettings = new() { Indent = true, Encoding = new UTF8Encoding(false) };
	private static bool _showDemoWindow;
	private static bool _showShortcutsWindow;

	public static void Render()
	{
		if (_showDemoWindow)
			ImGui.ShowDemoWindow(ref _showDemoWindow);

		if (_showShortcutsWindow)
			ShortcutsWindow.Render(ref _showShortcutsWindow);

		Vector2 viewportSize = ImGui.GetMainViewport().Size;
		ImGui.SetNextWindowSize(viewportSize);
		ImGui.SetNextWindowPos(Vector2.Zero);

		if (ImGui.Begin("3D Level Editor", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.MenuBar))
		{
			if (ImGui.BeginMenuBar())
			{
				if (ImGui.BeginMenu("File"))
				{
					if (ImGui.MenuItem("New"))
					{
						DialogResult dialogResult = Dialog.FileSave(_fileExtension);
						if (dialogResult is { IsOk: true })
						{
							using MemoryStream ms = new();
							using XmlWriter writer = XmlWriter.Create(ms, _xmlWriterSettings);

							Level3dData level = Level3dData.Default.DeepCopy();

							XmlFormatSerializer.WriteLevel(level, writer);
							File.WriteAllBytes(dialogResult.Path, ms.ToArray());

							LevelState.SetLevel(dialogResult.Path, level);
						}
					}

					if (ImGui.MenuItem("Open"))
					{
						DialogResult dialogResult = Dialog.FileOpen(_fileExtension);
						if (dialogResult is { IsOk: true })
						{
							Load(dialogResult.Path);
						}
					}

					if (ImGui.MenuItem("Save"))
					{
						if (LevelState.LevelFilePath != null)
						{
							Save(LevelState.LevelFilePath);
						}
						else
						{
							DialogResult dialogResult = Dialog.FileSave(_fileExtension);
							if (dialogResult is { IsOk: true })
							{
								Save(dialogResult.Path);
							}
						}
					}

					if (ImGui.MenuItem("Save As"))
					{
						DialogResult dialogResult = Dialog.FileSave(_fileExtension);
						if (dialogResult is { IsOk: true })
						{
							Save(dialogResult.Path);
						}
					}

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
					if (ImGui.MenuItem("Shortcuts"))
						_showShortcutsWindow = true;

					ImGui.EndMenu();
				}

				ImGui.EndMenuBar();
			}

			const int leftWidth = 256;
			const int rightWidth = 512;
			float middleWidth = viewportSize.X - leftWidth - rightWidth;

			const int bottomHeight = 448;
			float levelEditorHeight = viewportSize.Y - bottomHeight;

			if (ImGui.BeginChild("Left", new(leftWidth, 0)))
			{
				const int levelInfoHeight = 192;
				const int padding = 4; // TODO: Get from ImGui style.
				LevelInfoWindow.Render(new(leftWidth, levelInfoHeight));
				LevelAssetsWindow.Render(new(leftWidth, viewportSize.Y - bottomHeight - levelInfoHeight - padding));
				DebugWindow.Render(new(leftWidth, 0));
			}

			ImGui.EndChild(); // End Left

			ImGui.SameLine();

			if (ImGui.BeginChild("Middle", new(middleWidth, 0)))
			{
				LevelEditorWindow.Render(new(middleWidth, levelEditorHeight));
				ObjectCreatorWindow.Render(new(middleWidth, 0));
			}

			ImGui.EndChild(); // End Middle

			ImGui.SameLine();

			if (ImGui.BeginChild("Right", new(0, 0)))
			{
				ObjectEditorWindow.Render(new(0, 0));
			}

			ImGui.EndChild(); // End Right
		}

		ImGui.End(); // End 3D Level Editor
	}

	private static void Load(string path)
	{
		using FileStream fs = new(path, FileMode.Open);
		using XmlReader reader = XmlReader.Create(fs);
		Level3dData level = XmlFormatSerializer.ReadLevel(reader);

		LevelState.SetLevel(path, level);
	}

	private static void Save(string path)
	{
		using MemoryStream ms = new();
		using XmlWriter writer = XmlWriter.Create(ms, _xmlWriterSettings);
		XmlFormatSerializer.WriteLevel(LevelState.Level, writer);
		writer.Flush();

		ms.Write("\n"u8);

		File.WriteAllBytes(path, ms.ToArray());
		LevelState.SetLevel(path, LevelState.Level);
	}
}
