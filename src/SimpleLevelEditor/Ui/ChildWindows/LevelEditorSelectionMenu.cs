using Detach.Utils;
using ImGuiGlfw;
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
		if (LevelEditorState.SelectedWorldObject != null)
			RenderSelectionMenu(framebufferSize, drawList, cursorScreenPos, nearPlane, normalizedMousePosition, gridSnap, ref LevelEditorState.SelectedWorldObject.Position, LevelEditorState.SelectedWorldObject.Rotation, "world object");
		else if (LevelEditorState.SelectedEntity != null)
			RenderSelectionMenu(framebufferSize, drawList, cursorScreenPos, nearPlane, normalizedMousePosition, gridSnap, ref LevelEditorState.SelectedEntity.Position, Vector3.Zero, "entity");
	}

	private static void RenderSelectionMenu(Vector2 framebufferSize, ImDrawListPtr drawList, Vector2 cursorScreenPos, Plane nearPlane, Vector2 normalizedMousePosition, float gridSnap, ref Vector3 objectPosition, Vector3 objectRotation, ReadOnlySpan<char> name)
	{
		Vector2? posOrigin = GetPosition2d(framebufferSize, cursorScreenPos, nearPlane, objectPosition);
		if (!posOrigin.HasValue)
			return;

		bool wasMoveButtonActive = RenderMoveButton("Move", drawList, posOrigin.Value, ref _isMoveButtonActive);
		if (_isMoveButtonActive)
		{
			bool ctrl = GlfwInput.IsKeyDown(Keys.ControlLeft) || GlfwInput.IsKeyDown(Keys.ControlRight);
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
				Vector3 targetPosition = Camera3d.GetMouseWorldPosition(normalizedMousePosition, new(Vector3.UnitY, -objectPosition.Y));
				if (Vector3.Dot(targetPosition, nearPlane.Normal) + nearPlane.D < 0)
				{
					objectPosition.X = gridSnap > 0 ? MathF.Round(targetPosition.X / gridSnap) * gridSnap : targetPosition.X;
					objectPosition.Z = gridSnap > 0 ? MathF.Round(targetPosition.Z / gridSnap) * gridSnap : targetPosition.Z;
				}
			}
		}

		if (wasMoveButtonActive)
			LevelState.Track($"Moved {name}");

		Matrix4x4 rotationMatrix = MathUtils.CreateRotationMatrixFromEulerAngles(MathUtils.ToRadians(objectRotation));
		Vector2? posX = GetPosition2d(framebufferSize, cursorScreenPos, nearPlane, objectPosition + Vector3.Transform(Vector3.UnitX, rotationMatrix));
		Vector2? posY = GetPosition2d(framebufferSize, cursorScreenPos, nearPlane, objectPosition + Vector3.Transform(Vector3.UnitY, rotationMatrix));
		Vector2? posZ = GetPosition2d(framebufferSize, cursorScreenPos, nearPlane, objectPosition + Vector3.Transform(Vector3.UnitZ, rotationMatrix));

		if (posX.HasValue)
			drawList.AddLine(posOrigin.Value, posX.Value, 0xff0000ff);

		if (posY.HasValue)
			drawList.AddLine(posOrigin.Value, posY.Value, 0xff00ff00);

		if (posZ.HasValue)
			drawList.AddLine(posOrigin.Value, posZ.Value, 0xffff0000);
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
}
