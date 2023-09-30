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

					Vector2 tileSize = new(160);
					int rowLength = (int)MathF.Floor(size.X / tileSize.X);
					for (int i = 0; i < LevelState.Level.Meshes.Count; i++)
					{
						if (i % rowLength != 0)
							ImGui.SameLine();

						string meshName = LevelState.Level.Meshes[i];
						if (ImGui.Selectable(meshName, ObjectCreatorState.SelectedMeshName == meshName, ImGuiSelectableFlags.None, tileSize))
							ObjectCreatorState.SelectedMeshName = meshName;
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
