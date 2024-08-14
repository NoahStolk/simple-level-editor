using ImGuiNET;
using NativeFileDialogUtils;
using SimpleLevelEditorV2.States.EntityConfigEditor;

namespace SimpleLevelEditorV2.Ui.EntityConfig;

public sealed class EntityConfigTexturesWindow
{
	public void Render(EntityConfigEditorState state, Vector2 initialWindowSize)
	{
		ImGuiIOPtr io = ImGui.GetIO();
		Vector2 screenCenter = new(io.DisplaySize.X / 2, io.DisplaySize.Y / 2);
		ImGui.SetNextWindowPos(screenCenter - initialWindowSize with { X = 0 }, ImGuiCond.Appearing);
		ImGui.SetNextWindowSize(initialWindowSize, ImGuiCond.Appearing);
		if (ImGui.Begin("Textures"))
		{
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

		ImGui.End();
	}
}
