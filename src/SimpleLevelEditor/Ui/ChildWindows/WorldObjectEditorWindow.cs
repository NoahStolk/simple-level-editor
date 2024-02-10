using Detach;
using ImGuiNET;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.State;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class WorldObjectEditorWindow
{
	private static bool _proportionalScaling;

	private static readonly WorldObject _default = new()
	{
		Id = 0,
		Mesh = string.Empty,
		Position = default,
		Rotation = default,
		Scale = Vector3.One,
		Texture = string.Empty,
		Flags = [],
	};
	public static WorldObject DefaultObject { get; private set; } = _default.DeepCopy();

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

		// I have no idea why there isn't an easier way to do this.
		if (_proportionalScaling)
		{
			// This is completely insane.
			const float itemWidthXy = 104;
			const float itemWidthZ = 106;

			ImGui.PushItemWidth(itemWidthXy);
			if (ImGui.DragFloat("##scale_x", ref worldObject.Scale.X, 0.05f, 0.05f, float.MaxValue, "%.2f"))
				ScaleProportionally(ref worldObject.Scale, worldObject.Scale.X);

			ImGui.SameLine();
			if (ImGui.DragFloat("##scale_y", ref worldObject.Scale.Y, 0.05f, 0.05f, float.MaxValue, "%.2f"))
				ScaleProportionally(ref worldObject.Scale, worldObject.Scale.Y);
			ImGui.PopItemWidth();

			ImGui.SameLine();
			ImGui.PushItemWidth(itemWidthZ);
			if (ImGui.DragFloat("##scale_z", ref worldObject.Scale.Z, 0.05f, 0.05f, float.MaxValue, "%.2f"))
				ScaleProportionally(ref worldObject.Scale, worldObject.Scale.Z);
			ImGui.PopItemWidth();

			if (ImGui.IsItemDeactivatedAfterEdit())
				LevelState.Track("Changed object scale");
		}
		else
		{
			ImGui.DragFloat3("##scale", ref worldObject.Scale, 0.05f, 0.05f, float.MaxValue, "%.2f");
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

		for (int i = 0; i < worldObject.Flags.Count; i++)
		{
			string value = worldObject.Flags[i];

			if (ImGui.InputText(Inline.Span($"##flag_{worldObject.Id}_{i}"), ref value, 32))
				worldObject.Flags[i] = value;

			if (ImGui.IsItemDeactivatedAfterEdit())
				LevelState.Track("Changed object flags");

			ImGui.SameLine();
			if (ImGui.Button(Inline.Span($"X##flags{i}")))
			{
				worldObject.Flags.RemoveAt(i);
				LevelState.Track("Removed object flag");
			}
		}

		if (ImGui.Button("Add Flag"))
		{
			worldObject.Flags.Add(string.Empty);
			LevelState.Track("Added object flag");
		}

		const int rowLength = 4;

		ImGui.SeparatorText("Mesh");

		if (ImGui.BeginChild("Mesh", new(0, 280), ImGuiChildFlags.Border))
		{
			float childWidth = ImGui.GetContentRegionAvail().X;
			float tileSize = childWidth / rowLength;
			Vector2 origin = ImGui.GetCursorScreenPos();
			for (int i = 0; i < LevelState.Level.Meshes.Count; i++)
			{
				string meshName = LevelState.Level.Meshes[i];

				Vector2 localPosition = new(i % rowLength * tileSize, MathF.Floor(i / (float)rowLength) * tileSize);
				ImDrawListPtr drawList = ImGui.GetWindowDrawList();
				Vector2 cursorPos = origin + localPosition;

				MeshPreviewFramebuffer? framebuffer = MeshContainer.GetMeshPreviewFramebuffer(meshName);
				if (framebuffer != null)
				{
					const float padding = 12;
					Vector2 start = cursorPos + new Vector2(padding);
					Vector2 end = cursorPos + new Vector2(tileSize) - new Vector2(padding);
					Vector2 size = end - start;

					framebuffer.Render(size);
					drawList.AddImage((IntPtr)framebuffer.FramebufferTextureId, start, end, Vector2.UnitY, Vector2.UnitX);
				}

				if (AssetTile(meshName, cursorPos, tileSize, drawList, worldObject.Mesh == meshName))
				{
					worldObject.Mesh = meshName;
					LevelState.Track("Changed object mesh");
				}
			}

			AddScrollMarker(LevelState.Level.Meshes.Count, rowLength, tileSize);
		}

		ImGui.EndChild(); // End Mesh

		ImGui.SeparatorText("Texture");

		if (ImGui.BeginChild("Texture", new(0, 280), ImGuiChildFlags.Border))
		{
			float childWidth = ImGui.GetContentRegionAvail().X;
			float tileSize = childWidth / rowLength;
			Vector2 origin = ImGui.GetCursorScreenPos();
			for (int i = 0; i < LevelState.Level.Textures.Count; i++)
			{
				string textureName = LevelState.Level.Textures[i];

				Vector2 localPosition = new(i % rowLength * tileSize, MathF.Floor(i / (float)rowLength) * tileSize);
				ImDrawListPtr drawList = ImGui.GetWindowDrawList();
				Vector2 cursorPos = origin + localPosition;

				uint? textureId = TextureContainer.GetTexture(textureName);
				if (textureId.HasValue)
				{
					const float padding = 12;
					drawList.AddImage((IntPtr)textureId.Value, cursorPos + new Vector2(padding), cursorPos + new Vector2(tileSize) - new Vector2(padding));
				}

				if (AssetTile(textureName, cursorPos, tileSize, drawList, worldObject.Texture == textureName))
				{
					worldObject.Texture = textureName;
					LevelState.Track("Changed object texture");
				}
			}

			AddScrollMarker(LevelState.Level.Textures.Count, rowLength, tileSize);
		}

		ImGui.EndChild(); // End Texture

		static bool AssetTile(string assetName, Vector2 cursorPos, float tileSize, ImDrawListPtr drawList, bool isSelected)
		{
			ReadOnlySpan<char> displayName = Path.GetFileNameWithoutExtension(assetName.AsSpan());
			Vector2 displayNameSize = ImGui.CalcTextSize(displayName);
			Vector2 textOrigin = cursorPos + new Vector2(tileSize / 2f - displayNameSize.X / 2f, 3);
			drawList.AddRectFilled(textOrigin, textOrigin + displayNameSize, 0xB0000000);
			drawList.AddText(textOrigin, 0xFFFFFFFF, displayName);

			bool isHovered = ImGui.IsMouseHoveringRect(cursorPos, cursorPos + new Vector2(tileSize));
			drawList.AddRect(cursorPos, cursorPos + new Vector2(tileSize), GetBorderColor(isSelected, isHovered));

			return isHovered && ImGui.IsMouseClicked(ImGuiMouseButton.Left);
		}

		static uint GetBorderColor(bool isSelected, bool isHovered)
		{
			return (isSelected, isHovered) switch
			{
				(true, true) => 0xFF88FFFF,
				(true, false) => 0xFFFFFFFF,
				(false, true) => 0xFF448888,
				(false, false) => 0xFF444444,
			};
		}
	}

	private static void AddScrollMarker(int tileCount, int rowLength, float tileSize)
	{
		int rows = (int)MathF.Ceiling(tileCount / (float)rowLength);
		ImGui.InvisibleButton("scroll_marker", new(0, rows * tileSize));
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

	private static void ScaleProportionally(ref Vector3 scale, float value)
	{
		scale.X = value;
		scale.Y = value;
		scale.Z = value;
	}
}
