using ImGuiNET;

namespace GameEntityConfig.Editor.Ui.GameEntityConfigBuilder;

public sealed class GameEntityConfigBuilderWindow
{
	private GameEntityConfig.GameEntityConfigBuilder _gameEntityConfigBuilder = new();

	private string _name = string.Empty;

	private readonly ModelsChild _modelsChild = new();
	private readonly TexturesChild _texturesChild = new();
	private readonly ComponentsChild _componentsChild = new();
	private readonly EntityDescriptorsChild _entityDescriptorsChild = new();

	public void Render()
	{
		if (ImGui.Begin("Game Entity Config Builder"))
		{
			ImGui.InputText("Game Name", ref _name, 100);

			_modelsChild.Render();
			_texturesChild.Render();
			_componentsChild.Render();
			_entityDescriptorsChild.Render();
		}

		ImGui.End();
	}
}
