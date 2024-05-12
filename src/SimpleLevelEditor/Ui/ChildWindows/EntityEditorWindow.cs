using Detach;
using ImGuiNET;
using Microsoft.FSharp.Collections;
using SimpleLevelEditor.Formats.EntityConfig.Model;
using SimpleLevelEditor.Formats.Types;
using SimpleLevelEditor.Formats.Types.Level;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;
using System.Diagnostics;
using Color = Detach.Numerics.Color;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class EntityEditorWindow
{
	public static Entity DefaultEntity { get; private set; } = Entity.CreateDefault();

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
		DefaultEntity = Entity.CreateDefault();
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
					entity.Shape = descriptor.Shape.GetDefaultDescriptor();
					entity.Properties = ListModule.OfSeq(descriptor.Properties.ConvertAll(p => new EntityProperty(p.Name, p.Type.DefaultValue)));
					LevelState.Track("Changed entity type");
				}
			}

			ImGui.EndCombo();
		}
	}

	private static void RenderEntityShape(Entity entity)
	{
		ImGui.Text("Position");

		Vector3 position = entity.Position;
		if (ImGui.DragFloat3("##position", ref position, 0.1f, float.MinValue, float.MaxValue, "%.1f"))
			entity.Position = position;

		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity position");

		if (RenderResetButton(Inline.Span($"Position_reset##{entity.Id}")))
		{
			entity.Position = Vector3.Zero;
			LevelState.Track("Changed entity position");
		}

		switch (entity.Shape)
		{
			case ShapeDescriptor.Sphere sphere:
				RenderSphereInputs(entity.Id, ref sphere);
				entity.Shape = sphere;
				break;
			case ShapeDescriptor.Aabb aabb:
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
			EntityProperty? property = entity.Properties.FirstOrDefault(p => p.Key == propertyDescriptor.Name);
			if (property == null)
			{
				property = new EntityProperty(propertyDescriptor.Name, propertyDescriptor.Type.DefaultValue);
				entity.Properties.Append(property);
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
				case EntityPropertyValue.Bool b:
					bool boolValue = b.Value;
					if (ImGui.Checkbox(Inline.Span($"##property_value{entity.Id}_{i}"), ref boolValue))
						property.Value = EntityPropertyValue.NewBool(boolValue);
					break;
				case EntityPropertyValue.Int int32:
					int intValue = int32.Value;
					if (ImGui.DragInt(Inline.Span($"##property_value{entity.Id}_{i}"), ref intValue, (int)step, (int)minValue, (int)maxValue))
						property.Value = EntityPropertyValue.NewInt(intValue);
					break;
				case EntityPropertyValue.Float f:
					float floatValue = f.Value;
					if (ImGui.DragFloat(Inline.Span($"##property_value{entity.Id}_{i}"), ref floatValue, step, minValue, maxValue))
						property.Value = EntityPropertyValue.NewFloat(floatValue);
					break;
				case EntityPropertyValue.Vector2 v2:
					Vector2 vector2Value = v2.Value;
					if (ImGui.DragFloat2(Inline.Span($"##property_value{entity.Id}_{i}"), ref vector2Value, step, minValue, maxValue))
						property.Value = EntityPropertyValue.NewVector2(vector2Value);
					break;
				case EntityPropertyValue.Vector3 v3:
					Vector3 vector3Value = v3.Value;
					if (ImGui.DragFloat3(Inline.Span($"##property_value{entity.Id}_{i}"), ref vector3Value, step, minValue, maxValue))
						property.Value = EntityPropertyValue.NewVector3(vector3Value);
					break;
				case EntityPropertyValue.Vector4 v4:
					Vector4 vector4Value = v4.Value;
					if (ImGui.DragFloat4(Inline.Span($"##property_value{entity.Id}_{i}"), ref vector4Value, step, minValue, maxValue))
						property.Value = EntityPropertyValue.NewVector4(vector4Value);
					break;
				case EntityPropertyValue.String s:
					string stringValue = s.Value;
					if (ImGui.InputText(Inline.Span($"##property_value{entity.Id}_{i}"), ref stringValue, 32))
						property.Value = EntityPropertyValue.NewString(stringValue);
					break;
				case EntityPropertyValue.Rgb rgb:
					Rgb rgbValue = rgb.Value;
					if (ImGuiUtils.ColorEdit3Rgb(Inline.Span($"##property_value{entity.Id}_{i}"), ref rgbValue))
						property.Value = EntityPropertyValue.NewRgb(rgbValue);
					break;
				case EntityPropertyValue.Rgba rgba:
					Rgba rgbaValue = rgba.Value;
					if (ImGuiUtils.ColorEdit4Rgba(Inline.Span($"##property_value{entity.Id}_{i}"), ref rgbaValue))
						property.Value = EntityPropertyValue.NewRgba(rgbaValue);
					break;
				default: throw new UnreachableException();
			}

			if (ImGui.IsItemDeactivatedAfterEdit())
				LevelState.Track("Changed entity property value");

			ImGui.Separator();
		}
	}

	private static void RenderSphereInputs(int entityId, ref ShapeDescriptor.Sphere sphere)
	{
		ImGui.Text("Radius");

		float radius = sphere.Radius;
		if (ImGui.DragFloat(Inline.Span($"##radius{entityId}"), ref radius, 0.1f, 0.1f, float.MaxValue, "%.1f"))
			sphere = (ShapeDescriptor.Sphere)ShapeDescriptor.NewSphere(radius);
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity radius");
	}

	private static void RenderAabbInputs(int entityId, ref ShapeDescriptor.Aabb aabb)
	{
		ImGui.Text("Box Size");

		Vector3 size = aabb.Size;
		if (ImGui.DragFloat3(Inline.Span($"##box_size{entityId}"), ref size, 0.1f, float.MinValue, -0.1f, "%.1f"))
			aabb = (ShapeDescriptor.Aabb)ShapeDescriptor.NewAabb(size);
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity box size");

		if (RenderResetButton(Inline.Span($"Box_min_reset{entityId}")))
		{
			aabb = (ShapeDescriptor.Aabb)ShapeDescriptor.NewAabb(Vector3.One);
			LevelState.Track("Changed entity box size");
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
