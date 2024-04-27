using Detach;
using ImGuiNET;
using SimpleLevelEditor.Formats.EntityConfig.Model;
using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.Formats.Types;
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
					ImGui.Text(Inline.Span($"Shape: {entity.Shape}"));
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
					Vector4 color = property.Type switch
					{
						EntityConfig.EntityPropertyTypeDescriptor.BoolProperty => new Vector4(0, 0.25f, 1, 1),
						EntityConfig.EntityPropertyTypeDescriptor.IntProperty => new Vector4(0, 0.5f, 1, 1),
						EntityConfig.EntityPropertyTypeDescriptor.FloatProperty => new Vector4(0, 0.7f, 0, 1),
						EntityConfig.EntityPropertyTypeDescriptor.Vector2Property => new Vector4(0, 0.8f, 0, 1),
						EntityConfig.EntityPropertyTypeDescriptor.Vector3Property => new Vector4(0, 0.9f, 0, 1),
						EntityConfig.EntityPropertyTypeDescriptor.Vector4Property => new Vector4(0, 1, 0, 1),
						EntityConfig.EntityPropertyTypeDescriptor.StringProperty => new Vector4(1, 0.5f, 0, 1),
						EntityConfig.EntityPropertyTypeDescriptor.RgbProperty => new Vector4(1, 0.75f, 0, 1),
						EntityConfig.EntityPropertyTypeDescriptor.RgbaProperty => new Vector4(1, 1, 0, 1),
						_ => new Vector4(1, 0, 0, 1),
					};
					string typeId = property.Type.GetTypeId();
					string defaultValue = property.Type.DefaultValue.ToString(); // TODO: Test this.
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
					ImGuiUtils.TextOptional(step, property.Type.Step == 0); // TODO: Test this.

					ImGui.TableNextColumn();
					ImGuiUtils.TextOptional(minValue, property.Type.MinValue == 0);

					ImGui.TableNextColumn();
					ImGuiUtils.TextOptional(maxValue, property.Type.MaxValue == 0);
				}

				ImGui.EndTable();
			}

			ImGui.TreePop();
		}
	}
}
