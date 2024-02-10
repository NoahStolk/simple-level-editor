using Detach;
using ImGuiNET;
using OneOf;
using SimpleLevelEditor.Data;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Model.EntityShapes;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class EntityEditorWindow
{
	private static readonly Dictionary<Type, string> _typeNames = new()
	{
		{ typeof(bool), "Boolean" },
		{ typeof(int), "Integer" },
		{ typeof(float), "Float" },
		{ typeof(Vector2), "Float (2)" },
		{ typeof(Vector3), "Float (3)" },
		{ typeof(Vector4), "Float (4)" },
		{ typeof(string), "Text" },
		{ typeof(Rgb), "RGB" },
		{ typeof(Rgba), "RGBA" },
	};

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
		ImGui.InputText(Inline.Span($"Name##{entity.Id}"), ref entity.Name, 32);
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity name");

		ImGui.Text("Position");

		ImGui.DragFloat3("##position", ref entity.Position, 0.1f, float.MinValue, float.MaxValue, "%.1f");
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed entity position");

		if (RenderResetButton(Inline.Span($"Position_reset##{entity.Id}")))
		{
			entity.Position = Vector3.Zero;
			LevelState.Track("Changed entity position");
		}

		ImGui.SeparatorText("Shape");

		if (ImGui.BeginCombo(Inline.Span($"Shape Type##{entity.Id}"), entity.Shape.GetType().Name))
		{
			if (ImGui.Selectable(Inline.Span($"Point##{entity.Id}"), entity.Shape is Point))
				entity.Shape = new Point();

			if (ImGui.Selectable(Inline.Span($"Sphere##{entity.Id}"), entity.Shape is Sphere))
				entity.Shape = new Sphere(2);

			if (ImGui.Selectable(Inline.Span($"Aabb##{entity.Id}"), entity.Shape is Aabb))
				entity.Shape = new Aabb(-Vector3.One, Vector3.One);

			ImGui.EndCombo();
		}

		switch (entity.Shape)
		{
			case Sphere sphere: RenderSphereInputs(entity.Id, sphere); break;
			case Aabb aabb: RenderAabbInputs(entity.Id, aabb); break;
		}

		ImGui.SeparatorText("Properties");

		for (int i = 0; i < entity.Properties.Count; i++)
		{
			EntityProperty property = entity.Properties[i];

			if (ImGui.InputText(Inline.Span($"Property Key##{entity.Id}_{i}"), ref property.Key, 32))
				entity.Properties[i] = property;

			ImGui.SameLine();
			ImGui.Text("(?)");
			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("Property names must begin with a letter.");

			if (ImGui.IsItemDeactivatedAfterEdit())
				LevelState.Track("Changed entity property key name");

			if (ImGui.BeginCombo(Inline.Span($"Property Type##{entity.Id}_{i}"), _typeNames[property.Value.Value.GetType()]))
			{
				Selectable<bool>(false);
				Selectable<int>(0);
				Selectable<float>(0f);
				Selectable<Vector2>(Vector2.Zero);
				Selectable<Vector3>(Vector3.Zero);
				Selectable<Vector4>(Vector4.Zero);
				Selectable<string>(string.Empty);
				Selectable<Rgb>(new Rgb(0, 0, 0));
				Selectable<Rgba>(new Rgba(0, 0, 0, 0));

				entity.Properties[i] = property;

				ImGui.EndCombo();

				void Selectable<T>(OneOf<bool, int, float, Vector2, Vector3, Vector4, string, Rgb, Rgba> defaultValue)
				{
					if (ImGui.Selectable(_typeNames[typeof(T)], property.Value.Value is T))
					{
						property.Value = defaultValue;
						LevelState.Track("Changed entity property value type");
					}
				}
			}

			entity.Properties[i].Value = property.Value.Value switch
			{
				bool b when ImGui.Checkbox(Inline.Span($"##property_value{entity.Id}_{i}"), ref b) => b,
				int int32 when ImGui.DragInt(Inline.Span($"##property_value{entity.Id}_{i}"), ref int32) => int32,
				float f when ImGui.DragFloat(Inline.Span($"##property_value{entity.Id}_{i}"), ref f) => f,
				Vector2 v2 when ImGui.DragFloat2(Inline.Span($"##property_value{entity.Id}_{i}"), ref v2) => v2,
				Vector3 v3 when ImGui.DragFloat3(Inline.Span($"##property_value{entity.Id}_{i}"), ref v3) => v3,
				Vector4 v4 when ImGui.DragFloat4(Inline.Span($"##property_value{entity.Id}_{i}"), ref v4) => v4,
				string s when ImGui.InputText(Inline.Span($"##property_value{entity.Id}_{i}"), ref s, 32) => s,
				Rgb rgb when ImGuiUtils.ColorEdit3Rgb(Inline.Span($"##property_value{entity.Id}_{i}"), ref rgb) => rgb,
				Rgba rgba when ImGuiUtils.ColorEdit4Rgba(Inline.Span($"##property_value{entity.Id}_{i}"), ref rgba) => rgba,
				_ => entity.Properties[i].Value,
			};

			if (ImGui.IsItemDeactivatedAfterEdit())
				LevelState.Track("Changed entity property value");

			if (ImGui.Button(Inline.Span($"Delete Property##{entity.Id}_{i}")))
			{
				entity.Properties.RemoveAt(i);
				LevelState.Track("Removed entity property");
			}

			ImGui.Separator();
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
