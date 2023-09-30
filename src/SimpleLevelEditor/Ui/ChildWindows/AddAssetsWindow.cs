using ImGuiNET;
using NativeFileDialogSharp;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.State;
using System.Diagnostics;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class AddAssetsWindow
{
	public static void Render(Vector2 size)
	{
		if (ImGui.BeginChild("Add New", size, true))
		{
			ImGui.BeginDisabled(LevelState.LevelFilePath == null);

			ImGui.SeparatorText("Add New");

			float height = MathF.Floor(size.Y / 2f - 40f) - 1;
			RenderAssetPaths(height, "Meshes", "obj", l => l.Meshes, (l, d) => l.Meshes = d);
			RenderAssetPaths(height, "Textures", "tga", l => l.Textures, (l, d) => l.Textures = d);

			ImGui.EndDisabled();
		}

		ImGui.EndChild(); // End Add New
	}

	private static void RenderAssetPaths(float windowHeight, string name, string dialogFilterList, Func<Level3dData, List<string>> selector, Action<Level3dData, List<string>> setter)
	{
		List<string> list = selector(LevelState.Level);

		if (ImGui.Button(Inline.Span($"Add {name}")))
		{
			Debug.Assert(LevelState.LevelFilePath != null, "Cannot click this button because it should be disabled.");

			DialogResult dialogResult = Dialog.FileOpenMultiple(dialogFilterList);
			if (dialogResult is { IsOk: true })
			{
				string? parentDirectory = Path.GetDirectoryName(LevelState.LevelFilePath);
				Debug.Assert(parentDirectory != null, "Parent directory should not be null.");

				string[] relativePaths = dialogResult.Paths.Select(path => Path.GetRelativePath(parentDirectory, path)).ToArray();

				list.AddRange(relativePaths);
				setter(LevelState.Level, list.Order().Distinct().ToList());
			}
		}

		if (ImGui.BeginChild(Inline.Span($"{name}List"), new(0, windowHeight), true))
		{
			string? toRemove = null;
			foreach (string item in list)
			{
				ImGui.PushID(Inline.Span($"button_delete_{name}_{item}"));
				if (ImGui.Button("X"))
					toRemove = item;

				ImGui.PopID();

				ImGui.SameLine();
				ImGui.Text(item);
			}

			if (toRemove != null)
				list.Remove(toRemove);
		}

		ImGui.EndChild();
	}
}
