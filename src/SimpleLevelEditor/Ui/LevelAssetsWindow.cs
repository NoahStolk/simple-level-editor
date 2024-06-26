using Detach;
using ImGuiNET;
using SimpleLevelEditor.State;
using SimpleLevelEditor.State.Level;
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
					string absolutePath = Path.Combine(parentDirectory, LevelState.Level.EntityConfigPath.Value);
					EntityConfigState.LoadEntityConfig(absolutePath);
				}
			}

			ImGui.SeparatorText("Entity Config");
			ImGui.Text(LevelState.Level.EntityConfigPath == null ? "<No entity config loaded>" : LevelState.Level.EntityConfigPath.Value);

			ImGui.BeginDisabled(LevelState.LevelFilePath == null);
			if (ImGui.Button("Load entity config"))
			{
				DialogWrapper.FileOpen(LoadEntityConfigCallback, "json");

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

			RenderModelPaths();
		}

		ImGui.End();
	}

	private static void RenderModelPaths()
	{
		ImGui.SeparatorText("Models");

		ImGui.BeginDisabled(LevelState.LevelFilePath == null);
		if (ImGui.Button("Add models"))
		{
			Debug.Assert(LevelState.LevelFilePath != null, "Cannot click this button because it should be disabled.");

			DialogWrapper.FileOpenMultiple(AddAssetsCallback, "obj");
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
		if (ImGui.BeginChild("ModelsList", Vector2.Zero, ImGuiChildFlags.Border))
		{
			string? toRemove = null;
			foreach (string modelPath in LevelState.Level.ModelPaths)
			{
				ImGui.PushID(Inline.Span($"button_delete_{modelPath}"));
				if (ImGui.Button("X"))
					toRemove = modelPath;

				ImGui.PopID();

				ImGui.SameLine();
				ImGui.Text(modelPath);
			}

			if (toRemove != null)
			{
				LevelState.Level.RemoveModel(toRemove);
				AssetLoadScheduleState.Schedule(LevelState.LevelFilePath);
				LevelState.Track($"Removed model '{toRemove}'");
			}
		}

		ImGui.EndChild();
		ImGui.EndDisabled();
	}

	private static void AddAssetsCallback(IReadOnlyList<string>? paths)
	{
		if (paths == null)
			return;

		string? parentDirectory = Path.GetDirectoryName(LevelState.LevelFilePath);
		Debug.Assert(parentDirectory != null, "Parent directory should not be null.");

		string[] relativePaths = paths.Select(path => Path.GetRelativePath(parentDirectory, path)).ToArray();

		foreach (string relativePath in relativePaths)
			LevelState.Level.AddModel(relativePath);

		AssetLoadScheduleState.Schedule(LevelState.LevelFilePath);

		LevelState.Track("Added assets");
	}
}
