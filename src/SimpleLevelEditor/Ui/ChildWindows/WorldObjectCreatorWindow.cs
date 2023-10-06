using ImGuiNET;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Ui.Components;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class WorldObjectCreatorWindow
{
	public static void Render(Vector2 size)
	{
		if (ImGui.BeginChild("Create World Object", size, true))
		{
			ImGui.SeparatorText("Create World Object");

			WorldObjectDataComponent.Render(ObjectCreatorState.NewWorldObject, false);
		}

		ImGui.EndChild(); // End Object Creator
	}
}
