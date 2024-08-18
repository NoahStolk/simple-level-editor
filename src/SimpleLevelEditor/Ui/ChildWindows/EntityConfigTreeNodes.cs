using Detach;
using ImGuiNET;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Formats;
using SimpleLevelEditor.Formats.EntityConfig;
using SimpleLevelEditor.Utils;
using System.Diagnostics;
using System.Globalization;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class EntityConfigTreeNodes
{
	public static void Render(EntityConfigData entityConfig)
	{
		if (ImGui.TreeNode("Models"))
		{
			for (int i = 0; i < entityConfig.ModelPaths.Count; i++)
				ImGui.Text(entityConfig.ModelPaths[i]);

			ImGui.TreePop();
		}

		if (ImGui.TreeNode("Textures"))
		{
			for (int i = 0; i < entityConfig.TexturePaths.Count; i++)
				ImGui.Text(entityConfig.TexturePaths[i]);

			ImGui.TreePop();
		}

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
					case EntityShapeDescriptor.Point point:
						NextColumnTextColored(Detach.Numerics.Rgba.Green, "Type");
						NextColumnText(point.Visualization.GetTypeId());

						switch (point.Visualization)
						{
							case PointEntityVisualization.SimpleSphere simpleSphere:
								NextColumnTextColored(Detach.Numerics.Rgba.Yellow, "Color");
								NextColumnText(simpleSphere.Color.ToDisplayString());
								NextColumnTextColored(Detach.Numerics.Rgba.Orange, "Radius");
								NextColumnText(simpleSphere.Radius.ToString(CultureInfo.InvariantCulture));
								break;
							case PointEntityVisualization.BillboardSprite billboardSprite:
								NextColumnTextColored(Detach.Numerics.Rgba.Purple, "Texture path");
								NextColumnText(billboardSprite.TexturePath);
								NextColumnTextColored(Detach.Numerics.Rgba.Aqua, "Size");
								NextColumnText(billboardSprite.Size.ToString(CultureInfo.InvariantCulture));
								break;
							case PointEntityVisualization.Model model:
								NextColumnTextColored(Detach.Numerics.Rgba.Red, "Model path");
								NextColumnText(model.ModelPath);
								NextColumnTextColored(Detach.Numerics.Rgba.Aqua, "Size");
								NextColumnText(model.Size.ToString(CultureInfo.InvariantCulture));
								break;
							default:
								throw new UnreachableException($"Unknown point visualization type: {point.Visualization.GetTypeId()}");
						}

						break;
					case EntityShapeDescriptor.Sphere sphere:
						NextColumnTextColored(Detach.Numerics.Rgba.Yellow, "Color");
						NextColumnText(sphere.Color.ToDisplayString());
						break;
					case EntityShapeDescriptor.Aabb aabb:
						NextColumnTextColored(Detach.Numerics.Rgba.Yellow, "Color");
						NextColumnText(aabb.Color.ToDisplayString());
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
					string defaultValue = property.Type.GetDefaultValue().ToDisplayString();
					ImGui.TableNextRow();

					NextColumnTextColored(color, typeId);
					NextColumnText(property.Name);
					NextColumnTextOptional(defaultValue);
					NextColumnTextOptional(Inline.Span(property.Type.GetStep()), property.Type.GetStep().IsZero());
					NextColumnTextOptional(Inline.Span(property.Type.GetMin()), property.Type.GetMin().IsZero());
					NextColumnTextOptional(Inline.Span(property.Type.GetMax()), property.Type.GetMax().IsZero());
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
