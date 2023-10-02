using ImGuiNET;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Ui.ChildWindows;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Ui;

public static class MainWindow
{
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
					if (ImGui.MenuItem("Shortcuts"))
						_showShortcutsWindow = true;

					ImGui.EndMenu();
				}

				ImGui.EndMenuBar();
			}

			const int leftWidth = 256;
			const int rightWidth = 512;
			const int bottomHeight = 448;
			float middleWidth = viewportSize.X - leftWidth - rightWidth;

			if (ImGui.BeginChild("Left", new(leftWidth, 0)))
			{
				const int levelInfoHeight = 192;
				LevelInfoWindow.Render(new(leftWidth, levelInfoHeight));
				LevelAssetsWindow.Render(new(leftWidth, viewportSize.Y - bottomHeight - levelInfoHeight));
				DebugWindow.Render(new(leftWidth, 0));
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
				if (ImGui.BeginChild("Mode", new(0, 32), true))
				{
					for (int i = 0; i < EnumUtils.LevelEditorModeArray.Count; i++)
					{
						if (i > 0)
							ImGui.SameLine();

						LevelEditorMode mode = EnumUtils.LevelEditorModeArray[i];
						ImGui.PushStyleColor(ImGuiCol.Button, mode == LevelEditorState.Mode ? new Vector4(0.5f, 0.2f, 0.2f, 1) : new(0.1f, 0.1f, 0.1f, 1));

						// TODO: Use images instead of a single letter.
						if (ImGui.Button(Inline.Span(EnumUtils.LevelEditorModeNames[mode][0]), new(24)))
							LevelEditorState.Mode = mode;

						if (ImGui.IsItemHovered(ImGuiHoveredFlags.DelayNone))
						{
							string shortcut = EnumUtils.LevelEditorModeShortcuts[mode];
							ImGui.SetTooltip(Inline.Span($"{Shortcuts.GetDescription(shortcut)} - shortcut: {Shortcuts.GetKeyDescription(shortcut)}"));
						}

						ImGui.PopStyleColor();
					}
				}

				ImGui.EndChild(); // End Mode

				switch (LevelEditorState.Mode)
				{
					case LevelEditorMode.AddWorldObjects:
						WorldObjectCreatorWindow.Render(new(0, 0));
						break;
					case LevelEditorMode.EditWorldObjects:
						ObjectEditorWindow.Render(new(0, 0));
						break;
				}
			}

			ImGui.EndChild(); // End Right
		}

		ImGui.End(); // End 3D Level Editor
	}
}
