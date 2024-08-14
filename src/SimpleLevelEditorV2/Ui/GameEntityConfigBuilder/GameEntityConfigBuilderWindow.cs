using ImGuiNET;
using NativeFileDialogUtils;
using SimpleLevelEditorV2.Formats.GameEntityConfig;
using SimpleLevelEditorV2.Formats.GameEntityConfig.Model;
using SimpleLevelEditorV2.States.App;
using SimpleLevelEditorV2.States.GameEntityConfigBuilder;

namespace SimpleLevelEditorV2.Ui.GameEntityConfigBuilder;

public sealed class GameEntityConfigBuilderWindow
{
	private readonly ModelsChild _modelsChild = new();
	private readonly TexturesChild _texturesChild = new();
	private readonly DataTypesChild _dataTypesChild = new();
	private readonly EntityDescriptorsChild _entityDescriptorsChild = new();

	public void Render(AppState appState, GameEntityConfigBuilderState gameEntityConfigBuilderState)
	{
		if (ImGui.Begin("Game Entity Config Builder", ImGuiWindowFlags.MenuBar))
		{
			if (ImGui.BeginMenuBar())
			{
				if (ImGui.BeginMenu("File"))
				{
					if (ImGui.MenuItem("New"))
					{
						gameEntityConfigBuilderState.ModelPaths.Clear();
						gameEntityConfigBuilderState.TexturePaths.Clear();
						gameEntityConfigBuilderState.DataTypes.Clear();
						gameEntityConfigBuilderState.EntityDescriptors.Clear();
					}

					if (ImGui.MenuItem("Open"))
					{
						DialogWrapper.FileOpen(
							s =>
							{
								if (s == null)
									return;

								string json = File.ReadAllText(s);
								GameEntityConfigModel config = GameEntityConfigSerializer.Deserialize(json);
								gameEntityConfigBuilderState.ModelPaths = config.ModelPaths.ToList();
								gameEntityConfigBuilderState.TexturePaths = config.TexturePaths.ToList();
								gameEntityConfigBuilderState.DataTypes = config.DataTypes.ToList();
								gameEntityConfigBuilderState.EntityDescriptors = config.EntityDescriptors.ToList();
							},
							"json");
					}

					if (ImGui.MenuItem("Save"))
					{
						SimpleLevelEditorV2.Formats.GameEntityConfig.GameEntityConfigBuilder builder = new();
						foreach (string modelPath in gameEntityConfigBuilderState.ModelPaths)
							builder = builder.WithModelPath(modelPath);
						foreach (string texturePath in gameEntityConfigBuilderState.TexturePaths)
							builder = builder.WithTexturePath(texturePath);
						foreach (DataType dataType in gameEntityConfigBuilderState.DataTypes)
							builder = builder.WithDataType(dataType);
						foreach (EntityDescriptor entityDescriptor in gameEntityConfigBuilderState.EntityDescriptors)
							builder = builder.WithEntityDescriptor(entityDescriptor);

						GameEntityConfigModel config = builder.Build();
						DialogWrapper.FileSave(
							s =>
							{
								if (s == null)
									return;

								string json = GameEntityConfigSerializer.Serialize(config);
								File.WriteAllText(Path.ChangeExtension(s, ".json"), json);
							},
							"json");
					}

					ImGui.Separator();

					if (ImGui.MenuItem("Exit"))
					{
						appState.CurrentView = AppView.Main;
					}

					ImGui.EndMenu();
				}

				ImGui.EndMenuBar();
			}

			_modelsChild.Render(gameEntityConfigBuilderState);
			_texturesChild.Render(gameEntityConfigBuilderState);
			_dataTypesChild.Render(gameEntityConfigBuilderState);
			_entityDescriptorsChild.Render(gameEntityConfigBuilderState);
		}

		ImGui.End();
	}
}
