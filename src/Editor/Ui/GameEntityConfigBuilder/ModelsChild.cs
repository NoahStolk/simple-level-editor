using Editor.States.GameEntityConfigBuilder;
using ImGuiNET;
using NativeFileDialogUtils;

namespace Editor.Ui.GameEntityConfigBuilder;

public sealed class ModelsChild
{
	public void Render(GameEntityConfigBuilderState state)
	{
		ImGui.SeparatorText("Models");

		for (int i = state.ModelPaths.Count - 1; i >= 0; i--)
		{
			string modelPath = state.ModelPaths[i];
			if (ImGui.Button($"X##{modelPath}"))
				state.ModelPaths.Remove(modelPath);
			ImGui.SameLine();
			ImGui.Text(modelPath);
		}

		if (ImGui.Button("Add Model"))
		{
			DialogWrapper.FileOpenMultiple(
				p =>
				{
					if (p != null)
						state.ModelPaths.AddRange(p);
				},
				FileConstants.ModelFormats);
		}
	}
}
