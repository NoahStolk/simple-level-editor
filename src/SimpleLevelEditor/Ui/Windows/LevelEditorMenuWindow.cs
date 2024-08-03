using Detach;
using ImGuiNET;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.State.States.LevelEditor;

namespace SimpleLevelEditor.Ui.Windows;

public static class LevelEditorMenuWindow
{
	public static void Render(Vector2 windowPosition)
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
					ImGui.SliderInt("Snap", ref LevelEditorState.SnapIndex, 0, LevelEditorState.SnapPoints.Count - 1, Inline.Span(LevelEditorState.Snap));
					ImGui.InputFloat("Target height", ref LevelEditorState.TargetHeight, 0.25f, 1, "%.2f");

					ImGui.EndTabItem();
				}

				if (ImGui.BeginTabItem("Display"))
				{
					ImGui.SliderFloat("Cell fade out min distance", ref LevelEditorState.GridCellFadeOutMinDistance, 16, 128);
					ImGui.SliderFloat("Cell fade out max distance", ref LevelEditorState.GridCellFadeOutMaxDistance, 32, 256);
					ImGui.SliderInt("Cell interval", ref LevelEditorState.GridCellInterval, 2, 16);
					ImGui.SliderFloat("Camera zoom", ref Camera3d.Zoom, 1, 100, "%.2f");

					bool shouldRenderWorldObjects = LevelEditorState.ShouldRenderWorldObjects;
					if (ImGui.Checkbox("WorldObjects", ref shouldRenderWorldObjects))
					{
						LevelEditorState.ShouldRenderWorldObjects = shouldRenderWorldObjects;
						LevelEditorState.ClearSelectedWorldObject();
					}

					foreach (KeyValuePair<string, bool> filter in LevelEditorState.EntityRenderFilter)
					{
						bool value = filter.Value;
						if (ImGui.Checkbox(filter.Key, ref value))
						{
							LevelEditorState.EntityRenderFilter[filter.Key] = value;

							if (filter.Key == LevelEditorState.SelectedEntity?.Name)
								LevelEditorState.ClearSelectedEntity();
						}
					}

					ImGui.EndTabItem();
				}

				ImGui.PopItemWidth();

				ImGui.EndTabBar();
			}
		}

		ImGui.End();

		ImGui.PopStyleColor();
	}
}
