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
	private static bool _showDemoWindow;

	public static void Render()
	{
		if (_showDemoWindow)
			ImGui.ShowDemoWindow(ref _showDemoWindow);

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
						DialogResult dialogResult = Dialog.FileSave("bin");
						if (dialogResult is { IsOk: true })
						{
							using MemoryStream ms = new();
							using BinaryWriter bw = new(ms);

							Level3dData level = Level3dData.Default.DeepCopy();

							BinaryFormatSerializer.WriteLevel(level, bw);
							File.WriteAllBytes(dialogResult.Path, ms.ToArray());

							LevelState.SetLevel(dialogResult.Path, level);
						}
					}

					const SerializerKind serializerKind = SerializerKind.Xml;
					if (ImGui.MenuItem("Open"))
					{
						DialogResult dialogResult = Dialog.FileOpen(GetFileExtension(serializerKind));
						if (dialogResult is { IsOk: true })
						{
							Load(serializerKind, dialogResult.Path);
						}
					}

					if (ImGui.MenuItem("Save"))
					{
						if (LevelState.LevelFilePath != null)
						{
							Save(serializerKind, LevelState.LevelFilePath);
						}
						else
						{
							DialogResult dialogResult = Dialog.FileSave(GetFileExtension(serializerKind));
							if (dialogResult is { IsOk: true })
							{
								Save(serializerKind, dialogResult.Path);
							}
						}
					}

					if (ImGui.MenuItem("Save As"))
					{
						DialogResult dialogResult = Dialog.FileSave(GetFileExtension(serializerKind));
						if (dialogResult is { IsOk: true })
						{
							Save(serializerKind, dialogResult.Path);
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

				ImGui.EndMenuBar();
			}

			const int leftWidth = 256;
			const int rightWidth = 512;
			float middleWidth = viewportSize.X - leftWidth - rightWidth;

			const int debugHeight = 256;
			const int objectCreatorHeight = 384;
			float levelEditorHeight = viewportSize.Y - objectCreatorHeight;

			if (ImGui.BeginChild("Left", new(leftWidth, 0)))
			{
				DebugWindow.Render(new(leftWidth, debugHeight));
				LevelInfoWindow.Render(new(leftWidth, 0));
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

	private static void Load(SerializerKind serializerKind, string path)
	{
		using FileStream fs = new(path, FileMode.Open);

		Level3dData level;
		switch (serializerKind)
		{
			case SerializerKind.Binary:
			{
				using BinaryReader br = new(fs);
				level = BinaryFormatSerializer.ReadLevel(br);
				break;
			}

			case SerializerKind.Xml:
			{
				using XmlReader reader = XmlReader.Create(fs);
				level = XmlFormatSerializer.ReadLevel(reader);
				break;
			}

			default:
				throw new ArgumentOutOfRangeException(nameof(serializerKind), serializerKind, null);
		}

		LevelState.SetLevel(path, level);
	}

	private static void Save(SerializerKind serializerKind, string path)
	{
		using MemoryStream ms = new();

		switch (serializerKind)
		{
			case SerializerKind.Xml:
			{
				using XmlWriter writer = XmlWriter.Create(ms, new() { Indent = true, Encoding = new UTF8Encoding(false) });
				XmlFormatSerializer.WriteLevel(LevelState.Level, writer);
				writer.Flush();

				ms.Write("\n"u8);
				break;
			}

			case SerializerKind.Binary:
			{
				using BinaryWriter bw = new(ms);
				BinaryFormatSerializer.WriteLevel(LevelState.Level, bw);
				break;
			}
		}

		File.WriteAllBytes(path, ms.ToArray());
		LevelState.SetLevel(path, LevelState.Level);
	}

	private static string GetFileExtension(SerializerKind serializerKind)
	{
		return serializerKind switch
		{
			SerializerKind.Xml => "xml",
			SerializerKind.Binary => "bin",
			_ => throw new ArgumentOutOfRangeException(nameof(serializerKind), serializerKind, null),
		};
	}
}
