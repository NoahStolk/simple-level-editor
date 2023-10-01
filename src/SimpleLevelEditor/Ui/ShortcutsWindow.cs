using ImGuiNET;
using SimpleLevelEditor.Extensions;

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
				foreach (KeyValuePair<string, Shortcut> shortcut in Shortcuts.ShortcutsDictionary)
				{
					ImGui.TableNextColumn();
					ImGui.Text(shortcut.Key);

					ImGui.TableNextColumn();
					ImGui.Text(Inline.Span($"{(shortcut.Value.Ctrl ? "CTRL " : string.Empty)}{(shortcut.Value.Shift ? "SHIFT " : string.Empty)}{shortcut.Value.Key.GetChar(false) ?? '?'}"));
				}

				ImGui.EndTable();
			}
		}

		ImGui.End(); // End Shortcuts
	}
}
