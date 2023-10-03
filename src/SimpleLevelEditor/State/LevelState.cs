using NativeFileDialogSharp;
using SimpleLevelEditor.Formats;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Rendering;
using System.Text;
using System.Xml;

namespace SimpleLevelEditor.State;

public static class LevelState
{
	private const string _fileExtension = "xml";
	private static readonly XmlWriterSettings _xmlWriterSettings = new() { Indent = true, Encoding = new UTF8Encoding(false) };

	public static string? LevelFilePath { get; private set; }
	public static Level3dData Level { get; private set; } = Level3dData.Default;

	public static void New()
	{
		Level3dData level = Level3dData.Default.DeepCopy();
		SetLevel(null, level);
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
			Save(dialogResult.Path);
	}

	private static void Save(string path)
	{
		using MemoryStream ms = new();
		using XmlWriter writer = XmlWriter.Create(ms, _xmlWriterSettings);
		XmlFormatSerializer.WriteLevel(Level, writer);
		writer.Flush();

		ms.Write("\n"u8);

		File.WriteAllBytes(path, ms.ToArray());
		SetLevel(path, Level);
	}

	private static void SetLevel(string? levelFilePath, Level3dData level)
	{
		LevelFilePath = levelFilePath;
		Level = level;

		ObjectEditorState.SelectedWorldObject = null;
		ObjectCreatorState.Reset();

		MeshContainer.Rebuild(levelFilePath);
		TextureContainer.Rebuild(levelFilePath);
	}
}
