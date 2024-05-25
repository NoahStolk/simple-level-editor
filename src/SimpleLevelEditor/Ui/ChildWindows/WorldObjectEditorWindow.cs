using Detach;
using ImGuiNET;
using SimpleLevelEditor.Formats.Types.Level;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.State;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class WorldObjectEditorWindow
{
	private static bool _proportionalScaling;

	public static WorldObject DefaultObject { get; private set; } = WorldObject.CreateDefault();

	public static void Render()
	{
		if (ImGui.BeginChild("Edit World Object", default, ImGuiChildFlags.Border))
		{
			ImGui.SeparatorText("Edit World Object");

			RenderWorldObjectInputs(LevelEditorState.SelectedWorldObject ?? DefaultObject);
		}

		ImGui.EndChild(); // End Object Editor
	}

	public static void Reset()
	{
		DefaultObject = WorldObject.CreateDefault();
	}

	private static void RenderWorldObjectInputs(WorldObject worldObject)
	{
		ImGui.Text("Position");

		Vector3 position = worldObject.Position;
		if (ImGui.DragFloat3("##position", ref position, 0.1f, float.MinValue, float.MaxValue, "%.1f"))
			worldObject.Position = position;

		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed object position");

		if (RenderResetButton("Position_reset"))
		{
			worldObject.Position = Vector3.Zero;
			LevelState.Track("Changed object position");
		}

		ImGui.Text("Rotation");

		Vector3 rotation = worldObject.Rotation;
		if (ImGui.DragFloat3("##rotation", ref rotation, 5f, -180, 180, "%.0f"))
			worldObject.Rotation = rotation;

		if (ImGui.IsItemDeactivatedAfterEdit())
			LevelState.Track("Changed object rotation");

		if (RenderResetButton("Rotation_reset"))
		{
			worldObject.Rotation = Vector3.Zero;
			LevelState.Track("Changed object rotation");
		}

		ImGui.Text("Scale");

		if (_proportionalScaling)
		{
			Vector3 scale = worldObject.Scale;
			if (ImGui.DragFloat("##scale_proportional", ref scale.X, 0.05f, 0.05f, float.MaxValue, "%.2f"))
				worldObject.Scale = new Vector3(scale.X);

			if (ImGui.IsItemDeactivatedAfterEdit())
				LevelState.Track("Changed object scale");
		}
		else
		{
			Vector3 scale = worldObject.Scale;
			if (ImGui.DragFloat3("##scale", ref scale, 0.05f, 0.05f, float.MaxValue, "%.2f"))
				worldObject.Scale = scale;

			if (ImGui.IsItemDeactivatedAfterEdit())
				LevelState.Track("Changed object scale");
		}

		if (RenderResetButton("Scale_reset"))
		{
			worldObject.Scale = Vector3.One;
			LevelState.Track("Changed object scale");
		}

		ImGui.SameLine();
		ImGui.Checkbox("Proportional", ref _proportionalScaling);

		ImGui.Separator();

		for (int i = 0; i < worldObject.Flags.Length; i++)
		{
			string value = worldObject.Flags[i];

			if (ImGui.InputText(Inline.Span($"##flag_{worldObject.Id}_{i}"), ref value, 32))
				worldObject.ChangeFlagAtIndex(i, value);

			if (ImGui.IsItemDeactivatedAfterEdit())
				LevelState.Track("Changed object flags");

			ImGui.SameLine();
			if (ImGui.Button(Inline.Span($"X##flags{i}")))
			{
				worldObject.RemoveFlagAtIndex(i);
				LevelState.Track("Removed object flag");
			}
		}

		if (ImGui.Button("Add Flag"))
		{
			worldObject.AddFlag(string.Empty);
			LevelState.Track("Added object flag");
		}

		const int rowLength = 4;
		const float padding = 3;

		ImGui.SeparatorText("Model");

		Vector2 availableSize = ImGui.GetContentRegionAvail();
		if (ImGui.BeginChild("Model", availableSize, ImGuiChildFlags.Border))
		{
			float childWidth = ImGui.GetContentRegionAvail().X;
			float tileSize = childWidth / rowLength - padding * rowLength;
			for (int i = 0; i < LevelState.Level.Models.Length; i++)
			{
				string meshName = LevelState.Level.Models[i];

				ModelPreviewFramebuffer? framebuffer = ModelContainer.LevelContainer.GetModelPreviewFramebuffer(meshName);
				if (framebuffer != null)
				{
					framebuffer.Render(GetBorderColor(worldObject.ModelPath == meshName), new Vector2(tileSize));
					AssetTile(worldObject, i, (IntPtr)framebuffer.FramebufferTextureId, rowLength, tileSize, meshName);
				}
			}

			AddScrollMarker(LevelState.Level.Models.Length, rowLength, tileSize);
		}

		ImGui.EndChild(); // End Mesh

		static Vector4 GetBorderColor(bool isSelected)
		{
			return isSelected switch
			{
				true => new Vector4(0.3f, 0.7f, 0.3f, 1),
				false => new Vector4(0.3f, 0.3f, 0.3f, 1),
			};
		}

		static void AssetTile(WorldObject worldObject, int i, IntPtr textureId, int rowLength, float tileSize, string meshName)
		{
			if (i % rowLength != 0)
				ImGui.SameLine();

			if (ImGui.ImageButton(Inline.Span($"Image{i}"), textureId, new Vector2(tileSize), Vector2.UnitY, Vector2.UnitX))
			{
				worldObject.ModelPath = meshName;
				LevelState.Track("Changed world object model");
			}
		}
	}

	private static void AddScrollMarker(int tileCount, int rowLength, float tileSize)
	{
		int rows = (int)MathF.Ceiling(tileCount / (float)rowLength);
		ImGui.InvisibleButton("scroll_marker", new Vector2(0, rows * tileSize));
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
