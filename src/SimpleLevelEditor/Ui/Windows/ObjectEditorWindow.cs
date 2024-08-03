using ImGuiNET;
using SimpleLevelEditor.State.States.LevelEditor;
using SimpleLevelEditor.Ui.ChildWindows;

namespace SimpleLevelEditor.Ui.Windows;

public static class ObjectEditorWindow
{
	public static void Render()
	{
		if (ImGui.Begin("Object Editor"))
		{
			Vector4 activeButton = new(0.3f, 0.3f, 0.3f, 1);

			ImGui.PushStyleColor(ImGuiCol.Button, LevelEditorState.Mode == LevelEditorState.EditMode.WorldObjects ? activeButton : default);
			if (ImGui.Button("World objects"))
				LevelEditorState.Mode = LevelEditorState.EditMode.WorldObjects;

			ImGui.PopStyleColor();

			ImGui.SameLine();
			ImGui.PushStyleColor(ImGuiCol.Button, LevelEditorState.Mode == LevelEditorState.EditMode.Entities ? activeButton : default);
			if (ImGui.Button("Entities"))
				LevelEditorState.Mode = LevelEditorState.EditMode.Entities;

			ImGui.PopStyleColor();

			if (LevelEditorState.Mode == LevelEditorState.EditMode.WorldObjects)
				WorldObjectEditorWindow.Render();
			else if (LevelEditorState.Mode == LevelEditorState.EditMode.Entities)
				EntityEditorWindow.Render();
		}

		ImGui.End();
	}
}
