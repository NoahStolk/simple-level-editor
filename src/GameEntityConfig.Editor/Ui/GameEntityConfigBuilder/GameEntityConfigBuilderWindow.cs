using GameEntityConfig.Core;
using GameEntityConfig.Editor.States;
using ImGuiNET;
using NativeFileDialogUtils;

namespace GameEntityConfig.Editor.Ui.GameEntityConfigBuilder;

public sealed class GameEntityConfigBuilderWindow
{
	private readonly GameEntityConfigBuilderState _state = new();

	private readonly ModelsChild _modelsChild = new();
	private readonly TexturesChild _texturesChild = new();
	private readonly DataTypesChild _dataTypesChild = new();
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
						foreach (string modelPath in _state.ModelPaths)
							builder = builder.WithModelPath(modelPath);
						foreach (string texturePath in _state.TexturePaths)
							builder = builder.WithTexturePath(texturePath);
						foreach (DataType dataType in _state.DataTypes)
							builder = builder.WithDataType(dataType);
						foreach (EntityDescriptor entityDescriptor in _state.EntityDescriptors)
							builder = builder.WithEntityDescriptor(entityDescriptor);

						Core.GameEntityConfig config = builder.Build();
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

					if (ImGui.MenuItem("Load"))
					{
						DialogWrapper.FileOpen(
							s =>
							{
								if (s == null)
									return;

								string json = File.ReadAllText(s);
								Core.GameEntityConfig config = GameEntityConfigSerializer.Deserialize(json);
								_state.ModelPaths = config.ModelPaths.ToList();
								_state.TexturePaths = config.TexturePaths.ToList();
								_state.DataTypes = config.DataTypes.ToList();
								_state.EntityDescriptors = config.EntityDescriptors.ToList();
							},
							"json");
					}

					ImGui.EndMenu();
				}

				ImGui.EndMenuBar();
			}

			_modelsChild.Render(_state);
			_texturesChild.Render(_state);
			_dataTypesChild.Render(_state);
			_entityDescriptorsChild.Render(_state);
		}

		ImGui.End();
	}
}
