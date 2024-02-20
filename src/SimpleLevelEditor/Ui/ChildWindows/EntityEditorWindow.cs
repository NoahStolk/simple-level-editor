using Detach;
using Detach.Numerics;
using ImGuiNET;
using OneOf;
using SimpleLevelEditor.Formats;
using SimpleLevelEditor.Formats.Model;
using SimpleLevelEditor.Formats.Model.EntityConfig;
using SimpleLevelEditor.Formats.Model.Level;
using SimpleLevelEditor.Formats.Model.Level.EntityShapes;
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
		Shape = new Point(),
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
						EntityShape.Point => new Point(),
						EntityShape.Sphere => new Sphere(2),
						EntityShape.Aabb => new Aabb(-Vector3.One, Vector3.One),
						_ => throw new UnreachableException($"Invalid entity shape: {descriptor.Shape}"),
					};

					// TODO: Report warning when TryRead calls fail.
					entity.Properties = descriptor.Properties.ConvertAll(p => new EntityProperty
					{
						Key = p.Name,
						Value = p.Type switch
						{
							EntityPropertyType.Bool => DataFormatter.TryReadBool(p.DefaultValue, out bool b) && b,
							EntityPropertyType.Int => DataFormatter.TryReadInt(p.DefaultValue, out int int32) ? int32 : 0,
							EntityPropertyType.Float => DataFormatter.TryReadFloat(p.DefaultValue, out float f) ? f : 0f,
							EntityPropertyType.Vector2 => DataFormatter.TryReadVector2(p.DefaultValue, out Vector2 v2) ? v2 : Vector2.Zero,
							EntityPropertyType.Vector3 => DataFormatter.TryReadVector3(p.DefaultValue, out Vector3 v3) ? v3 : Vector3.Zero,
							EntityPropertyType.Vector4 => DataFormatter.TryReadVector4(p.DefaultValue, out Vector4 v4) ? v4 : Vector4.Zero,
							EntityPropertyType.String => p.DefaultValue ?? string.Empty,
							EntityPropertyType.Rgb => DataFormatter.TryReadRgb(p.DefaultValue, out Rgb rgb) ? rgb : new(0, 0, 0),
							EntityPropertyType.Rgba => DataFormatter.TryReadRgba(p.DefaultValue, out Rgba rgba) ? rgba : new(0, 0, 0, 0),
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

		switch (entity.Shape.Value)
		{
			case Sphere sphere: RenderSphereInputs(entity.Id, sphere); break;
			case Aabb aabb: RenderAabbInputs(entity.Id, aabb); break;
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
				throw new InvalidOperationException("Entity property not found");

			OneOf<int, float> step = property.Value.Value switch
			{
				int => DataFormatter.TryReadInt(propertyDescriptor.Step, out int s) ? s : 1,
				float or Vector2 or Vector3 or Vector4 => DataFormatter.TryReadFloat(propertyDescriptor.Step, out float s) ? s : 0.1f,
				_ => 0,
			};

			OneOf<int, float> minValue = property.Value.Value switch
			{
				int => DataFormatter.TryReadInt(propertyDescriptor.MinValue, out int min) ? min : int.MinValue,
				float or Vector2 or Vector3 or Vector4 => DataFormatter.TryReadFloat(propertyDescriptor.MinValue, out float min) ? min : float.MinValue,
				_ => 0,
			};

			OneOf<int, float> maxValue = property.Value.Value switch
			{
				int => DataFormatter.TryReadInt(propertyDescriptor.MaxValue, out int max) ? max : int.MaxValue,
				float or Vector2 or Vector3 or Vector4 => DataFormatter.TryReadFloat(propertyDescriptor.MaxValue, out float max) ? max : float.MaxValue,
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

	private static void RenderSphereInputs(int entityId, Sphere sphere)
	{
		ImGui.Text("Radius");

		ImGui.DragFloat(Inline.Span($"##radius{entityId}"), ref sphere.Radius, 0.1f, 0.1f, float.MaxValue, "%.1f");
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity radius");
	}

	private static void RenderAabbInputs(int entityId, Aabb aabb)
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
