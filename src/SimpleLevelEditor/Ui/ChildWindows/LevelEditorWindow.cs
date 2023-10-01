using ImGuiNET;
using Silk.NET.GLFW;
using SimpleLevelEditor.Maths;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Model.Enums;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class LevelEditorWindow
{
	private static readonly float[] _gridSnapPoints = { 0, 0.125f, 0.25f, 0.5f, 1, 2, 4, 8 };
	private static int _gridSnapIndex = 4;

	private static float GridSnap => _gridSnapIndex >= 0 && _gridSnapIndex < _gridSnapPoints.Length ? _gridSnapPoints[_gridSnapIndex] : 0;

	public static void Render(Vector2 size)
	{
		Vector2 framebufferSize = size - new Vector2(0, 32);

		SceneFramebuffer.Initialize(framebufferSize);
		Camera3d.AspectRatio = framebufferSize.X / framebufferSize.Y;

		if (ImGui.BeginChild("Level Editor", size, true))
		{
			ImGui.SeparatorText("Level Editor");

			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			SceneFramebuffer.RenderFramebuffer(framebufferSize);

			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			drawList.AddImage((IntPtr)SceneFramebuffer.FramebufferTextureId, cursorScreenPos, cursorScreenPos + framebufferSize, Vector2.UnitY, Vector2.UnitX);

			Vector2 cursorPosition = ImGui.GetCursorPos();

			ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0, 0, 0, 0.2f));
			if (ImGui.BeginChild("Level Editor Menu", new(280, 160), true))
			{
				const int itemWidth = 160;

				ImGui.SeparatorText("Grid");

				ImGui.PushItemWidth(itemWidth);
				ImGui.SliderInt("Grid Snap", ref _gridSnapIndex, 0, _gridSnapPoints.Length - 1, Inline.Span(GridSnap));
				ImGui.SliderInt("Cells per side", ref LevelEditorState.GridCellCount, 1, 64);
				ImGui.SliderInt("Cell size", ref LevelEditorState.GridCellSize, 1, 4);
				ImGui.PopItemWidth();

				ImGui.SeparatorText("Height");

				ImGui.PushItemWidth(itemWidth);
				ImGui.InputFloat("Height", ref LevelEditorState.TargetHeight, 0.25f, 1, "%.2f");
				ImGui.PopItemWidth();
			}

			ImGui.EndChild(); // End Level Editor Menu

			if (ImGui.BeginChild("Mode", new(32, 96), true))
			{
				for (int i = 0; i < EnumUtils.LevelEditorModeArray.Count; i++)
				{
					LevelEditorMode mode = EnumUtils.LevelEditorModeArray[i];
					ImGui.PushStyleColor(ImGuiCol.Button, mode == LevelEditorState.Mode ? new Vector4(0.5f, 0.2f, 0.2f, 1) : new(0.1f, 0.1f, 0.1f, 1));

					// TODO: Use images instead of a single letter.
					if (ImGui.Button(Inline.Span(EnumUtils.LevelEditorModeNames[mode][0]), new(24)))
						LevelEditorState.Mode = mode;

					if (ImGui.IsItemHovered(ImGuiHoveredFlags.DelayNone))
					{
						string shortcut = EnumUtils.LevelEditorModeShortcuts[mode];
						ImGui.SetTooltip(Inline.Span($"{Shortcuts.GetDescription(shortcut)} - shortcut: {Shortcuts.GetKeyDescription(shortcut)}"));
					}

					ImGui.PopStyleColor();
				}
			}

			ImGui.EndChild(); // End Mode

			ImGui.PopStyleColor();

			ImGui.SetCursorPos(cursorPosition);
			ImGui.InvisibleButton("3d_view", framebufferSize);
			bool isFocused = ImGui.IsItemHovered();
			Camera3d.Update(ImGui.GetIO().DeltaTime, isFocused);

			Vector2 mousePosition = Input.GetMousePosition() - cursorScreenPos;
			Vector2 normalizedMousePosition = new Vector2(mousePosition.X / framebufferSize.X - 0.5f, -(mousePosition.Y / framebufferSize.Y - 0.5f)) * 2;
			CalculateTargetPosition(normalizedMousePosition);
			CalculateHighlightedObject(normalizedMousePosition, isFocused);

			if (isFocused && Input.IsButtonPressed(MouseButton.Left))
				OnLeftClick();
		}

		ImGui.EndChild(); // End Level Editor
	}

	private static void OnLeftClick()
	{
		switch (LevelEditorState.Mode)
		{
			case LevelEditorMode.AddWorldObjects:
				if (ObjectCreatorState.SelectedMeshName != null && !LevelState.Level.WorldObjects.Exists(wo => wo.Position == LevelEditorState.TargetPosition))
				{
					WorldObject worldObject = new()
					{
						Position = LevelEditorState.TargetPosition,
						Mesh = ObjectCreatorState.SelectedMeshName,
						Texture = string.Empty,
						Scale = new(1),
						Rotation = new(0),
						BoundingMesh = string.Empty,
						Values = WorldObjectValues.None,
					};
					LevelState.Level.WorldObjects.Add(worldObject);
				}

				break;
			case LevelEditorMode.EditWorldObjects:
				if (LevelEditorState.HighlightedObject != null)
				{
					ObjectEditorState.SelectedWorldObject = ObjectEditorState.SelectedWorldObject == LevelEditorState.HighlightedObject ? null : LevelEditorState.HighlightedObject;
				}

				break;
		}
	}

	private static void CalculateTargetPosition(Vector2 normalizedMousePosition)
	{
		LevelEditorState.TargetPosition = Camera3d.GetMouseWorldPosition(normalizedMousePosition, new(Vector3.UnitY, -LevelEditorState.TargetHeight));
		if (GridSnap > 0)
		{
			LevelEditorState.TargetPosition.X = MathF.Round(LevelEditorState.TargetPosition.X / GridSnap) * GridSnap;
			LevelEditorState.TargetPosition.Z = MathF.Round(LevelEditorState.TargetPosition.Z / GridSnap) * GridSnap;
		}
	}

	private static void CalculateHighlightedObject(Vector2 normalizedMousePosition, bool isFocused)
	{
		Matrix4x4 viewProjection = Camera3d.ViewMatrix * Camera3d.Projection;
		Plane farPlane = new(viewProjection.M13 - viewProjection.M14, viewProjection.M23 - viewProjection.M24, viewProjection.M33 - viewProjection.M34, viewProjection.M43 - viewProjection.M44);
		Vector3 rayEndPosition = Camera3d.GetMouseWorldPosition(normalizedMousePosition, farPlane);
		Ray ray = new(Camera3d.Position, Vector3.Normalize(rayEndPosition - Camera3d.Position));
		Vector3? closestIntersection = null;
		LevelEditorState.HighlightedObject = null;

		if (!isFocused)
			return;

		if (Input.IsButtonHeld(Camera3d.LookButton))
			return;

		for (int i = 0; i < LevelState.Level.WorldObjects.Count; i++)
		{
			WorldObject worldObject = LevelState.Level.WorldObjects[i];
			MeshContainer.Entry? mesh = MeshContainer.GetMesh(worldObject.Mesh);
			if (mesh == null)
				continue;

			Vector3 bbScale = worldObject.Scale * (mesh.BoundingMax - mesh.BoundingMin);
			Vector3 bbOffset = (mesh.BoundingMax + mesh.BoundingMin) / 2;
			float maxScale = Math.Max(bbScale.X, Math.Max(bbScale.Y, bbScale.Z));
			Sphere sphere = new(worldObject.Position + bbOffset, maxScale);
			Vector3? sphereIntersection = ray.Intersects(sphere);
			if (sphereIntersection == null)
				continue;

			Matrix4x4 modelMatrix = Matrix4x4.CreateScale(worldObject.Scale) * MathUtils.CreateRotationMatrixFromEulerAngles(worldObject.Rotation) * Matrix4x4.CreateTranslation(worldObject.Position);
			for (int j = 0; j < mesh.Mesh.Indices.Length; j += 3)
			{
				Vector3 p1 = Vector3.Transform(mesh.Mesh.Vertices[mesh.Mesh.Indices[j]].Position, modelMatrix);
				Vector3 p2 = Vector3.Transform(mesh.Mesh.Vertices[mesh.Mesh.Indices[j + 1]].Position, modelMatrix);
				Vector3 p3 = Vector3.Transform(mesh.Mesh.Vertices[mesh.Mesh.Indices[j + 2]].Position, modelMatrix);

				Vector3? triangleIntersection = ray.Intersects(p1, p2, p3);
				if (triangleIntersection == null)
					continue;

				if (closestIntersection == null || Vector3.DistanceSquared(Camera3d.Position, triangleIntersection.Value) < Vector3.DistanceSquared(Camera3d.Position, closestIntersection.Value))
				{
					closestIntersection = triangleIntersection.Value;
					LevelEditorState.HighlightedObject = worldObject;
				}
			}
		}
	}
}
