using Detach;
using ImGuiNET;
using SimpleLevelEditor.Content;
using SimpleLevelEditor.Logic;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Ui.ChildWindows;

namespace SimpleLevelEditor.Ui;

public static class LevelEditorWindow
{
	private static readonly float[] _snapPoints = [0, 0.125f, 0.25f, 0.5f, 1, 2, 4, 8];
	private static int _snapIndex = 4;

	private static float Snap => _snapIndex >= 0 && _snapIndex < _snapPoints.Length ? _snapPoints[_snapIndex] : 0;

	public static void Render()
	{
		if (ImGui.Begin("Level Editor"))
		{
			Vector2 framebufferSize = ImGui.GetContentRegionAvail();

			SceneFramebuffer.Initialize(framebufferSize);
			Camera3d.AspectRatio = framebufferSize.X / framebufferSize.Y;

			SceneFramebuffer.RenderFramebuffer(framebufferSize);

			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			drawList.AddImage((IntPtr)SceneFramebuffer.FramebufferTextureId, cursorScreenPos, cursorScreenPos + framebufferSize, Vector2.UnitY, Vector2.UnitX);

			Vector2 focusPointIconSize = new(16, 16);
			Vector2 focusPointIconPosition = cursorScreenPos + Camera3d.GetScreenPositionFrom3dPoint(Camera3d.FocusPointTarget, framebufferSize) - focusPointIconSize / 2;
			drawList.AddImage((IntPtr)InternalContent.Textures["FocusPoint"], focusPointIconPosition, focusPointIconPosition + focusPointIconSize);

			RenderMoveDistances(drawList, cursorScreenPos, framebufferSize);

			Vector2 cursorPosition = ImGui.GetCursorPos();

			RenderLevelEditorMenus(ImGui.GetCursorScreenPos());

			Matrix4x4 viewProjection = Camera3d.ViewMatrix * Camera3d.Projection;
			Plane nearPlane = new(-viewProjection.M13, -viewProjection.M23, -viewProjection.M33, -viewProjection.M43);
			Vector2 mousePosition = Input.GlfwInput.CursorPosition - cursorScreenPos;
			Vector2 normalizedMousePosition = new Vector2(mousePosition.X / framebufferSize.X - 0.5f, -(mousePosition.Y / framebufferSize.Y - 0.5f)) * 2;

			LevelEditorSelectionMenu.RenderSelectionMenu(framebufferSize, drawList, cursorScreenPos, nearPlane, normalizedMousePosition, Snap);

			ImGui.SetCursorPos(cursorPosition);
			ImGui.InvisibleButton("3d_view", framebufferSize);
			bool isFocused = ImGui.IsItemHovered();
			Camera3d.Update(ImGui.GetIO().DeltaTime, isFocused);

			MainLogic.Run(isFocused, normalizedMousePosition, nearPlane, Snap);
		}

		ImGui.End();
	}

	private static void RenderMoveDistances(ImDrawListPtr drawList, Vector2 cursorScreenPos, Vector2 framebufferSize)
	{
		if (!LevelEditorState.MoveTargetPosition.HasValue)
			return;

		Vector3? selectedObjectPosition = LevelEditorState.SelectedWorldObject?.Position ?? LevelEditorState.SelectedEntity?.Position;
		if (selectedObjectPosition == null)
			return;

		Vector3 moveTarget = LevelEditorState.MoveTargetPosition.Value;

		uint colorAsInt = ImGui.GetColorU32(new Vector4(1, 1, 0, 1));

		float halfDistanceX = MathF.Abs(selectedObjectPosition.Value.X - moveTarget.X) / 2;
		float halfDistanceY = MathF.Abs(selectedObjectPosition.Value.Y - moveTarget.Y) / 2;
		float halfDistanceZ = MathF.Abs(selectedObjectPosition.Value.Z - moveTarget.Z) / 2;

		Vector3 averageOnX = selectedObjectPosition.Value with { X = (selectedObjectPosition.Value.X + moveTarget.X) / 2 };
		Vector3 averageOnY = selectedObjectPosition.Value with { Y = (selectedObjectPosition.Value.Y + moveTarget.Y) / 2 };
		Vector3 averageOnZ = selectedObjectPosition.Value with { Z = (selectedObjectPosition.Value.Z + moveTarget.Z) / 2 };

		if (halfDistanceX > 0)
			AddText($"{halfDistanceX:F2}", averageOnX with { Z = moveTarget.Z });

		if (halfDistanceY > 0)
			AddText($"{halfDistanceY:F2}", averageOnY);

		if (halfDistanceZ > 0)
			AddText($"{halfDistanceZ:F2}", averageOnZ);

		void AddText(ReadOnlySpan<char> text, Vector3 position)
		{
			Vector2 textSize = ImGui.CalcTextSize(text);
			Vector2 padding = new(4, 2);
			Vector2 origin = cursorScreenPos + Camera3d.GetScreenPositionFrom3dPoint(position, framebufferSize);
			drawList.AddRectFilled(origin - padding, origin + textSize + padding, 0xff000000);
			drawList.AddText(origin, colorAsInt, text);
		}
	}

	private static void RenderLevelEditorMenus(Vector2 windowPosition)
	{
		ImGui.PushStyleColor(ImGuiCol.WindowBg, new Vector4(0, 0, 0, 0.4f));
		ImGui.SetNextWindowPos(windowPosition, ImGuiCond.Always);
		ImGui.SetNextWindowSizeConstraints(new Vector2(280, 160), new Vector2(360, 1024));
		if (ImGui.Begin("Level Editor Menu", ImGuiWindowFlags.NoMove))
		{
			const int itemWidth = 160;
			if (ImGui.BeginTabBar("LevelEditorMenus"))
			{
				ImGui.PushItemWidth(itemWidth);

				if (ImGui.BeginTabItem("Editing"))
				{
					ImGui.SliderInt("Snap", ref _snapIndex, 0, _snapPoints.Length - 1, Inline.Span(Snap));
					ImGui.InputFloat("Target height", ref LevelEditorState.TargetHeight, 0.25f, 1, "%.2f");
					ImGui.SliderFloat("Camera zoom", ref Camera3d.Zoom, 1, 100, "%.2f");

					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Display"))
				{
					ImGui.SliderInt("Cells per side", ref LevelEditorState.GridCellCount, 1, 64);
					ImGui.SliderInt("Cell size", ref LevelEditorState.GridCellSize, 1, 4);

					foreach (KeyValuePair<string, bool> filter in LevelEditorState.RenderFilter)
					{
						bool value = filter.Value;
						if (ImGui.Checkbox(filter.Key, ref value))
						{
							LevelEditorState.RenderFilter[filter.Key] = value;

							if (filter.Key == "WorldObjects")
								LevelEditorState.ClearSelectedWorldObject();
							else if (filter.Key == $"Entities:{LevelEditorState.SelectedEntity?.Name}")
								LevelEditorState.ClearSelectedEntity();
						}
					}

					ImGui.EndTabItem();
				}

				ImGui.PopItemWidth();

				ImGui.EndTabBar();
			}
		}

		ImGui.End(); // End Level Editor Menu

		ImGui.PopStyleColor();
	}
}
