using ImGuiNET;
using SimpleLevelEditor.State;

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

					const int rowLength = 7;
					Vector2 tileSize = new(160);
					for (int i = 0; i < LevelState.Level.Meshes.Count; i++)
					{
						string meshName = LevelState.Level.Meshes[i];
						if (ImGui.Selectable(meshName, ObjectCreatorState.SelectedMeshName == meshName, ImGuiSelectableFlags.None, tileSize))
							ObjectCreatorState.SelectedMeshName = meshName;

						if (i == 0 || i % (rowLength - 1) != 0)
							ImGui.SameLine();
					}

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
