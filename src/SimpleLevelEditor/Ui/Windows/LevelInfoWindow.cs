using Detach;
using ImGuiNET;
using SimpleLevelEditor.Formats.Level;
using SimpleLevelEditor.State.States.EntityConfig;
using SimpleLevelEditor.State.States.Level;
using SimpleLevelEditor.Ui.ChildWindows;

namespace SimpleLevelEditor.Ui.Windows;

public static class LevelInfoWindow
{
	public static void Render()
	{
		if (ImGui.Begin("Level Info"))
		{
			ImGui.TextWrapped(LevelState.LevelFilePath ?? "<No level loaded>");
			if (LevelState.LevelFilePath != null)
			{
				ImGui.Text(LevelState.IsModified ? "(unsaved changes)" : "(saved)");
				ImGui.SeparatorText("Level");
				RenderLevel(LevelState.Level);
			}
		}

		ImGui.End();
	}

	private static void RenderLevel(Level3dData level)
	{
		ImGui.Text(Inline.Span($"Models: {level.ModelPaths.Count}"));
		ImGui.Text(Inline.Span($"WorldObjects: {level.WorldObjects.Count}"));
		ImGui.Text(Inline.Span($"Entities: {level.Entities.Count}"));
		ImGui.TextWrapped(Inline.Span($"EntityConfig: {level.EntityConfigPath ?? "<No entity config loaded>"}"));
		ImGui.SeparatorText("Entity config");
		if (level.EntityConfigPath != null)
		{
			EntityConfigTreeNodes.Render(EntityConfigState.EntityConfig);
		}
	}
}
