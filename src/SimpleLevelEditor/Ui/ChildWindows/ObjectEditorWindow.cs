using ImGuiNET;
using SimpleLevelEditor.Model.Enums;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class ObjectEditorWindow
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

	public static void Render(Vector2 size)
	{
		if (ImGui.BeginChild("Object Editor", size, true))
		{
			ImGui.SeparatorText("Object Editor");

			if (ObjectEditorState.SelectedWorldObject != null)
			{
				Vector3 position = ObjectEditorState.SelectedWorldObject.Position;
				if (ImGui.InputFloat3("Position", ref position, "%.3f", ImGuiInputTextFlags.CharsDecimal))
					ObjectEditorState.SelectedWorldObject.Position = position;

				ObjectEditorState.SelectedWorldObject.Rotation = Rotation("Rotation X", ObjectEditorState.SelectedWorldObject.Rotation, r => r.X, (r, f) => r with { X = f });
				ObjectEditorState.SelectedWorldObject.Rotation = Rotation("Rotation Y", ObjectEditorState.SelectedWorldObject.Rotation, r => r.Y, (r, f) => r with { Y = f });
				ObjectEditorState.SelectedWorldObject.Rotation = Rotation("Rotation Z", ObjectEditorState.SelectedWorldObject.Rotation, r => r.Z, (r, f) => r with { Z = f });

				Vector3 scale = ObjectEditorState.SelectedWorldObject.Scale;
				if (ImGui.SliderFloat3("Scale", ref scale, 0.01f, 20f, "%.3f", ImGuiSliderFlags.Logarithmic))
					ObjectEditorState.SelectedWorldObject.Scale = scale;

				ImGui.SameLine();
				ImGui.PushID(Inline.Span("Scale_reset"));
				if (ImGui.Button("Reset"))
					ObjectEditorState.SelectedWorldObject.Scale = Vector3.One;

				ImGui.PopID();

				ImGui.Separator();

				uint values = (uint)ObjectEditorState.SelectedWorldObject.Values;
				for (int i = 0; i < EnumUtils.WorldObjectValuesArray.Count; i++)
				{
					WorldObjectValues value = EnumUtils.WorldObjectValuesArray[i];
					if (value == WorldObjectValues.None)
						continue;

					if (ImGui.CheckboxFlags(EnumUtils.WorldObjectValuesNames[value], ref values, (uint)value))
					{
						ObjectEditorState.SelectedWorldObject.Values = (WorldObjectValues)values;
						break;
					}
				}

				ImGui.SeparatorText("Mesh");

				if (ImGui.BeginChild("Mesh", new(0, 256), true))
				{
					foreach (string item in LevelState.Level.Meshes)
					{
						if (ImGui.Button(item))
							ObjectEditorState.SelectedWorldObject.Mesh = item;
					}
				}

				ImGui.EndChild(); // End Mesh

				ImGui.SeparatorText("Texture");

				if (ImGui.BeginChild("Texture", new(0, 256), true))
				{
					foreach (string item in LevelState.Level.Textures)
					{
						if (ImGui.Button(item))
							ObjectEditorState.SelectedWorldObject.Texture = item;
					}
				}

				ImGui.EndChild(); // End Texture

				ImGui.Separator();

				ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(1, 0, 0, 1));
				if (ImGui.Button("Delete"))
				{
					LevelState.Level.WorldObjects.Remove(ObjectEditorState.SelectedWorldObject);
					ObjectEditorState.SelectedWorldObject = null;
				}

				ImGui.PopStyleColor();
			}
			else
			{
				ImGui.Text("None selected.");
			}
		}

		ImGui.EndChild(); // End Object Editor
	}
}
