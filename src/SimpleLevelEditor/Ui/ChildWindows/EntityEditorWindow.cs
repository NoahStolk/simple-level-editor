using Detach;
using ImGuiNET;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Model.EntityTypes;
using SimpleLevelEditor.State;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class EntityEditorWindow
{
	private static readonly Entity _default = new()
	{
		Id = 0,
		Name = string.Empty,
		Properties = new(),
		Shape = new Point(default),
	};
	public static Entity DefaultEntity { get; private set; } = _default.DeepCopy();

	public static void Render(Vector2 size)
	{
		if (ImGui.BeginChild("Edit Entity", size, true))
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
		ImGui.InputText("Name", ref entity.Name, 32);
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity name");

		if (ImGui.BeginCombo("Entity type", entity.Shape.Value.GetType().Name))
		{
			if (ImGui.Selectable("Point", entity.Shape.Value is Point))
				entity.Shape = new Point(default);

			if (ImGui.Selectable("Sphere", entity.Shape.Value is Sphere))
				entity.Shape = new Sphere(default, 2);

			if (ImGui.Selectable("Aabb", entity.Shape.Value is Aabb))
				entity.Shape = new Aabb(-Vector3.One, Vector3.One);

			ImGui.EndCombo();
		}

		switch (entity.Shape.Value)
		{
			case Point point: RenderPointInputs(point); break;
			case Sphere sphere: RenderSphereInputs(sphere); break;
			case Aabb aabb: RenderAabbInputs(aabb); break;
		}

		ImGui.SeparatorText("Properties");

		for (int i = 0; i < entity.Properties.Count; i++)
		{
			EntityProperty property = entity.Properties[i];

			if (ImGui.InputText($"##property_key{i}", ref property.Key, 32))
				entity.Properties[i] = property;

			ImGui.SameLine();
			if (ImGui.Button($"X##property_remove{i}"))
			{
				entity.Properties.RemoveAt(i);
				LevelState.Track("Removed entity property");
			}

			if (ImGui.IsItemDeactivatedAfterEdit())
				LevelState.Track("Changed entity property key name");

			if (ImGui.BeginCombo(Inline.Span($"Property type##{i}"), property.Value.Value.GetType().Name))
			{
				if (ImGui.Selectable("Boolean", property.Value.Value is bool))
					property.Value = false;

				if (ImGui.Selectable("Integer", property.Value.Value is int))
					property.Value = 0;

				if (ImGui.Selectable("Float", property.Value.Value is float))
					property.Value = 0f;

				if (ImGui.Selectable("Float (2)", property.Value.Value is Vector2))
					property.Value = Vector2.Zero;

				if (ImGui.Selectable("Float (3)", property.Value.Value is Vector3))
					property.Value = Vector3.Zero;

				if (ImGui.Selectable("Float (4)", property.Value.Value is Vector4))
					property.Value = Vector4.Zero;

				if (ImGui.Selectable("Text", property.Value.Value is string))
					property.Value = string.Empty;

				entity.Properties[i] = property;

				ImGui.EndCombo();
			}

			entity.Properties[i].Value = property.Value.Value switch
			{
				bool b when ImGui.Checkbox(Inline.Span($"##property_value{i}"), ref b) => b,
				int int32 when ImGui.DragInt(Inline.Span($"##property_value{i}"), ref int32) => int32,
				float f when ImGui.DragFloat(Inline.Span($"##property_value{i}"), ref f) => f,
				Vector2 v2 when ImGui.DragFloat2(Inline.Span($"##property_value{i}"), ref v2) => v2,
				Vector3 v3 when ImGui.DragFloat3(Inline.Span($"##property_value{i}"), ref v3) => v3,
				Vector4 v4 when ImGui.DragFloat4(Inline.Span($"##property_value{i}"), ref v4) => v4,
				string s when ImGui.InputText(Inline.Span($"##property_value{i}"), ref s, 32) => s,
				_ => entity.Properties[i].Value,
			};

			ImGui.Spacing();
		}

		if (ImGui.Button("Add Property"))
		{
			entity.Properties.Add(new()
			{
				Key = string.Empty,
				Value = false,
			});
			LevelState.Track("Added entity property");
		}
	}

	private static void RenderPointInputs(Point point)
	{
		ImGui.Text("Position");

		ImGui.DragFloat3("##position", ref point.Position, 0.1f, float.MinValue, float.MaxValue, "%.1f");
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity position");

		if (RenderResetButton("Position_reset"))
		{
			point.Position = Vector3.Zero;
			LevelState.Track("Changed entity position");
		}
	}

	private static void RenderSphereInputs(Sphere sphere)
	{
		ImGui.Text("Position");

		ImGui.DragFloat3("##position", ref sphere.Position, 0.1f, float.MinValue, float.MaxValue, "%.1f");
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity position");

		if (RenderResetButton("Position_reset"))
		{
			sphere.Position = Vector3.Zero;
			LevelState.Track("Changed entity position");
		}

		ImGui.Text("Radius");

		ImGui.DragFloat("##radius", ref sphere.Radius, 0.1f, float.MinValue, float.MaxValue, "%.1f");
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity radius");

		if (RenderResetButton("Radius_reset"))
		{
			sphere.Radius = 0;
			LevelState.Track("Changed entity radius");
		}
	}

	private static void RenderAabbInputs(Aabb aabb)
	{
		ImGui.Text("Box Min");

		ImGui.DragFloat3("##box_min", ref aabb.Min, 0.1f, float.MinValue, float.MaxValue, "%.1f");
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity box min");

		if (RenderResetButton("Box_min_reset"))
		{
			aabb.Min = Vector3.Zero;
			LevelState.Track("Changed entity box min");
		}

		ImGui.Text("Box Max");

		ImGui.DragFloat3("##box_max", ref aabb.Max, 0.1f, float.MinValue, float.MaxValue, "%.1f");
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity box max");

		if (RenderResetButton("Box_max_reset"))
		{
			aabb.Max = Vector3.Zero;
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
