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
				WorldObjectDataComponent.Render(ObjectEditorState.SelectedWorldObject, true);
			}
			else
			{
				ImGui.Text("None selected.");
			}
		}

		ImGui.EndChild(); // End Object Editor
	}
}
