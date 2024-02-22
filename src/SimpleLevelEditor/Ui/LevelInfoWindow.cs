using Detach;
using ImGuiNET;
using SimpleLevelEditor.Formats.EntityConfig.Model;
using SimpleLevelEditor.Formats.EntityConfig.Model.PropertyTypes;
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
					Vector4 color = property.Type.Value switch
					{
						BoolPropertyType => new(0, 0.25f, 1, 1),
						IntPropertyType => new(0, 0.5f, 1, 1),
						FloatPropertyType => new(0, 0.7f, 0, 1),
						Vector2PropertyType => new(0, 0.8f, 0, 1),
						Vector3PropertyType => new(0, 0.9f, 0, 1),
						Vector4PropertyType => new(0, 1, 0, 1),
						StringPropertyType => new(1, 0.5f, 0, 1),
						RgbPropertyType => new(1, 0.75f, 0, 1),
						RgbaPropertyType => new(1, 1, 0, 1),
						_ => new(1, 0, 0, 1),
					};
					string type = property.Type.Value switch
					{
						BoolPropertyType => "bool",
						IntPropertyType => "int",
						FloatPropertyType => "float",
						Vector2PropertyType => "Vector2",
						Vector3PropertyType => "Vector3",
						Vector4PropertyType => "Vector4",
						StringPropertyType => "string",
						RgbPropertyType => "Rgb",
						RgbaPropertyType => "Rgba",
						_ => "unknown",
					};
					string defaultValue = property.Type.Value switch
					{
						BoolPropertyType boolProperty => boolProperty.DefaultValue.ToString(),
						IntPropertyType intProperty => intProperty.DefaultValue.ToString(CultureInfo.InvariantCulture),
						FloatPropertyType floatProperty => floatProperty.DefaultValue.ToString(CultureInfo.InvariantCulture),
						Vector2PropertyType vector2Property => vector2Property.DefaultValue.ToString(),
						Vector3PropertyType vector3Property => vector3Property.DefaultValue.ToString(),
						Vector4PropertyType vector4Property => vector4Property.DefaultValue.ToString(),
						StringPropertyType stringProperty => stringProperty.DefaultValue,
						RgbPropertyType rgbProperty => rgbProperty.DefaultValue.ToString(),
						RgbaPropertyType rgbaProperty => rgbaProperty.DefaultValue.ToString(),
						_ => "unknown",
					};
					string? step = property.Type.Value switch
					{
						IntPropertyType intProperty => intProperty.Step?.ToString(CultureInfo.InvariantCulture),
						FloatPropertyType floatProperty => floatProperty.Step?.ToString(CultureInfo.InvariantCulture),
						Vector2PropertyType vector2Property => vector2Property.Step?.ToString(CultureInfo.InvariantCulture),
						Vector3PropertyType vector3Property => vector3Property.Step?.ToString(CultureInfo.InvariantCulture),
						Vector4PropertyType vector4Property => vector4Property.Step?.ToString(CultureInfo.InvariantCulture),
						_ => null,
					};
					string? minValue = property.Type.Value switch
					{
						IntPropertyType intProperty => intProperty.MinValue?.ToString(CultureInfo.InvariantCulture),
						FloatPropertyType floatProperty => floatProperty.MinValue?.ToString(CultureInfo.InvariantCulture),
						Vector2PropertyType vector2Property => vector2Property.MinValue?.ToString(CultureInfo.InvariantCulture),
						Vector3PropertyType vector3Property => vector3Property.MinValue?.ToString(CultureInfo.InvariantCulture),
						Vector4PropertyType vector4Property => vector4Property.MinValue?.ToString(CultureInfo.InvariantCulture),
						_ => null,
					};
					string? maxValue = property.Type.Value switch
					{
						IntPropertyType intProperty => intProperty.MaxValue?.ToString(CultureInfo.InvariantCulture),
						FloatPropertyType floatProperty => floatProperty.MaxValue?.ToString(CultureInfo.InvariantCulture),
						Vector2PropertyType vector2Property => vector2Property.MaxValue?.ToString(CultureInfo.InvariantCulture),
						Vector3PropertyType vector3Property => vector3Property.MaxValue?.ToString(CultureInfo.InvariantCulture),
						Vector4PropertyType vector4Property => vector4Property.MaxValue?.ToString(CultureInfo.InvariantCulture),
						_ => null,
					};

					ImGui.TableNextRow();

					ImGui.TableNextColumn();
					ImGui.TextColored(color, type);

					ImGui.TableNextColumn();
					ImGui.Text(property.Name);

					ImGui.TableNextColumn();
					ImGuiUtils.TextOptional(defaultValue);

					ImGui.TableNextColumn();
					ImGuiUtils.TextOptional(step);

					ImGui.TableNextColumn();
					ImGuiUtils.TextOptional(minValue);

					ImGui.TableNextColumn();
					ImGuiUtils.TextOptional(maxValue);
				}

				ImGui.EndTable();
			}

			ImGui.TreePop();
		}
	}
}
