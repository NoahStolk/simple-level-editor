using ImGuiNET;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Model.Enums;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Ui.Components;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class WorldObjectEditorWindow
{
	private static readonly WorldObject _default = new()
	{
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

			WorldObjectDataComponent.Render(ObjectEditorState.SelectedWorldObject ?? DefaultObject);
		}

		ImGui.EndChild(); // End Object Editor
	}

	public static void Reset()
	{
		DefaultObject = _default.DeepCopy();
	}
}
