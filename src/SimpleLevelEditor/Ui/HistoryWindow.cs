using ImGuiNET;
using SimpleLevelEditor.State;

namespace SimpleLevelEditor.Ui;

public static class HistoryWindow
{
	public static void Render()
	{
		if (ImGui.Begin("History"))
		{
			for (int i = 0; i < LevelState.History.Count; i++)
			{
				bool isCurrent = i == LevelState.CurrentHistoryIndex;
				LevelState.HistoryEntry entry = LevelState.History[i];
				ImGui.TextColored(isCurrent ? new(0, 1, 0, 1) : Vector4.One, entry.EditDescription);
			}
		}

		ImGui.End();
	}
}
