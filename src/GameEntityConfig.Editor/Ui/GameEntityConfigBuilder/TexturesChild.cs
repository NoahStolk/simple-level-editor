using ImGuiNET;
using NativeFileDialogUtils;

namespace GameEntityConfig.Editor.Ui.GameEntityConfigBuilder;

public sealed class TexturesChild
{
	private List<string> _texturePaths = [];

	public IReadOnlyList<string> TexturePaths => _texturePaths;

	public void Render()
	{
		ImGui.SeparatorText("Textures");

		for (int i = _texturePaths.Count - 1; i >= 0; i--)
		{
			string texturePath = _texturePaths[i];
			if (ImGui.Button($"X##{texturePath}"))
				_texturePaths.Remove(texturePath);
			ImGui.SameLine();
			ImGui.Text(texturePath);
		}

		if (ImGui.Button("Add Texture"))
			DialogWrapper.FileOpenMultiple(AddTexturePaths, FileConstants.TextureFormats);
	}

	private void AddTexturePaths(IReadOnlyList<string>? texturePaths)
	{
		if (texturePaths != null)
			_texturePaths.AddRange(texturePaths);

		_texturePaths = _texturePaths.Distinct().ToList();
	}
}
