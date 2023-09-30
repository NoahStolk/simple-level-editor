using ImGuiNET;
using SimpleLevelEditor.Model.Enums;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class ObjectEditorWindow
{
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

				Vector3 rotation = ObjectEditorState.SelectedWorldObject.Rotation;
				if (ImGui.InputFloat3("Rotation", ref rotation, "%.3f", ImGuiInputTextFlags.CharsDecimal))
					ObjectEditorState.SelectedWorldObject.Rotation = rotation;

				Vector3 scale = ObjectEditorState.SelectedWorldObject.Scale;
				if (ImGui.InputFloat3("Scale", ref scale, "%.3f", ImGuiInputTextFlags.CharsDecimal))
					ObjectEditorState.SelectedWorldObject.Scale = scale;

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
			}
			else
			{
				ImGui.Text("None selected.");
			}
		}

		ImGui.EndChild(); // End Object Editor
	}
}
