using Detach;
using Detach.Utils;
using ImGuiNET;
using Silk.NET.GLFW;
using SimpleLevelEditor.Content;
using SimpleLevelEditor.Logic;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.State;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class LevelEditorWindow
{
	private static readonly float[] _gridSnapPoints = { 0, 0.125f, 0.25f, 0.5f, 1, 2, 4, 8 };
	private static int _gridSnapIndex = 4;

	private static bool _isMoveXzButtonActive;
	private static bool _isMoveYButtonActive;
	private static bool _isRotationXKnobActive;
	private static bool _isRotationYKnobActive;
	private static bool _isRotationZKnobActive;

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

			Vector2 focusPointIconSize = new(16, 16);
			Vector2 focusPointIconPosition = cursorScreenPos + Camera3d.GetScreenPositionFrom3dPoint(Camera3d.FocusPointTarget, framebufferSize) - focusPointIconSize / 2;
			drawList.AddImage((IntPtr)InternalContent.Textures["FocusPoint"], focusPointIconPosition, focusPointIconPosition + focusPointIconSize);

			Vector2 cursorPosition = ImGui.GetCursorPos();

			ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0, 0, 0, 0.2f));
			if (ImGui.BeginChild("Level Editor Menu", new(280, 192), true))
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

				ImGui.SeparatorText("Objects");

				ImGui.PushItemWidth(itemWidth);
				ImGui.Checkbox("Render bounding boxes", ref LevelEditorState.RenderBoundingBoxes);
				ImGui.PopItemWidth();
			}

			ImGui.EndChild(); // End Level Editor Menu

			ImGui.PopStyleColor();

			Matrix4x4 viewProjection = Camera3d.ViewMatrix * Camera3d.Projection;
			Plane nearPlane = new(-viewProjection.M13, -viewProjection.M23, -viewProjection.M33, -viewProjection.M43);
			Vector2 mousePosition = Input.GetMousePosition() - cursorScreenPos;
			Vector2 normalizedMousePosition = new Vector2(mousePosition.X / framebufferSize.X - 0.5f, -(mousePosition.Y / framebufferSize.Y - 0.5f)) * 2;

			RenderSelectionMenu(framebufferSize, drawList, cursorScreenPos, nearPlane, normalizedMousePosition);

			ImGui.SetCursorPos(cursorPosition);
			ImGui.InvisibleButton("3d_view", framebufferSize);
			bool isFocused = ImGui.IsItemHovered();
			Camera3d.Update(ImGui.GetIO().DeltaTime, isFocused);

			MainLogic.Run(isFocused, normalizedMousePosition, nearPlane, GridSnap);
		}

		ImGui.EndChild(); // End Level Editor
	}

	private static void RenderSelectionMenu(Vector2 framebufferSize, ImDrawListPtr drawList, Vector2 cursorScreenPos, Plane nearPlane, Vector2 normalizedMousePosition)
	{
		if (LevelEditorState.SelectedWorldObject == null)
			return;

		Vector2? posOrigin = GetPosition2d(LevelEditorState.SelectedWorldObject.Position);
		if (!posOrigin.HasValue)
			return;

		Matrix4x4 rotationMatrix = MathUtils.CreateRotationMatrixFromEulerAngles(MathUtils.ToRadians(LevelEditorState.SelectedWorldObject.Rotation));
		Vector2? posX = GetPosition2d(LevelEditorState.SelectedWorldObject.Position + Vector3.Transform(Vector3.UnitX, rotationMatrix));
		Vector2? posY = GetPosition2d(LevelEditorState.SelectedWorldObject.Position + Vector3.Transform(Vector3.UnitY, rotationMatrix));
		Vector2? posZ = GetPosition2d(LevelEditorState.SelectedWorldObject.Position + Vector3.Transform(Vector3.UnitZ, rotationMatrix));

		const Keys rotationKey = Keys.R;
		const Keys scaleKey = Keys.G;

		if (!Input.IsKeyHeld(rotationKey) && !Input.IsKeyHeld(scaleKey))
		{
			bool wasActiveXz = RenderMoveButton("Move XZ", drawList, posOrigin.Value, ref _isMoveXzButtonActive);
			if (_isMoveXzButtonActive)
			{
				Vector3 targetPosition = Camera3d.GetMouseWorldPosition(normalizedMousePosition, new(Vector3.UnitY, -LevelEditorState.SelectedWorldObject.Position.Y));
				if (Vector3.Dot(targetPosition, nearPlane.Normal) + nearPlane.D < 0)
				{
					LevelEditorState.SelectedWorldObject.Position.X = GridSnap > 0 ? MathF.Round(targetPosition.X / GridSnap) * GridSnap : targetPosition.X;
					LevelEditorState.SelectedWorldObject.Position.Z = GridSnap > 0 ? MathF.Round(targetPosition.Z / GridSnap) * GridSnap : targetPosition.Z;
				}
			}

			if (wasActiveXz)
				LevelState.Track("Moved world object (hor)");

			if (posY.HasValue)
			{
				bool wasActiveY = RenderMoveButton("Move Y", drawList, posY.Value, ref _isMoveYButtonActive);
				if (_isMoveYButtonActive)
				{
					Vector3 point1 = LevelEditorState.SelectedWorldObject.Position;
					Vector3 point2 = point1 + Vector3.Transform(Vector3.UnitX, Camera3d.Rotation);
					Vector3 point3 = point1 + Vector3.Transform(Vector3.UnitY, Camera3d.Rotation);
					Plane plane = Plane.CreateFromVertices(point1, point2, point3);
					Vector3 targetPosition = Camera3d.GetMouseWorldPosition(normalizedMousePosition, plane);
					Vector3? snappedTargetPosition = Vector3.Dot(targetPosition, nearPlane.Normal) + nearPlane.D >= 0 ? null : targetPosition with
					{
						Y = GridSnap > 0 ? MathF.Round(targetPosition.Y / GridSnap) * GridSnap : targetPosition.Y,
					};

					if (snappedTargetPosition.HasValue)
					{
						LevelEditorState.SelectedWorldObject.Position.Y = snappedTargetPosition.Value.Y;
					}
				}

				if (wasActiveY)
					LevelState.Track("Moved world object (ver)");
			}

			static bool RenderMoveButton(string text, ImDrawListPtr drawList, Vector2 posOrigin, ref bool isActive)
			{
				const int padding = 6;
				Vector2 size = ImGui.CalcTextSize(text) + new Vector2(padding * 2);

				ImGui.SetCursorScreenPos(posOrigin);
				ImGui.InvisibleButton(text, size);
				bool wasActive = isActive && ImGui.IsMouseReleased(ImGuiMouseButton.Left);
				isActive = ImGui.IsItemActive();
				drawList.AddRectFilled(posOrigin, posOrigin + size, !isActive && ImGui.IsItemHovered() ? 0xff444444 : 0xff222222);
				drawList.AddRect(posOrigin, posOrigin + size, ImGui.GetColorU32(ImGuiCol.Text));
				drawList.AddText(posOrigin + new Vector2(padding), ImGui.GetColorU32(ImGuiCol.Text), text);
				return wasActive;
			}
		}

		if (posX.HasValue)
		{
			drawList.AddLine(posOrigin.Value, posX.Value, 0xff0000ff);
			ImGui.SetCursorScreenPos(posX.Value);
			RenderRotationAndScaleControls('X', ref LevelEditorState.SelectedWorldObject.Rotation.X, ref LevelEditorState.SelectedWorldObject.Scale.X, ref _isRotationXKnobActive);
		}

		if (posY.HasValue)
		{
			drawList.AddLine(posOrigin.Value, posY.Value, 0xff00ff00);
			ImGui.SetCursorScreenPos(posY.Value);
			RenderRotationAndScaleControls('Y', ref LevelEditorState.SelectedWorldObject.Rotation.Y, ref LevelEditorState.SelectedWorldObject.Scale.Y, ref _isRotationYKnobActive);
		}

		if (posZ.HasValue)
		{
			drawList.AddLine(posOrigin.Value, posZ.Value, 0xffff0000);
			ImGui.SetCursorScreenPos(posZ.Value);
			RenderRotationAndScaleControls('Z', ref LevelEditorState.SelectedWorldObject.Rotation.Z, ref LevelEditorState.SelectedWorldObject.Scale.Z, ref _isRotationZKnobActive);
		}

		static void RenderRotationAndScaleControls(char axis, ref float rotationDegrees, ref float scale, ref bool isRotationKnobActive)
		{
			if (Input.IsKeyHeld(rotationKey))
			{
				(_, bool wasActive) = ImGuiExt.KnobAngle(Inline.Span($"Rotation {axis}"), ref rotationDegrees, ref isRotationKnobActive);

				if (wasActive)
					LevelState.Track("Rotated object");
			}
			else if (Input.IsKeyHeld(scaleKey))
			{
				ImGui.PushItemWidth(80);
				ImGui.SliderFloat(Inline.Span($"Scale {axis}"), ref scale, 0.01f, 20f, "%.3f", ImGuiSliderFlags.Logarithmic);
				ImGui.PopItemWidth();

				if (ImGui.IsItemDeactivatedAfterEdit())
					LevelState.Track("Scaled object");
			}
		}

		Vector2? GetPosition2d(Vector3 position3d)
		{
			// Only render if the object is in front of the camera.
			if (Vector3.Dot(position3d, nearPlane.Normal) + nearPlane.D >= 0)
				return null;

			Vector2 position2d = Camera3d.GetScreenPositionFrom3dPoint(position3d, framebufferSize);
			if (position2d.X > 0 && position2d.X < framebufferSize.X && position2d.Y > 0 && position2d.Y < framebufferSize.Y)
				return cursorScreenPos + position2d;

			return null;
		}
	}
}
