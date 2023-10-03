using ImGuiNET;

namespace SimpleLevelEditor.Ui.Components;

public static class AssetTilesComponent
{
	public static void Render(IReadOnlyList<string> items, ref string selectedItem)
	{
		const int rowLength = 4;

		if (ImGui.BeginTable("Grid", rowLength))
		{
			for (int i = 0; i < items.Count; i++)
			{
				if (i % rowLength == 0)
					ImGui.TableNextRow();

				ImGui.TableNextColumn();
				string meshName = items[i];

				if (ImGui.Selectable(Inline.Span(meshName), selectedItem == meshName, ImGuiSelectableFlags.None, new(0, 128)))
					selectedItem = meshName;
			}

			ImGui.EndTable();
		}
	}
}
