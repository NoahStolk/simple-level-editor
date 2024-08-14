using ImGuiNET;

namespace SimpleLevelEditorV2.Ui.Shortcuts;

public sealed class ShortcutsWindow
{
	public void Render(ref bool showWindow, SimpleLevelEditorV2.Shortcuts shortcuts)
	{
		if (!showWindow)
			return;

		ImGui.SetNextWindowSize(new Vector2(512, 384));
		if (ImGui.Begin("Shortcuts", ref showWindow, ImGuiWindowFlags.NoResize))
		{
			if (ImGui.BeginTable("Table", 2))
			{
				ImGui.TableSetupColumn("Shortcut", ImGuiTableColumnFlags.WidthFixed, 256);
				ImGui.TableSetupColumn("Keys", ImGuiTableColumnFlags.None);
				ImGui.TableHeadersRow();

				ImGui.TableNextRow();
				for (int i = 0; i < shortcuts.ShortcutsList.Count; i++)
				{
					Shortcut shortcut = shortcuts.ShortcutsList[i];

					ImGui.TableNextColumn();
					ImGui.Text(shortcut.Description);

					ImGui.TableNextColumn();
					ImGui.Text(shortcut.KeyDescription);
				}

				ImGui.EndTable();
			}
		}

		ImGui.End();
	}
}
