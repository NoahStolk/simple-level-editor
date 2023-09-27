using ImGuiNET;

namespace SimpleLevelEditor.Ui;

public static class Style
{
	public static void Initialize()
	{
		ImGuiStylePtr style = ImGui.GetStyle();
		style.WindowPadding = new(4, 4);
		style.ItemSpacing = new(4, 4);
	}
}
