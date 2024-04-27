using Detach;
using ImGuiNET;
using SimpleLevelEditor.Formats.EntityConfig.Model;
using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.Formats.Types;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;
using System.Diagnostics;
using Color = Detach.Numerics.Color;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class EntityEditorWindow
{
	private static readonly Entity _default = new()
	{
		Id = 0,
		Name = string.Empty,
		Properties = [],
		Position = default,
		Shape = Level.ShapeDescriptor.Point,
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
						EntityShape.Point => Level.ShapeDescriptor.Point,
						EntityShape.Sphere => Level.ShapeDescriptor.NewSphere(2),
						EntityShape.Aabb => Level.ShapeDescriptor.NewAabb(-Vector3.One, Vector3.One),
						_ => throw new UnreachableException($"Invalid entity shape: {descriptor.Shape}"),
					};

					entity.Properties = descriptor.Properties.ConvertAll(p => new EntityProperty
					{
						Key = p.Name,
						Value = p.Type.DefaultValue,
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
			case Level.ShapeDescriptor.Sphere sphere:
				RenderSphereInputs(entity.Id, ref sphere);
				entity.Shape = sphere;
				break;
			case Level.ShapeDescriptor.Aabb aabb:
				RenderAabbInputs(entity.Id, ref aabb);
				entity.Shape = aabb;
				break;
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
				property = new EntityProperty
				{
					Key = propertyDescriptor.Name,
					Value = propertyDescriptor.Type.DefaultValue,
				};
				entity.Properties.Add(property);
			}

			float step = propertyDescriptor.Type.Step;
			float minValue = propertyDescriptor.Type.MinValue;
			float maxValue = propertyDescriptor.Type.MaxValue;

			ImGui.Text(propertyDescriptor.Name);
			if (!string.IsNullOrWhiteSpace(propertyDescriptor.Description))
			{
				ImGui.SameLine();
				ImGui.TextColored(Color.Gray(0.5f), "(?)");
				if (ImGui.IsItemHovered())
					ImGui.SetTooltip(propertyDescriptor.Description);
			}

			switch (property.Value)
			{
				case Level.EntityPropertyValue.Bool b:
					bool boolValue = b.Value;
					if (ImGui.Checkbox(Inline.Span($"##property_value{entity.Id}_{i}"), ref boolValue))
						property.Value = Level.EntityPropertyValue.NewBool(boolValue);
					break;
				case Level.EntityPropertyValue.Int int32:
					int intValue = int32.Value;
					if (ImGui.DragInt(Inline.Span($"##property_value{entity.Id}_{i}"), ref intValue, (int)step, (int)minValue, (int)maxValue))
						property.Value = Level.EntityPropertyValue.NewInt(intValue);
					break;
				case Level.EntityPropertyValue.Float f:
					float floatValue = f.Value;
					if (ImGui.DragFloat(Inline.Span($"##property_value{entity.Id}_{i}"), ref floatValue, step, minValue, maxValue))
						property.Value = Level.EntityPropertyValue.NewFloat(floatValue);
					break;
				case Level.EntityPropertyValue.Vector2 v2:
					Vector2 vector2Value = v2.Value;
					if (ImGui.DragFloat2(Inline.Span($"##property_value{entity.Id}_{i}"), ref vector2Value, step, minValue, maxValue))
						property.Value = Level.EntityPropertyValue.NewVector2(vector2Value);
					break;
				case Level.EntityPropertyValue.Vector3 v3:
					Vector3 vector3Value = v3.Value;
					if (ImGui.DragFloat3(Inline.Span($"##property_value{entity.Id}_{i}"), ref vector3Value, step, minValue, maxValue))
						property.Value = Level.EntityPropertyValue.NewVector3(vector3Value);
					break;
				case Level.EntityPropertyValue.Vector4 v4:
					Vector4 vector4Value = v4.Value;
					if (ImGui.DragFloat4(Inline.Span($"##property_value{entity.Id}_{i}"), ref vector4Value, step, minValue, maxValue))
						property.Value = Level.EntityPropertyValue.NewVector4(vector4Value);
					break;
				case Level.EntityPropertyValue.String s:
					string stringValue = s.Value;
					if (ImGui.InputText(Inline.Span($"##property_value{entity.Id}_{i}"), ref stringValue, 32))
						property.Value = Level.EntityPropertyValue.NewString(stringValue);
					break;
				case Level.EntityPropertyValue.Rgb rgb:
					Formats.Types.Color.Rgb rgbValue = rgb.Value;
					if (ImGuiUtils.ColorEdit3Rgb(Inline.Span($"##property_value{entity.Id}_{i}"), ref rgbValue))
						property.Value = Level.EntityPropertyValue.NewRgb(rgbValue);
					break;
				case Level.EntityPropertyValue.Rgba rgba:
					Formats.Types.Color.Rgba rgbaValue = rgba.Value;
					if (ImGuiUtils.ColorEdit4Rgba(Inline.Span($"##property_value{entity.Id}_{i}"), ref rgbaValue))
						property.Value = Level.EntityPropertyValue.NewRgba(rgbaValue);
					break;
				default: throw new UnreachableException();
			}

			if (ImGui.IsItemDeactivatedAfterEdit())
				LevelState.Track("Changed entity property value");

			ImGui.Separator();
		}
	}

	private static void RenderSphereInputs(int entityId, ref Level.ShapeDescriptor.Sphere sphere)
	{
		ImGui.Text("Radius");

		float radius = sphere.Radius;
		if (ImGui.DragFloat(Inline.Span($"##radius{entityId}"), ref radius, 0.1f, 0.1f, float.MaxValue, "%.1f"))
			sphere = (Level.ShapeDescriptor.Sphere)Level.ShapeDescriptor.NewSphere(radius);
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity radius");
	}

	private static void RenderAabbInputs(int entityId, ref Level.ShapeDescriptor.Aabb aabb)
	{
		ImGui.Text("Box Min");

		Vector3 min = aabb.Min;
		if (ImGui.DragFloat3(Inline.Span($"##box_min{entityId}"), ref min, 0.1f, float.MinValue, -0.1f, "%.1f"))
			aabb = (Level.ShapeDescriptor.Aabb)Level.ShapeDescriptor.NewAabb(min, aabb.Max);
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity box min");

		if (RenderResetButton(Inline.Span($"Box_min_reset{entityId}")))
		{
			aabb = (Level.ShapeDescriptor.Aabb)Level.ShapeDescriptor.NewAabb(-Vector3.One, aabb.Max);
			LevelState.Track("Changed entity box min");
		}

		ImGui.Text("Box Max");

		Vector3 max = aabb.Max;
		if (ImGui.DragFloat3(Inline.Span($"##box_max{entityId}"), ref max, 0.1f, 0.1f, float.MaxValue, "%.1f"))
			aabb = (Level.ShapeDescriptor.Aabb)Level.ShapeDescriptor.NewAabb(aabb.Min, max);
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity box max");

		if (RenderResetButton(Inline.Span($"Box_max_reset{entityId}")))
		{
			aabb = (Level.ShapeDescriptor.Aabb)Level.ShapeDescriptor.NewAabb(aabb.Min, Vector3.One);
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
