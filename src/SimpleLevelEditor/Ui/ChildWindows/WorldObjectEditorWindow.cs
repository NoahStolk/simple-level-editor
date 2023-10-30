using Detach;
using ImGuiNET;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Model.Enums;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class WorldObjectEditorWindow
{
	private static readonly WorldObject _default = new()
	{
		Id = 0,
		Mesh = string.Empty,
		Position = default,
		Rotation = default,
		Scale = Vector3.One,
		Texture = string.Empty,
		Values = WorldObjectValues.None,
		BoundingMesh = string.Empty,
	};
	public static WorldObject DefaultObject { get; private set; } = _default.DeepCopy();

	public static void Render(Vector2 size)
	{
		if (ImGui.BeginChild("Edit World Object", size, true))
		{
			ImGui.SeparatorText("Edit World Object");

			RenderWorldObjectInputs(LevelEditorState.SelectedWorldObject ?? DefaultObject);
		}

		ImGui.EndChild(); // End Object Editor
	}

	public static void Reset()
	{
		DefaultObject = _default.DeepCopy();
	}

	private static void RenderWorldObjectInputs(WorldObject worldObject)
	{
		Vector3 position = worldObject.Position;
		if (ImGui.InputFloat3("Position", ref position, "%.3f", ImGuiInputTextFlags.CharsDecimal))
			worldObject.Position = position;

		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed object position");

		worldObject.Rotation = RenderRotationInputs("Rotation X", worldObject.Rotation, static r => r.X, static (r, f) => r with { X = f });
		worldObject.Rotation = RenderRotationInputs("Rotation Y", worldObject.Rotation, static r => r.Y, static (r, f) => r with { Y = f });
		worldObject.Rotation = RenderRotationInputs("Rotation Z", worldObject.Rotation, static r => r.Z, static (r, f) => r with { Z = f });

		Vector3 scale = worldObject.Scale;
		if (ImGui.SliderFloat3("Scale", ref scale, 0.01f, 20f, "%.3f", ImGuiSliderFlags.Logarithmic))
			worldObject.Scale = scale;

		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed object scale");

		ImGui.SameLine();
		ImGui.PushID("Scale_reset");
		if (ImGui.Button("Reset"))
		{
			worldObject.Scale = Vector3.One;
			LevelState.Track("Changed object scale");
		}

		ImGui.PopID();

		ImGui.Separator();

		for (int i = 0; i < EnumUtils.WorldObjectValuesArray.Count; i++)
		{
			WorldObjectValues value = EnumUtils.WorldObjectValuesArray[i];
			if (value == WorldObjectValues.None)
				continue;

			uint values = (uint)worldObject.Values;
			if (ImGui.CheckboxFlags(EnumUtils.WorldObjectValuesNames[value], ref values, (uint)value))
			{
				worldObject.Values = (WorldObjectValues)values;
				LevelState.Track("Changed object values");
			}
		}

		ImGui.SeparatorText("Mesh");

		if (ImGui.BeginChild("Mesh", new(0, 280), true))
			RenderAssetsGrid(LevelState.Level.Meshes, ref worldObject.Mesh);

		ImGui.EndChild(); // End Mesh

		ImGui.SeparatorText("Texture");

		if (ImGui.BeginChild("Texture", new(0, 280), true))
			RenderAssetsGrid(LevelState.Level.Textures, ref worldObject.Texture);

		ImGui.EndChild(); // End Texture
	}

	private static Vector3 RenderRotationInputs(ReadOnlySpan<char> label, Vector3 rotation, Func<Vector3, float> selector, Func<Vector3, float, Vector3> setter)
	{
		float rotationInRadians = selector(rotation);
		if (ImGui.SliderAngle(label, ref rotationInRadians, -180f, 180f))
			rotation = setter(rotation, rotationInRadians);

		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed object rotation");

		ImGui.SameLine();
		ImGui.PushID(Inline.Span($"{label}_reset"));
		if (ImGui.Button("Reset"))
		{
			rotation = setter(rotation, 0f);
			LevelState.Track("Changed object rotation");
		}

		ImGui.PopID();

		return rotation;
	}

	private static void RenderAssetsGrid(IReadOnlyList<string> items, ref string selectedItem)
	{
		const int rowLength = 4;

		if (ImGui.BeginTable("Grid", rowLength))
		{
			for (int i = 0; i < items.Count; i++)
			{
				if (i % rowLength == 0)
					ImGui.TableNextRow();

				ImGui.TableNextColumn();
				string assetName = items[i];

				if (ImGui.Selectable(Inline.Span(assetName), selectedItem == assetName, ImGuiSelectableFlags.None, new(0, 128)))
				{
					selectedItem = assetName;
					LevelState.Track("Changed object asset");
				}
			}

			ImGui.EndTable();
		}
	}
}
