using Detach;
using ImGuiNET;
using SimpleLevelEditor.State;
using System.Diagnostics;

namespace SimpleLevelEditor.Ui;

public static class LevelAssetsWindow
{
	public static void Render()
	{
		if (ImGui.Begin("Level Assets"))
		{
			if (ImGui.Button("Reload all"))
			{
				LevelState.ReloadAssets(LevelState.LevelFilePath);
				if (LevelState.Level.EntityConfigPath != null)
				{
					string? parentDirectory = Path.GetDirectoryName(LevelState.LevelFilePath);
					Debug.Assert(parentDirectory != null, "Parent directory should not be null.");
					string absolutePath = Path.Combine(parentDirectory, LevelState.Level.EntityConfigPath);
					EntityConfigState.LoadEntityConfig(absolutePath);
				}
			}

			ImGui.SeparatorText("Entity Config");
			ImGui.Text(LevelState.Level.EntityConfigPath ?? "<No entity config loaded>");

			ImGui.BeginDisabled(LevelState.LevelFilePath == null);
			if (ImGui.Button("Load entity config"))
			{
				DialogWrapper.FileOpen(LoadEntityConfigCallback, "xml");

				static void LoadEntityConfigCallback(string? path)
				{
					if (path == null)
						return;

					string? parentDirectory = Path.GetDirectoryName(LevelState.LevelFilePath);
					Debug.Assert(parentDirectory != null, "Parent directory should not be null.");
					LevelState.Level.EntityConfigPath = Path.GetRelativePath(parentDirectory, path);
					EntityConfigState.LoadEntityConfig(path);
				}
			}

			ImGui.EndDisabled();

			float height = ImGui.GetContentRegionAvail().Y / 2f - 48;

			RenderAssetPaths(height, "Meshes", "obj", LevelState.Level.Meshes, l => LevelState.Level.Meshes = l);
			RenderAssetPaths(height, "Textures", "tga", LevelState.Level.Textures, l => LevelState.Level.Textures = l);
		}

		ImGui.End();
	}

	private static void RenderAssetPaths(float windowHeight, ReadOnlySpan<char> name, string dialogFilterList, List<string> list, Action<List<string>> setList)
	{
		ImGui.SeparatorText(name);

		ImGui.BeginDisabled(LevelState.LevelFilePath == null);
		if (ImGui.Button(Inline.Span($"Add {name}")))
		{
			Debug.Assert(LevelState.LevelFilePath != null, "Cannot click this button because it should be disabled.");

			DialogWrapper.FileOpenMultiple(p => AddAssetsCallback(list, setList, p), dialogFilterList);
		}

		ImGui.EndDisabled();

		if (LevelState.LevelFilePath == null)
		{
			ImGui.SameLine();
			ImGui.Text("(?)");
			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("You must save the level before you can add assets.");
		}

		ImGui.BeginDisabled(LevelState.LevelFilePath == null);
		if (ImGui.BeginChild(Inline.Span($"{name}List"), new Vector2(0, windowHeight), ImGuiChildFlags.Border))
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
			{
				list.Remove(toRemove);
				LevelState.ReloadAssets(LevelState.LevelFilePath);

				LevelState.Track("Removed assets");
			}
		}

		ImGui.EndChild();
		ImGui.EndDisabled();
	}

	private static void AddAssetsCallback(List<string> list, Action<List<string>> setList, IReadOnlyList<string>? paths)
	{
		if (paths == null)
			return;

		string? parentDirectory = Path.GetDirectoryName(LevelState.LevelFilePath);
		Debug.Assert(parentDirectory != null, "Parent directory should not be null.");

		string[] relativePaths = paths.Select(path => Path.GetRelativePath(parentDirectory, path)).ToArray();

		list.AddRange(relativePaths);
		setList(list.Order().Distinct().ToList());
		AssetLoadScheduleState.Schedule(LevelState.LevelFilePath);

		LevelState.Track("Added assets");
	}
}
