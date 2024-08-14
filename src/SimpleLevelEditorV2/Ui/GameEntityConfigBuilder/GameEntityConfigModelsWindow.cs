using ImGuiNET;
using NativeFileDialogUtils;
using SimpleLevelEditorV2.States.GameEntityConfigBuilder;

namespace SimpleLevelEditorV2.Ui.GameEntityConfigBuilder;

public sealed class GameEntityConfigModelsWindow
{
	public void Render(GameEntityConfigBuilderState state, Vector2 initialWindowSize)
	{
		ImGuiIOPtr io = ImGui.GetIO();
		Vector2 screenCenter = new(io.DisplaySize.X / 2, io.DisplaySize.Y / 2);
		ImGui.SetNextWindowPos(screenCenter - initialWindowSize, ImGuiCond.Appearing);
		ImGui.SetNextWindowSize(initialWindowSize, ImGuiCond.Appearing);
		if (ImGui.Begin("Models"))
		{
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

		ImGui.End();
	}
}
