using Detach;
using ImGuiNET;
using SimpleLevelEditor.Formats.Types.Level;
using SimpleLevelEditor.State;
using SimpleLevelEditor.State.States.EntityConfig;
using SimpleLevelEditor.State.States.Level;
using SimpleLevelEditor.Ui.ChildWindows;

namespace SimpleLevelEditor.Ui;

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
		ImGui.Text(Inline.Span($"Models: {level.ModelPaths.Length}"));
		ImGui.Text(Inline.Span($"WorldObjects: {level.WorldObjects.Length}"));
		ImGui.Text(Inline.Span($"Entities: {level.Entities.Length}"));
		ImGui.TextWrapped(Inline.Span($"EntityConfig: {(level.EntityConfigPath == null ? "<No entity config loaded>" : level.EntityConfigPath.Value)}"));
		ImGui.SeparatorText("Entity config");
		if (level.EntityConfigPath != null)
		{
			EntityConfigTreeNodes.Render(EntityConfigState.EntityConfig);
		}
	}
}
