using Detach;
using ImGuiNET;
using SimpleLevelEditor.State;

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
				ImGui.Separator();
				ImGui.Text(Inline.Span($"Version: {LevelState.Level.Version}"));
				ImGui.Text(Inline.Span($"Meshes: {LevelState.Level.Meshes.Count}"));
				ImGui.Text(Inline.Span($"Textures: {LevelState.Level.Textures.Count}"));
				ImGui.Text(Inline.Span($"WorldObjects: {LevelState.Level.WorldObjects.Count}"));
				ImGui.Text(Inline.Span($"Entities: {LevelState.Level.Entities.Count}"));
			}
		}

		ImGui.End();
	}
}
