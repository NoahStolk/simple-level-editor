using ImGuiNET;
using NativeFileDialogSharp;
using SimpleLevelEditor.Formats.Level3d;
using SimpleLevelEditor.State;
using System.Diagnostics;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class LevelInfoWindow
{
	public static void Render(Vector2 size)
	{
		if (ImGui.BeginChild("Level Info", size, true))
		{
			ImGui.SeparatorText("Level Info");

			ImGui.TextWrapped(LevelState.LevelFilePath ?? "None");
			ImGui.Text(Inline.Span($"Version: {LevelState.Level.Version}"));
			ImGui.Text(Inline.Span($"Meshes: {LevelState.Level.Meshes.Count}"));
			ImGui.Text(Inline.Span($"Textures: {LevelState.Level.Textures.Count}"));
			ImGui.Text(Inline.Span($"WorldObjects: {LevelState.Level.WorldObjects.Count}"));
			ImGui.Text(Inline.Span($"Entities: {LevelState.Level.Entities.Count}"));

			ImGui.BeginDisabled(LevelState.LevelFilePath == null);

			ImGui.SeparatorText("Add New");

			RenderAssetPaths("Meshes", "obj", l => l.Meshes, (l, d) => l.Meshes = d);
			RenderAssetPaths("Textures", "tga", l => l.Textures, (l, d) => l.Textures = d);

			ImGui.EndDisabled();
		}

		ImGui.EndChild(); // End LevelInfo
	}

	private static void RenderAssetPaths(string name, string dialogFilterList, Func<Level3dData, List<string>> selector, Action<Level3dData, List<string>> setter)
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

		if (ImGui.BeginChild(Inline.Span($"{name}List"), new(0, 256), true))
		{
			string? toRemove = null;
			foreach (string mesh in list)
			{
				ImGui.PushID(Inline.Span($"button_delete_{name}_{mesh}"));
				if (ImGui.Button("X"))
					toRemove = mesh;

				ImGui.PopID();

				ImGui.SameLine();
				ImGui.Text(mesh);
			}

			if (toRemove != null)
				list.Remove(toRemove);
		}

		ImGui.EndChild();
	}
}
