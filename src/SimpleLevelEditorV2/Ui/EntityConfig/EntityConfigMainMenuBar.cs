using ImGuiNET;
using NativeFileDialogUtils;
using SimpleLevelEditorV2.Formats.EntityConfig;
using SimpleLevelEditorV2.Formats.EntityConfig.Model;
using SimpleLevelEditorV2.States.App;
using SimpleLevelEditorV2.States.EntityConfigEditor;

namespace SimpleLevelEditorV2.Ui.EntityConfig;

public sealed class EntityConfigMainMenuBar
{
	public void Render(AppState appState, EntityConfigEditorState entityConfigEditorState)
	{
		if (ImGui.BeginMainMenuBar())
		{
			if (ImGui.BeginMenu("File"))
			{
				if (ImGui.MenuItem("New"))
				{
					entityConfigEditorState.ModelPaths.Clear();
					entityConfigEditorState.TexturePaths.Clear();
					entityConfigEditorState.DataTypes.Clear();
					entityConfigEditorState.EntityDescriptors.Clear();
				}

				if (ImGui.MenuItem("Open"))
				{
					DialogWrapper.FileOpen(
						s =>
						{
							if (s == null)
								return;

							string json = File.ReadAllText(s);
							EntityConfigModel config = EntityConfigSerializer.Deserialize(json);
							entityConfigEditorState.ModelPaths = config.ModelPaths.ToList();
							entityConfigEditorState.TexturePaths = config.TexturePaths.ToList();
							entityConfigEditorState.DataTypes = config.DataTypes.ToList();
							entityConfigEditorState.EntityDescriptors = config.EntityDescriptors.ToList();
						},
						"json");
				}

				if (ImGui.MenuItem("Save"))
				{
					EntityConfigBuilder builder = new();
					foreach (string modelPath in entityConfigEditorState.ModelPaths)
						builder = builder.WithModelPath(modelPath);
					foreach (string texturePath in entityConfigEditorState.TexturePaths)
						builder = builder.WithTexturePath(texturePath);
					foreach (DataType dataType in entityConfigEditorState.DataTypes)
						builder = builder.WithDataType(dataType);
					foreach (EntityDescriptor entityDescriptor in entityConfigEditorState.EntityDescriptors)
						builder = builder.WithEntityDescriptor(entityDescriptor);

					EntityConfigModel config = builder.Build();
					DialogWrapper.FileSave(
						s =>
						{
							if (s == null)
								return;

							string json = EntityConfigSerializer.Serialize(config);
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

			if (ImGui.BeginMenu("Help"))
			{
				if (ImGui.MenuItem("Shortcuts"))
				{
					entityConfigEditorState.ShowShortcutsWindow = true;
				}

				ImGui.EndMenu();
			}

			ImGui.EndMainMenuBar();
		}
	}
}
