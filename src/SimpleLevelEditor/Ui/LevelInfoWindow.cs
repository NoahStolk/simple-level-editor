using Detach;
using ImGuiNET;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Formats.EntityConfig.Model;
using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;
using System.Globalization;

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
					ImGui.Text(Inline.Span($"Shape: {entity.Shape.ToString()}"));
					if (entity.Properties.Count > 0)
					{
						RenderEntityProperties(i, entity);
					}

					ImGui.TreePop();
				}
			}

			ImGui.TreePop();
		}
	}

	private static void RenderEntityProperties(int i, EntityDescriptor entity)
	{
		if (ImGui.TreeNode(Inline.Span($"Properties##{i}")))
		{
			if (ImGui.BeginTable(Inline.Span($"PropertiesTable{i}"), 6))
			{
				ImGui.TableSetupColumn("Type");
				ImGui.TableSetupColumn("Name");
				ImGui.TableSetupColumn("Default");
				ImGui.TableSetupColumn("Step");
				ImGui.TableSetupColumn("Min");
				ImGui.TableSetupColumn("Max");
				ImGui.TableHeadersRow();

				for (int j = 0; j < entity.Properties.Count; j++)
				{
					EntityPropertyDescriptor property = entity.Properties[j];
					Vector4 color = property.Type.GetDisplayColor();
					string typeId = property.Type.GetTypeId();
					string defaultValue = property.Type.DefaultValue.WriteValue();
					string step = property.Type.Step.ToString(CultureInfo.InvariantCulture);
					string minValue = property.Type.MinValue.ToString(CultureInfo.InvariantCulture);
					string maxValue = property.Type.MaxValue.ToString(CultureInfo.InvariantCulture);

					ImGui.TableNextRow();

					ImGui.TableNextColumn();
					ImGui.TextColored(color, typeId);

					ImGui.TableNextColumn();
					ImGui.Text(property.Name);

					ImGui.TableNextColumn();
					ImGuiUtils.TextOptional(defaultValue);

					ImGui.TableNextColumn();
					ImGuiUtils.TextOptional(step, property.Type.Step.IsZero());

					ImGui.TableNextColumn();
					ImGuiUtils.TextOptional(minValue, property.Type.MinValue.IsZero());

					ImGui.TableNextColumn();
					ImGuiUtils.TextOptional(maxValue, property.Type.MaxValue.IsZero());
				}

				ImGui.EndTable();
			}

			ImGui.TreePop();
		}
	}
}
