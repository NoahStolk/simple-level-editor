using ImGuiNET;
using SimpleLevelEditorV2.States.GameEntityConfigBuilder;

namespace SimpleLevelEditorV2.Ui.GameEntityConfigBuilder;

public sealed class GameEntityConfigBuilderWindow
{
	private readonly ModelsChild _modelsChild = new();
	private readonly TexturesChild _texturesChild = new();
	private readonly DataTypesChild _dataTypesChild = new();
	private readonly EntityDescriptorsChild _entityDescriptorsChild = new();

	public void Render(GameEntityConfigBuilderState gameEntityConfigBuilderState)
	{
		if (ImGui.Begin("Game Entity Config Builder"))
		{
			_modelsChild.Render(gameEntityConfigBuilderState);
			_texturesChild.Render(gameEntityConfigBuilderState);
			_dataTypesChild.Render(gameEntityConfigBuilderState);
			_entityDescriptorsChild.Render(gameEntityConfigBuilderState);
		}

		ImGui.End();
	}
}
