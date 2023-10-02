using ImGuiNET;

namespace SimpleLevelEditor.Ui;

public static class ShortcutsWindow
{
	public static void Render(ref bool showWindow)
	{
		ImGui.SetNextWindowSize(new(384, 160));
		if (ImGui.Begin("Shortcuts", ref showWindow, ImGuiWindowFlags.NoResize))
		{
			if (ImGui.BeginTable("Table", 2))
			{
				ImGui.TableSetupColumn("Shortcut", ImGuiTableColumnFlags.WidthFixed, 256);
				ImGui.TableSetupColumn("Keys", ImGuiTableColumnFlags.WidthFixed);
				ImGui.TableHeadersRow();

				ImGui.TableNextRow();
				for (int i = 0; i < Shortcuts.ShortcutsList.Count; i++)
				{
					Shortcut shortcut = Shortcuts.ShortcutsList[i];

					ImGui.TableNextColumn();
					ImGui.Text(shortcut.Description);

					ImGui.TableNextColumn();
					ImGui.Text(Inline.Span(shortcut.KeyDescription));
				}

				ImGui.EndTable();
			}
		}

		ImGui.End(); // End Shortcuts
	}
}
