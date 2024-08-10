using ImGuiNET;
using NativeFileDialogUtils;

namespace GameEntityConfig.Editor.Ui.GameEntityConfigBuilder;

public sealed class ModelsChild
{
	private List<string> _modelPaths = [];

	public IReadOnlyList<string> ModelPaths => _modelPaths;

	public void Render()
	{
		ImGui.SeparatorText("Models");

		for (int i = _modelPaths.Count - 1; i >= 0; i--)
		{
			string modelPath = _modelPaths[i];
			if (ImGui.Button($"X##{modelPath}"))
				_modelPaths.Remove(modelPath);
			ImGui.SameLine();
			ImGui.Text(modelPath);
		}

		if (ImGui.Button("Add Model"))
			DialogWrapper.FileOpenMultiple(AddModelPaths, FileConstants.ModelFormats);
	}

	private void AddModelPaths(IReadOnlyList<string>? modelPaths)
	{
		if (modelPaths != null)
			_modelPaths.AddRange(modelPaths);

		_modelPaths = _modelPaths.Distinct().ToList();
	}
}
