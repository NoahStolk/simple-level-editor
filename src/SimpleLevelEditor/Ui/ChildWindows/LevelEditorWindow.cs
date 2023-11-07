using Detach;
using ImGuiNET;
using SimpleLevelEditor.Content;
using SimpleLevelEditor.Logic;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.State;

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

			Vector2 focusPointIconSize = new(16, 16);
			Vector2 focusPointIconPosition = cursorScreenPos + Camera3d.GetScreenPositionFrom3dPoint(Camera3d.FocusPointTarget, framebufferSize) - focusPointIconSize / 2;
			drawList.AddImage((IntPtr)InternalContent.Textures["FocusPoint"], focusPointIconPosition, focusPointIconPosition + focusPointIconSize);

			Vector2 cursorPosition = ImGui.GetCursorPos();

			ImGui.PushStyleColor(ImGuiCol.ChildBg, new Vector4(0, 0, 0, 0.2f));
			if (ImGui.BeginChild("Level Editor Menu", new(280, 144), true))
			{
				const int itemWidth = 160;

				ImGui.SeparatorText("Grid");

				ImGui.PushItemWidth(itemWidth);
				ImGui.SliderInt("Grid snap", ref _gridSnapIndex, 0, _gridSnapPoints.Length - 1, Inline.Span(GridSnap));
				ImGui.SliderInt("Cells per side", ref LevelEditorState.GridCellCount, 1, 64);
				ImGui.SliderInt("Cell size", ref LevelEditorState.GridCellSize, 1, 4);
				ImGui.PopItemWidth();

				ImGui.SeparatorText("Height");

				ImGui.PushItemWidth(itemWidth);
				ImGui.InputFloat("Height", ref LevelEditorState.TargetHeight, 0.25f, 1, "%.2f");
				ImGui.PopItemWidth();
			}

			ImGui.EndChild(); // End Level Editor Menu

			ImGui.PopStyleColor();

			Matrix4x4 viewProjection = Camera3d.ViewMatrix * Camera3d.Projection;
			Plane nearPlane = new(-viewProjection.M13, -viewProjection.M23, -viewProjection.M33, -viewProjection.M43);
			Vector2 mousePosition = Input.GetMousePosition() - cursorScreenPos;
			Vector2 normalizedMousePosition = new Vector2(mousePosition.X / framebufferSize.X - 0.5f, -(mousePosition.Y / framebufferSize.Y - 0.5f)) * 2;

			LevelEditorSelectionMenu.RenderSelectionMenu(framebufferSize, drawList, cursorScreenPos, nearPlane, normalizedMousePosition, GridSnap);

			ImGui.SetCursorPos(cursorPosition);
			ImGui.InvisibleButton("3d_view", framebufferSize);
			bool isFocused = ImGui.IsItemHovered();
			Camera3d.Update(ImGui.GetIO().DeltaTime, isFocused);

			MainLogic.Run(isFocused, normalizedMousePosition, nearPlane, GridSnap);
		}

		ImGui.EndChild(); // End Level Editor
	}
}
