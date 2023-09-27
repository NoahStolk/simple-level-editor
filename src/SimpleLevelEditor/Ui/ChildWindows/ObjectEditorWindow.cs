using ImGuiNET;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class ObjectEditorWindow
{
	public static void Render(Vector2 size)
	{
		if (ImGui.BeginChild("Object Editor", size, true))
		{
			ImGui.SeparatorText("Object Editor");
		}

		ImGui.EndChild(); // End Object Editor
	}
}
