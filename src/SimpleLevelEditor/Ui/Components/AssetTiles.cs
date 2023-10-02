using ImGuiNET;

namespace SimpleLevelEditor.Ui.Components;

public static class AssetTiles
{
	public static void Render(IReadOnlyList<string> items, ref string selectedItem, Vector2 tileSize, Vector2 totalSize)
	{
		int rowLength = (int)Math.Max(1, MathF.Floor(totalSize.X / tileSize.X));
		for (int i = 0; i < items.Count; i++)
		{
			if (i % rowLength != 0)
				ImGui.SameLine();

			string meshName = items[i];
			if (ImGui.Selectable(meshName, selectedItem == meshName, ImGuiSelectableFlags.None, tileSize))
				selectedItem = meshName;
		}
	}
}
