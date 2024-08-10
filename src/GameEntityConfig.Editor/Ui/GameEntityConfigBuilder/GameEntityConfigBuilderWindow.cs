using GameEntityConfig.Editor.States;
using ImGuiNET;

namespace GameEntityConfig.Editor.Ui.GameEntityConfigBuilder;

public sealed class GameEntityConfigBuilderWindow
{
	private readonly GameEntityConfigBuilderState _state = new();

	private readonly ModelsChild _modelsChild = new();
	private readonly TexturesChild _texturesChild = new();
	private readonly ComponentsChild _componentsChild = new();
	private readonly EntityDescriptorsChild _entityDescriptorsChild = new();

	public void Render()
	{
		if (ImGui.Begin("Game Entity Config Builder", ImGuiWindowFlags.MenuBar))
		{
			if (ImGui.BeginMenuBar())
			{
				if (ImGui.BeginMenu("File"))
				{
					if (ImGui.MenuItem("Save"))
					{
						GameEntityConfig.GameEntityConfigBuilder builder = new();
					}

					// if (ImGui.MenuItem("Load"))
					// 	_gameEntityConfigBuilder.Load(_name);
					ImGui.EndMenu();
				}

				ImGui.EndMenuBar();
			}

			ImGui.InputText("Game Name", ref _state.Name, 100);

			_modelsChild.Render(_state);
			_texturesChild.Render(_state);
			_componentsChild.Render(_state);
			_entityDescriptorsChild.Render(_state);
		}

		ImGui.End();
	}
}
