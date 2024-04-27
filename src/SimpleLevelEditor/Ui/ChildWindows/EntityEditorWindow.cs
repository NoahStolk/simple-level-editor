using Detach;
using Detach.Numerics;
using ImGuiNET;
using OneOf;
using SimpleLevelEditor.Formats.EntityConfig.Model;
using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.Formats.Types.Level;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;
using System.Diagnostics;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class EntityEditorWindow
{
	private static readonly Entity _default = new()
	{
		Id = 0,
		Name = string.Empty,
		Properties = [],
		Position = default,
		Shape = ShapeDescriptor.Point,
	};
	public static Entity DefaultEntity { get; private set; } = _default.DeepCopy();

	public static void Render()
	{
		if (ImGui.BeginChild("Edit Entity", default, ImGuiChildFlags.Border))
		{
			ImGui.SeparatorText("Edit Entity");

			RenderEntityInputs(LevelEditorState.SelectedEntity ?? DefaultEntity);
		}

		ImGui.EndChild(); // End Object Editor
	}

	public static void Reset()
	{
		DefaultEntity = _default.DeepCopy();
	}

	private static void RenderEntityInputs(Entity entity)
	{
		RenderEntityType(entity);
		RenderEntityShape(entity);
		RenderEntityProperties(entity);
	}

	private static void RenderEntityType(Entity entity)
	{
		if (ImGui.BeginCombo(Inline.Span($"Entity Type##{entity.Id}"), entity.Name))
		{
			for (int i = 0; i < EntityConfigState.EntityConfig.Entities.Count; i++)
			{
				EntityDescriptor descriptor = EntityConfigState.EntityConfig.Entities[i];
				if (ImGui.Selectable(descriptor.Name))
				{
					entity.Name = descriptor.Name;
					entity.Shape = descriptor.Shape switch
					{
						EntityShape.Point => ShapeDescriptor.Point,
						EntityShape.Sphere => ShapeDescriptor.NewSphere(2),
						EntityShape.Aabb => ShapeDescriptor.NewAabb(-Vector3.One, Vector3.One),
						_ => throw new UnreachableException($"Invalid entity shape: {descriptor.Shape}"),
					};

					entity.Properties = descriptor.Properties.ConvertAll(p => new EntityProperty
					{
						Key = p.Name,
						Value = p.Type.Value switch
						{
							BoolPropertyType b => b.DefaultValue,
							IntPropertyType int32 => int32.DefaultValue,
							FloatPropertyType f => f.DefaultValue,
							Vector2PropertyType v2 => v2.DefaultValue,
							Vector3PropertyType v3 => v3.DefaultValue,
							Vector4PropertyType v4 => v4.DefaultValue,
							StringPropertyType str => str.DefaultValue,
							RgbPropertyType rgb => rgb.DefaultValue,
							RgbaPropertyType rgba => rgba.DefaultValue,
							_ => throw new UnreachableException($"Invalid entity property type: {p.Type}"),
						},
					});
					LevelState.Track("Changed entity type");
				}
			}

			ImGui.EndCombo();
		}
	}

	private static void RenderEntityShape(Entity entity)
	{
		ImGui.Text("Position");

		ImGui.DragFloat3("##position", ref entity.Position, 0.1f, float.MinValue, float.MaxValue, "%.1f");
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity position");

		if (RenderResetButton(Inline.Span($"Position_reset##{entity.Id}")))
		{
			entity.Position = Vector3.Zero;
			LevelState.Track("Changed entity position");
		}

		switch (entity.Shape)
		{
			case ShapeDescriptor.Sphere sphere: RenderSphereInputs(entity.Id, sphere); break;
			case ShapeDescriptor.Aabb aabb: RenderAabbInputs(entity.Id, aabb); break;
		}
	}

	private static void RenderEntityProperties(Entity entity)
	{
		ImGui.SeparatorText("Properties");

		EntityDescriptor? entityDescriptor = EntityConfigState.EntityConfig.Entities.Find(e => e.Name == entity.Name);
		if (entityDescriptor == null)
		{
			ImGui.TextColored(Color.Red, "Unknown entity type");
			return;
		}

		for (int i = 0; i < entityDescriptor.Properties.Count; i++)
		{
			EntityPropertyDescriptor propertyDescriptor = entityDescriptor.Properties[i];
			EntityProperty? property = entity.Properties.Find(p => p.Key == propertyDescriptor.Name);
			if (property == null)
			{
				property = new()
				{
					Key = propertyDescriptor.Name,
					Value = propertyDescriptor.Type.Value switch
					{
						BoolPropertyType b => b.DefaultValue,
						IntPropertyType int32 => int32.DefaultValue,
						FloatPropertyType f => f.DefaultValue,
						Vector2PropertyType v2 => v2.DefaultValue,
						Vector3PropertyType v3 => v3.DefaultValue,
						Vector4PropertyType v4 => v4.DefaultValue,
						StringPropertyType str => str.DefaultValue,
						RgbPropertyType rgb => rgb.DefaultValue,
						RgbaPropertyType rgba => rgba.DefaultValue,
						_ => throw new UnreachableException($"Invalid entity property type: {propertyDescriptor.Type}"),
					},
				};
				entity.Properties.Add(property);
			}

			OneOf<int, float> step = propertyDescriptor.Type.Value switch
			{
				IntPropertyType int32 => int32.Step ?? 0,
				FloatPropertyType f => f.Step ?? 0,
				Vector2PropertyType v2 => v2.Step ?? 0,
				Vector3PropertyType v3 => v3.Step ?? 0,
				Vector4PropertyType v4 => v4.Step ?? 0,
				_ => 0,
			};

			OneOf<int, float> minValue = propertyDescriptor.Type.Value switch
			{
				IntPropertyType int32 => int32.MinValue ?? 0,
				FloatPropertyType f => f.MinValue ?? 0,
				Vector2PropertyType v2 => v2.MinValue ?? 0,
				Vector3PropertyType v3 => v3.MinValue ?? 0,
				Vector4PropertyType v4 => v4.MinValue ?? 0,
				_ => 0,
			};

			OneOf<int, float> maxValue = propertyDescriptor.Type.Value switch
			{
				IntPropertyType int32 => int32.MaxValue ?? 0,
				FloatPropertyType f => f.MaxValue ?? 0,
				Vector2PropertyType v2 => v2.MaxValue ?? 0,
				Vector3PropertyType v3 => v3.MaxValue ?? 0,
				Vector4PropertyType v4 => v4.MaxValue ?? 0,
				_ => 0,
			};

			ImGui.Text(propertyDescriptor.Name);
			if (!string.IsNullOrWhiteSpace(propertyDescriptor.Description))
			{
				ImGui.SameLine();
				ImGui.TextColored(Color.Gray(0.5f), "(?)");
				if (ImGui.IsItemHovered())
					ImGui.SetTooltip(propertyDescriptor.Description);
			}

			property.Value = property.Value.Value switch
			{
				bool b when ImGui.Checkbox(Inline.Span($"##property_value{entity.Id}_{i}"), ref b) => b,
				int int32 when ImGui.DragInt(Inline.Span($"##property_value{entity.Id}_{i}"), ref int32, step.AsT0, minValue.AsT0, maxValue.AsT0) => int32,
				float f when ImGui.DragFloat(Inline.Span($"##property_value{entity.Id}_{i}"), ref f, step.AsT1, minValue.AsT1, maxValue.AsT1) => f,
				Vector2 v2 when ImGui.DragFloat2(Inline.Span($"##property_value{entity.Id}_{i}"), ref v2, step.AsT1, minValue.AsT1, maxValue.AsT1) => v2,
				Vector3 v3 when ImGui.DragFloat3(Inline.Span($"##property_value{entity.Id}_{i}"), ref v3, step.AsT1, minValue.AsT1, maxValue.AsT1) => v3,
				Vector4 v4 when ImGui.DragFloat4(Inline.Span($"##property_value{entity.Id}_{i}"), ref v4, step.AsT1, minValue.AsT1, maxValue.AsT1) => v4,
				string s when ImGui.InputText(Inline.Span($"##property_value{entity.Id}_{i}"), ref s, 32) => s,
				Rgb rgb when ImGuiUtils.ColorEdit3Rgb(Inline.Span($"##property_value{entity.Id}_{i}"), ref rgb) => rgb,
				Rgba rgba when ImGuiUtils.ColorEdit4Rgba(Inline.Span($"##property_value{entity.Id}_{i}"), ref rgba) => rgba,
				_ => property.Value,
			};

			if (ImGui.IsItemDeactivatedAfterEdit())
				LevelState.Track("Changed entity property value");

			ImGui.Separator();
		}
	}

	private static void RenderSphereInputs(int entityId, ShapeDescriptor.Sphere sphere)
	{
		ImGui.Text("Radius");

		ImGui.DragFloat(Inline.Span($"##radius{entityId}"), ref sphere.Radius, 0.1f, 0.1f, float.MaxValue, "%.1f");
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity radius");
	}

	private static void RenderAabbInputs(int entityId, ShapeDescriptor.Aabb aabb)
	{
		ImGui.Text("Box Min");

		ImGui.DragFloat3(Inline.Span($"##box_min{entityId}"), ref aabb.Min, 0.1f, float.MinValue, -0.1f, "%.1f");
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity box min");

		if (RenderResetButton(Inline.Span($"Box_min_reset{entityId}")))
		{
			aabb.Min = -Vector3.One;
			LevelState.Track("Changed entity box min");
		}

		ImGui.Text("Box Max");

		ImGui.DragFloat3(Inline.Span($"##box_max{entityId}"), ref aabb.Max, 0.1f, 0.1f, float.MaxValue, "%.1f");
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity box max");

		if (RenderResetButton(Inline.Span($"Box_max_reset{entityId}")))
		{
			aabb.Max = Vector3.One;
			LevelState.Track("Changed entity box max");
		}
	}

	private static bool RenderResetButton(ReadOnlySpan<char> label)
	{
		ImGui.SameLine();
		ImGui.PushID(label);
		if (ImGui.Button("Reset"))
			return true;

		ImGui.PopID();
		return false;
	}
}
