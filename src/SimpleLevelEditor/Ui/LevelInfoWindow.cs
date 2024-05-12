using Detach;
using Detach.Numerics;
using ImGuiNET;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Formats.EntityConfig.Model;
using SimpleLevelEditor.Formats.Types;
using SimpleLevelEditor.Formats.Types.EntityConfig;
using SimpleLevelEditor.Formats.Types.Level;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;
using System.Diagnostics;
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
		ImGui.Text(Inline.Span($"Meshes: {level.Meshes.Length}"));
		ImGui.Text(Inline.Span($"Textures: {level.Textures.Length}"));
		ImGui.Text(Inline.Span($"WorldObjects: {level.WorldObjects.Length}"));
		ImGui.Text(Inline.Span($"Entities: {level.Entities.Length}"));
		ImGui.SeparatorText("Entity config");
		ImGui.TextWrapped(level.EntityConfigPath == null ? "<No entity config loaded>" : level.EntityConfigPath.Value);
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
				ImGui.SetNextItemOpen(true, ImGuiCond.Appearing);
				if (ImGui.TreeNode(entity.Name))
				{
					RenderEntity(i, entity);
					ImGui.TreePop();
				}
			}

			ImGui.TreePop();
		}
	}

	private static void RenderEntity(int i, EntityDescriptor entity)
	{
		ImGui.Text(Inline.Span($"Shape: {entity.Shape.GetTypeId()}"));

		ImGui.SetNextItemOpen(true, ImGuiCond.Appearing);
		if (ImGui.TreeNode(Inline.Span($"Editor visualization##{entity.Name}")))
		{
			if (ImGui.BeginTable(Inline.Span($"EditorVisualizationTable{i}"), 2))
			{
				ImGui.TableSetupColumn("Property", ImGuiTableColumnFlags.WidthFixed, 96);
				ImGui.TableSetupColumn("Value");
				ImGui.TableHeadersRow();

				switch (entity.Shape)
				{
					case EntityShape.Point point:
						NextColumnTextColored(Color.Green, "Type");
						NextColumnText(point.Visualization.GetTypeId());

						switch (point.Visualization)
						{
							case PointEntityVisualization.SimpleSphere simpleSphere:
								NextColumnTextColored(Color.Yellow, "Color");
								NextColumnText(ToColorString(simpleSphere.Color));
								NextColumnTextColored(Color.Orange, "Radius");
								NextColumnText(simpleSphere.Radius.ToString(CultureInfo.InvariantCulture));
								break;
							case PointEntityVisualization.BillboardSprite billboardSprite:
								NextColumnTextColored(Color.Purple, "Texture");
								NextColumnText(billboardSprite.TextureName);
								NextColumnTextColored(Color.Aqua, "Size");
								NextColumnText(billboardSprite.Size.ToString(CultureInfo.InvariantCulture));
								break;
							case PointEntityVisualization.Mesh mesh:
								NextColumnTextColored(Color.Red, "Mesh");
								NextColumnText(mesh.MeshName);
								NextColumnTextColored(Color.Purple, "Texture");
								NextColumnText(mesh.TextureName);
								NextColumnTextColored(Color.Aqua, "Size");
								NextColumnText(mesh.Size.ToString(CultureInfo.InvariantCulture));
								break;
							default:
								throw new UnreachableException($"Unknown point visualization type: {point.Visualization.GetTypeId()}");
						}

						break;
					case EntityShape.Sphere sphere:
						NextColumnTextColored(Color.Yellow, "Color");
						NextColumnText(ToColorString(sphere.Color));
						break;
					case EntityShape.Aabb aabb:
						NextColumnTextColored(Color.Yellow, "Color");
						NextColumnText(ToColorString(aabb.Color));
						break;
					default:
						throw new UnreachableException($"Unknown entity shape type: {entity.Shape.GetTypeId()}");
				}

				ImGui.EndTable();
			}

			ImGui.TreePop();
		}

		if (entity.Properties.Count > 0)
		{
			RenderEntityProperties(i, entity);
		}
	}

	private static string ToColorString(Rgb rgb)
	{
		// TODO: Use F# ToDataString instead.
		return $"{rgb.R} {rgb.G} {rgb.B}";
	}

	private static void RenderEntityProperties(int i, EntityDescriptor entity)
	{
		ImGui.SetNextItemOpen(true, ImGuiCond.Appearing);
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

					NextColumnTextColored(color, typeId);
					NextColumnText(property.Name);
					NextColumnTextOptional(defaultValue);
					NextColumnTextOptional(step, property.Type.Step.IsZero());
					NextColumnTextOptional(minValue, property.Type.MinValue.IsZero());
					NextColumnTextOptional(maxValue, property.Type.MaxValue.IsZero());
				}

				ImGui.EndTable();
			}

			ImGui.TreePop();
		}
	}

	private static void NextColumnText(ReadOnlySpan<char> text)
	{
		ImGui.TableNextColumn();
		ImGui.Text(text);
	}

	private static void NextColumnTextColored(Vector4 color, ReadOnlySpan<char> text)
	{
		ImGui.TableNextColumn();
		ImGui.TextColored(color, text);
	}

	private static void NextColumnTextOptional(string? text)
	{
		ImGui.TableNextColumn();
		ImGuiUtils.TextOptional(text);
	}

	private static void NextColumnTextOptional(ReadOnlySpan<char> text, bool condition)
	{
		ImGui.TableNextColumn();
		ImGuiUtils.TextOptional(text, condition);
	}
}
