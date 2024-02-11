using Detach;
using ImGuiNET;
using SimpleLevelEditor.Model.EntityConfig;
using SimpleLevelEditor.Model.Level;
using SimpleLevelEditor.State;

namespace SimpleLevelEditor.Ui;

public static class LevelInfoWindow
{
	public static void Render()
	{
		if (ImGui.Begin("Level Info"))
		{
			ImGui.TextWrapped(LevelState.LevelFilePath ?? "<No level loaded>");
			if (LevelState.LevelFilePath != null)
			{
				ImGui.Text(LevelState.IsModified ? "(unsaved changes)" : "(saved)");
				ImGui.SeparatorText("Level");
				RenderLevel(LevelState.Level);
			}
		}

		ImGui.End();
	}

	private static void RenderLevel(Level3dData level)
	{
		ImGui.Text(Inline.Span($"Version: {level.Version}"));
		ImGui.Text(Inline.Span($"Meshes: {level.Meshes.Count}"));
		ImGui.Text(Inline.Span($"Textures: {level.Textures.Count}"));
		ImGui.Text(Inline.Span($"WorldObjects: {level.WorldObjects.Count}"));
		ImGui.Text(Inline.Span($"Entities: {level.Entities.Count}"));
		ImGui.SeparatorText("Entity config");
		ImGui.TextWrapped(level.EntityConfigPath ?? "<No entity config loaded>");
		if (level.EntityConfigPath != null)
		{
			RenderEntityConfig(EntityConfigState.EntityConfig);
		}
	}

	private static void RenderEntityConfig(EntityConfigData entityConfig)
	{
		ImGui.Text(Inline.Span($"Version: {entityConfig.Version}"));
		if (ImGui.TreeNode("Entities"))
		{
			for (int i = 0; i < entityConfig.Entities.Count; i++)
			{
				EntityDescriptor entity = entityConfig.Entities[i];
				if (ImGui.TreeNode(entity.Name))
				{
					ImGui.Text(Inline.Span($"Name: {entity.Name}"));
					ImGui.Text(Inline.Span($"Shape: {entity.Shape}"));
					if (ImGui.TreeNode(Inline.Span($"Properties##{i}")))
					{
						for (int j = 0; j < entity.Properties.Count; j++)
						{
							EntityPropertyDescriptor property = entity.Properties[j];
							ImGui.TextColored(
							property.Type switch
							{
								EntityPropertyType.Bool => new(0, 0.25f, 1, 1),
								EntityPropertyType.Int => new(0, 0.5f, 1, 1),
								EntityPropertyType.Float => new(0, 0.7f, 0, 1),
								EntityPropertyType.Vector2 => new(0, 0.8f, 0, 1),
								EntityPropertyType.Vector3 => new(0, 0.9f, 0, 1),
								EntityPropertyType.Vector4 => new(0, 1, 0, 1),
								EntityPropertyType.String => new(1, 0.5f, 0, 1),
								EntityPropertyType.Rgb => new(1, 0.75f, 0, 1),
								EntityPropertyType.Rgba => new(1, 1, 0, 1),
								_ => new(1, 0, 0, 1),
							},
							Inline.Span(property.Type));
							ImGui.SameLine();
							ImGui.Text(property.Name);
						}

						ImGui.TreePop();
					}

					ImGui.TreePop();
				}
			}

			ImGui.TreePop();
		}
	}
}
