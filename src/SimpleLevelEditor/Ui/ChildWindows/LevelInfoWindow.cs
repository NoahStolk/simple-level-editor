using Detach;
using ImGuiNET;
using SimpleLevelEditor.State;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class LevelInfoWindow
{
	public static void Render(Vector2 size)
	{
		if (ImGui.BeginChild("Level Info", size, true))
		{
			ImGui.SeparatorText("Level Info");

			ImGui.TextWrapped(LevelState.LevelFilePath ?? "<None>");
			ImGui.Text(LevelState.LevelFilePath == null ? string.Empty : LevelState.IsModified ? "(unsaved changes)" : "(saved)");
			ImGui.Separator();
			ImGui.Text(Inline.Span($"Version: {LevelState.Level.Version}"));
			ImGui.Text(Inline.Span($"Meshes: {LevelState.Level.Meshes.Count}"));
			ImGui.Text(Inline.Span($"Textures: {LevelState.Level.Textures.Count}"));
			ImGui.Text(Inline.Span($"WorldObjects: {LevelState.Level.WorldObjects.Count}"));
			ImGui.Text(Inline.Span($"Entities: {LevelState.Level.Entities.Count}"));
		}

		ImGui.EndChild(); // End LevelInfo
	}
}
