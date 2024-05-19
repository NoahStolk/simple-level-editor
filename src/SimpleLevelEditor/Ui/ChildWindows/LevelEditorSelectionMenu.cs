using ImGuiNET;
using Silk.NET.GLFW;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.State;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class LevelEditorSelectionMenu
{
	private static bool _isMoveButtonActive;

	public static void RenderSelectionMenu(Vector2 framebufferSize, ImDrawListPtr drawList, Vector2 cursorScreenPos, Plane nearPlane, Vector2 normalizedMousePosition, float gridSnap)
	{
		Vector3? objectPosition = LevelEditorState.SelectedWorldObject?.Position ?? LevelEditorState.SelectedEntity?.Position;
		if (!objectPosition.HasValue)
			return;

		Vector2? buttonPosition = GetPosition2d(framebufferSize, cursorScreenPos, nearPlane, objectPosition.Value);
		if (!buttonPosition.HasValue)
			return;

		MoveActionResult result = RenderSelectionMenu(drawList, buttonPosition.Value, nearPlane, normalizedMousePosition, gridSnap, objectPosition.Value);

		if (result.Apply && LevelEditorState.MoveTargetPosition.HasValue)
		{
			if (LevelEditorState.SelectedWorldObject != null)
			{
				LevelEditorState.SelectedWorldObject.Position = LevelEditorState.MoveTargetPosition.Value;
				LevelState.Track("Moved world object");
			}
			else if (LevelEditorState.SelectedEntity != null)
			{
				LevelEditorState.SelectedEntity.Position = LevelEditorState.MoveTargetPosition.Value;
				LevelState.Track("Moved entity");
			}
		}

		if (result.IsMoving)
			LevelEditorState.MoveTargetPosition = result.NewPosition;
		else
			LevelEditorState.MoveTargetPosition = null;
	}

	private static MoveActionResult RenderSelectionMenu(ImDrawListPtr drawList, Vector2 buttonPosition, Plane nearPlane, Vector2 normalizedMousePosition, float gridSnap, Vector3 objectPosition)
	{
		bool wasMoveButtonActive = RenderMoveButton("Move", drawList, buttonPosition, ref _isMoveButtonActive);
		if (_isMoveButtonActive)
		{
			bool ctrl = Input.GlfwInput.IsKeyDown(Keys.ControlLeft) || Input.GlfwInput.IsKeyDown(Keys.ControlRight);
			if (ctrl)
			{
				Vector3 point1 = objectPosition;
				Vector3 point2 = point1 + Vector3.Transform(Vector3.UnitX, Camera3d.Rotation);
				Vector3 point3 = point1 + Vector3.Transform(Vector3.UnitY, Camera3d.Rotation);
				Plane plane = Plane.CreateFromVertices(point1, point2, point3);
				Vector3 targetPosition = Camera3d.GetMouseWorldPosition(normalizedMousePosition, plane);
				Vector3? snappedTargetPosition = Vector3.Dot(targetPosition, nearPlane.Normal) + nearPlane.D >= 0 ? null : targetPosition with
				{
					Y = gridSnap > 0 ? MathF.Round(targetPosition.Y / gridSnap) * gridSnap : targetPosition.Y,
				};

				if (snappedTargetPosition.HasValue)
				{
					objectPosition.Y = snappedTargetPosition.Value.Y;
				}
			}
			else
			{
				Vector3 targetPosition = Camera3d.GetMouseWorldPosition(normalizedMousePosition, new Plane(Vector3.UnitY, -objectPosition.Y));
				if (Vector3.Dot(targetPosition, nearPlane.Normal) + nearPlane.D < 0)
				{
					objectPosition.X = gridSnap > 0 ? MathF.Round(targetPosition.X / gridSnap) * gridSnap : targetPosition.X;
					objectPosition.Z = gridSnap > 0 ? MathF.Round(targetPosition.Z / gridSnap) * gridSnap : targetPosition.Z;
				}
			}
		}

		return new MoveActionResult(_isMoveButtonActive, wasMoveButtonActive, objectPosition);
	}

	private static bool RenderMoveButton(string text, ImDrawListPtr drawList, Vector2 posOrigin, ref bool isActive)
	{
		const int padding = 6;
		Vector2 size = ImGui.CalcTextSize(text) + new Vector2(padding * 2);
		posOrigin -= size / 2;

		ImGui.SetCursorScreenPos(posOrigin);
		ImGui.InvisibleButton(text, size);
		bool wasActive = isActive && ImGui.IsMouseReleased(ImGuiMouseButton.Left);
		isActive = ImGui.IsItemActive();
		drawList.AddRectFilled(posOrigin, posOrigin + size, !isActive && ImGui.IsItemHovered() ? 0xff444444 : 0xff222222);
		drawList.AddRect(posOrigin, posOrigin + size, ImGui.GetColorU32(ImGuiCol.Text));
		drawList.AddText(posOrigin + new Vector2(padding), ImGui.GetColorU32(ImGuiCol.Text), text);
		return wasActive;
	}

	private static Vector2? GetPosition2d(Vector2 framebufferSize, Vector2 cursorScreenPos, Plane nearPlane, Vector3 position3d)
	{
		// Only render if the object is in front of the camera.
		if (Vector3.Dot(position3d, nearPlane.Normal) + nearPlane.D >= 0)
			return null;

		Vector2 position2d = Camera3d.GetScreenPositionFrom3dPoint(position3d, framebufferSize);
		if (position2d.X > 0 && position2d.X < framebufferSize.X && position2d.Y > 0 && position2d.Y < framebufferSize.Y)
			return cursorScreenPos + position2d;

		return null;
	}

	private record struct MoveActionResult(bool IsMoving, bool Apply, Vector3 NewPosition);
}
