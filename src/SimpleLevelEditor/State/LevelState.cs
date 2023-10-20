using NativeFileDialogSharp;
using SimpleLevelEditor.Formats;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Rendering;
using System.Xml;

namespace SimpleLevelEditor.State;

public static class LevelState
{
	private const string _fileExtension = "xml";

	public static string? LevelFilePath { get; private set; }
	public static Level3dData Level { get; private set; } = Level3dData.Default;

	public static void New()
	{
		Level3dData level = Level3dData.Default.DeepCopy();
		SetLevel(null, level);
		ClearState();
		ReloadAssets(null);
	}

	public static void Load()
	{
		DialogResult dialogResult = DialogWrapper.FileOpen(_fileExtension);
		if (dialogResult is not { IsOk: true })
			return;

		using FileStream fs = new(dialogResult.Path, FileMode.Open);
		using XmlReader reader = XmlReader.Create(fs);
		Level3dData level = XmlFormatSerializer.ReadLevel(reader);

		SetLevel(dialogResult.Path, level);
		ClearState();
		ReloadAssets(dialogResult.Path);
	}

	public static void Save()
	{
		if (LevelFilePath != null && File.Exists(LevelFilePath))
			Save(LevelFilePath);
		else
			SaveAs();
	}

	public static void SaveAs()
	{
		DialogResult dialogResult = DialogWrapper.FileSave(_fileExtension);
		if (dialogResult is { IsOk: true })
		{
			string path = Path.ChangeExtension(dialogResult.Path, ".xml");
			Save(path);
		}
	}

	private static void Save(string path)
	{
		using MemoryStream ms = new();
		XmlFormatSerializer.WriteLevel(ms, Level, false);
		File.WriteAllBytes(path, ms.ToArray());
		SetLevel(path, Level);
	}

	private static void SetLevel(string? levelFilePath, Level3dData level)
	{
		LevelFilePath = levelFilePath;
		Level = level;
	}

	private static void ClearState()
	{
		ObjectEditorState.SelectedWorldObject = null;
		ObjectCreatorState.Reset();
	}

	public static void ReloadAssets(string? levelFilePath)
	{
		MeshContainer.Rebuild(levelFilePath);
		TextureContainer.Rebuild(levelFilePath);
	}
}
