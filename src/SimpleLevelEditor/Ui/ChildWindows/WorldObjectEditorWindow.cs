using ImGuiNET;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Ui.Components;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class WorldObjectEditorWindow
{
	public static void Render(Vector2 size)
	{
		if (ImGui.BeginChild("Edit World Object", size, true))
		{
			ImGui.SeparatorText("Edit World Object");

			if (ObjectEditorState.SelectedWorldObject != null)
			{
				WorldObjectDataComponent.Render(size, ObjectEditorState.SelectedWorldObject, true);

				ImGui.Separator();

				ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 0, 0, 1));
				if (ImGui.Button("Delete"))
				{
					LevelState.Level.WorldObjects.Remove(ObjectEditorState.SelectedWorldObject);
					ObjectEditorState.SelectedWorldObject = null;
				}

				ImGui.PopStyleColor();
			}
			else
			{
				ImGui.Text("None selected.");
			}
		}

		ImGui.EndChild(); // End Object Editor
	}
}
