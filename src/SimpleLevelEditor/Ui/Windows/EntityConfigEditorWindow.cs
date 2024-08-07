using Detach;
using ImGuiNET;
using Microsoft.FSharp.Collections;
using SimpleLevelEditor.State.States.EntityConfig;
using SimpleLevelEditor.State.States.EntityConfigEditor;
using SimpleLevelEditor.State.Utils;
using System.Diagnostics;

namespace SimpleLevelEditor.Ui.Windows;

public static class EntityConfigEditorWindow
{
	public static void Render()
	{
		if (ImGui.Begin("Entity Config Editor", ImGuiWindowFlags.MenuBar))
		{
			RenderMenuBar();
			RenderEntityConfig();
		}

		ImGui.End();
	}

	private static void RenderMenuBar()
	{
		if (ImGui.BeginMenuBar())
		{
			if (ImGui.BeginMenu("File"))
			{
				if (ImGui.MenuItem("New"))
					EntityConfigEditorState.New();

				if (ImGui.MenuItem("Open"))
					EntityConfigEditorState.Load();

				if (ImGui.MenuItem("Save"))
					EntityConfigEditorState.Save();

				if (ImGui.MenuItem("Save as..."))
					EntityConfigEditorState.SaveAs();

				ImGui.EndMenu();
			}

			ImGui.EndMenuBar();
		}
	}

	private static void RenderEntityConfig()
	{
		RenderPaths(
			"Models",
			"Add models",
			EntityConfigEditorState.EntityConfig.ModelPaths,
			paths => AddCallback(EntityConfigEditorState.EntityConfig.AddModel, paths),
			EntityConfigEditorState.EntityConfig.RemoveModel,
			"obj");

		RenderPaths(
			"Textures",
			"Add textures",
			EntityConfigEditorState.EntityConfig.TexturePaths,
			paths => AddCallback(EntityConfigEditorState.EntityConfig.AddTexture, paths),
			EntityConfigEditorState.EntityConfig.RemoveTexture,
			"bmp,gif,jpeg,pbm,png,tiff,tga,webp");
	}

	private static void RenderPaths(
		ReadOnlySpan<char> title,
		ReadOnlySpan<char> buttonLabel,
		FSharpList<string> paths,
		Action<IReadOnlyList<string>?> addCallback,
		Action<string> removeCallback,
		string fileExtensions)
	{
		ImGui.SeparatorText(title);

		if (ImGui.Button(buttonLabel))
		{
			DialogWrapper.FileOpenMultiple(addCallback, fileExtensions);
		}

		if (ImGui.BeginChild(Inline.Span($"{title}List"), new Vector2(0, 160), ImGuiChildFlags.Border))
		{
			string? toRemove = null;
			foreach (string path in paths)
			{
				ImGui.PushID(Inline.Span($"button_delete_{path}"));
				if (ImGui.Button("X"))
					toRemove = path;

				ImGui.PopID();

				ImGui.SameLine();
				ImGui.Text(path);
			}

			if (toRemove != null)
			{
				removeCallback(toRemove);
			}
		}

		ImGui.EndChild();
	}

	private static void AddCallback(Action<string> callback, IReadOnlyList<string>? paths)
	{
		if (paths == null)
			return;

		string? parentDirectory = Path.GetDirectoryName(EntityConfigEditorState.EntityConfigFilePath);
		Debug.Assert(parentDirectory != null, "Parent directory should not be null.");

		string[] relativePaths = paths.Select(path => Path.GetRelativePath(parentDirectory, path)).ToArray();

		foreach (string relativePath in relativePaths)
			callback(relativePath);
	}
}
