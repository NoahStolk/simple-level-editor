using ImGuiNET;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Ui.Components;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class ObjectCreatorWindow
{
	public static void Render(Vector2 size)
	{
		if (ImGui.BeginChild("Object Creator", size, true))
		{
			ImGui.SeparatorText("Object Creator");

			if (ImGui.BeginTabBar("Creators"))
			{
				if (ImGui.BeginTabItem("World Object"))
				{
					ImGui.SeparatorText("World Object");

					AssetTiles.Render(LevelState.Level.Meshes, ObjectCreatorState.SelectedMeshName, s => ObjectCreatorState.SelectedMeshName = s, new(160), size);

					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Entity"))
				{
					ImGui.SeparatorText("Entity");

					ImGui.EndTabItem();
				}

				ImGui.EndTabBar();
			}
		}

		ImGui.EndChild(); // End Object Creator
	}
}
