using Editor.States.GameEntityConfigBuilder;
using ImGuiNET;
using NativeFileDialogUtils;

namespace Editor.Ui.GameEntityConfigBuilder;

public sealed class TexturesChild
{
	public void Render(GameEntityConfigBuilderState state)
	{
		ImGui.SeparatorText("Textures");

		for (int i = state.TexturePaths.Count - 1; i >= 0; i--)
		{
			string texturePath = state.TexturePaths[i];
			if (ImGui.Button($"X##{texturePath}"))
				state.TexturePaths.Remove(texturePath);
			ImGui.SameLine();
			ImGui.Text(texturePath);
		}

		if (ImGui.Button("Add Texture"))
		{
			DialogWrapper.FileOpenMultiple(
				p =>
				{
					if (p != null)
						state.TexturePaths.AddRange(p);
				},
				FileConstants.TextureFormats);
		}
	}
}
