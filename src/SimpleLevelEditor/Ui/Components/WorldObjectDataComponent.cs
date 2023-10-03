using ImGuiNET;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Model.Enums;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Ui.Components;

public static class WorldObjectDataComponent
{
	private static Vector3 Rotation(ReadOnlySpan<char> label, Vector3 rotation, Func<Vector3, float> selector, Func<Vector3, float, Vector3> setter)
	{
		float rotationInRadians = MathUtils.ToRadians(selector(rotation));
		if (ImGui.SliderAngle(label, ref rotationInRadians, -180f, 180f))
			rotation = setter(rotation, MathUtils.ToDegrees(rotationInRadians));

		ImGui.SameLine();
		ImGui.PushID(Inline.Span($"{label}_reset"));
		if (ImGui.Button("Reset"))
			rotation = setter(rotation, 0f);

		ImGui.PopID();

		return rotation;
	}

	public static void Render(Vector2 size, WorldObject worldObject, bool includePosition)
	{
		if (includePosition)
		{
			Vector3 position = worldObject.Position;
			if (ImGui.InputFloat3("Position", ref position, "%.3f", ImGuiInputTextFlags.CharsDecimal))
				worldObject.Position = position;
		}

		worldObject.Rotation = Rotation("Rotation X", worldObject.Rotation, static r => r.X, static (r, f) => r with { X = f });
		worldObject.Rotation = Rotation("Rotation Y", worldObject.Rotation, static r => r.Y, static (r, f) => r with { Y = f });
		worldObject.Rotation = Rotation("Rotation Z", worldObject.Rotation, static r => r.Z, static (r, f) => r with { Z = f });

		Vector3 scale = worldObject.Scale;
		if (ImGui.SliderFloat3("Scale", ref scale, 0.01f, 20f, "%.3f", ImGuiSliderFlags.Logarithmic))
			worldObject.Scale = scale;

		ImGui.SameLine();
		ImGui.PushID(Inline.Span("Scale_reset"));
		if (ImGui.Button("Reset"))
			worldObject.Scale = Vector3.One;

		ImGui.PopID();

		ImGui.Separator();

		for (int i = 0; i < EnumUtils.WorldObjectValuesArray.Count; i++)
		{
			WorldObjectValues value = EnumUtils.WorldObjectValuesArray[i];
			if (value == WorldObjectValues.None)
				continue;

			uint values = (uint)worldObject.Values;
			if (ImGui.CheckboxFlags(EnumUtils.WorldObjectValuesNames[value], ref values, (uint)value))
				worldObject.Values = (WorldObjectValues)values;
		}

		ImGui.SeparatorText("Mesh");

		if (ImGui.BeginChild("Mesh", new(0, 256), true))
			AssetTilesComponent.Render(LevelState.Level.Meshes, ref worldObject.Mesh);

		ImGui.EndChild(); // End Mesh

		ImGui.SeparatorText("Texture");

		if (ImGui.BeginChild("Texture", new(0, 256), true))
			AssetTilesComponent.Render(LevelState.Level.Textures, ref worldObject.Texture);

		ImGui.EndChild(); // End Texture
	}
}
