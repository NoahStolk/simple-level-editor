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
		ImGui.Text("Position");
		ImGui.DragFloat3("##position", ref worldObject.Position, 0.1f, float.MinValue, float.MaxValue, "%.1f");
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed object position");

		if (RenderResetButton("Position_reset"))
		{
			worldObject.Position = Vector3.Zero;
			LevelState.Track("Changed object position");
		}

		ImGui.Text("Rotation");
		ImGui.DragFloat3("##rotation", ref worldObject.Rotation, 5f, -180, 180, "%.0f");
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed object rotation");

		if (RenderResetButton("Rotation_reset"))
		{
			worldObject.Rotation = Vector3.Zero;
			LevelState.Track("Changed object rotation");
		}

		ImGui.Text("Scale");
		ImGui.DragFloat3("##scale", ref worldObject.Scale, 0.05f, 0.05f, float.MaxValue, "%.2f");
		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed object scale");

		if (RenderResetButton("Scale_reset"))
		{
			worldObject.Scale = Vector3.One;
			LevelState.Track("Changed object scale");
		}

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

	private static bool RenderResetButton(ReadOnlySpan<char> label)
	{
		ImGui.SameLine();
		ImGui.PushID(label);
		if (ImGui.Button("Reset"))
			return true;

		ImGui.PopID();
		return false;
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

				if (ImGui.Selectable(Path.GetFileNameWithoutExtension(assetName.AsSpan()), selectedItem == assetName, ImGuiSelectableFlags.None, new(0, 128)))
				{
					selectedItem = assetName;
					LevelState.Track("Changed object asset");
				}
			}

			ImGui.EndTable();
		}
	}
}
