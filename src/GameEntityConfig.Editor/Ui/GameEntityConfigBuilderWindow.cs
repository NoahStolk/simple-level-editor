using ImGuiNET;

namespace GameEntityConfig.Editor.Ui;

public sealed class GameEntityConfigBuilderWindow
{
	public void Render()
	{
		if (ImGui.Begin("Game Entity Config Builder"))
		{
			// Render game name.
			// Render model paths.
			// Render texture paths.
			// Render component types.
			// Render entity descriptors.
		}

		ImGui.End();
	}
}
