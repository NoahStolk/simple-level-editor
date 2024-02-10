using Detach;
using Detach.Numerics;
using ImGuiNET;
using SimpleLevelEditor.State;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class DebugWindow
{
	public static void Render(Vector2 size)
	{
		if (ImGui.BeginChild("Warnings", size, ImGuiChildFlags.Border))
		{
			ImGui.SeparatorText("Warnings");

			ImGui.BeginDisabled(DebugState.Warnings.Count == 0);
			if (ImGui.Button("Clear"))
				DebugState.ClearWarnings();

			ImGui.EndDisabled();

			if (ImGui.BeginChild("WarningsList"))
			{
				if (DebugState.Warnings.Count > 0)
				{
					foreach (KeyValuePair<string, int> kvp in DebugState.Warnings)
						ImGui.TextWrapped(Inline.Span($"{kvp.Key}: {kvp.Value}"));
				}
				else
				{
					ImGui.TextColored(Color.Green, "No warnings");
				}
			}

			ImGui.EndChild();
		}

		ImGui.EndChild();
	}
}
