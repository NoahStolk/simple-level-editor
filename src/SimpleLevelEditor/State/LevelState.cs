using SimpleLevelEditor.Formats;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.Ui.ChildWindows;
using System.Security.Cryptography;
using System.Xml;

namespace SimpleLevelEditor.State;

public static class LevelState
{
	private const int _maxHistoryEntries = 100;
	private const string _fileExtension = "xml";

	private static byte[] _memoryMd5Hash;
	private static byte[] _fileMd5Hash;

	private static Level3dData _level = Level3dData.Default;

	static LevelState()
	{
		byte[] fileBytes = GetBytes(_level);
		_memoryMd5Hash = MD5.HashData(fileBytes);
		_fileMd5Hash = MD5.HashData(fileBytes);

		History = new List<HistoryEntry> { new(_level, MD5.HashData(fileBytes), "Reset") };
	}

	public static int CurrentHistoryIndex { get; private set; }
	public static string? LevelFilePath { get; private set; }
	public static bool IsModified { get; private set; }
	public static Level3dData Level
	{
		get => _level;
		private set
		{
			_level = value;

			byte[] fileBytes = GetBytes(_level);
			_memoryMd5Hash = MD5.HashData(fileBytes);
			IsModified = !_fileMd5Hash.SequenceEqual(_memoryMd5Hash);
		}
	}

	// Note; the history should never be empty.
	public static IReadOnlyList<HistoryEntry> History { get; private set; }

	private static byte[] GetBytes(Level3dData obj)
	{
		using MemoryStream ms = new();
		XmlFormatSerializer.WriteLevel(ms, obj, false);
		return ms.ToArray();
	}

	public static void New()
	{
		Level3dData level = Level3dData.Default.DeepCopy();
		SetLevel(null, level);
		ClearState();
		ReloadAssets(null);
		Track("Reset");
	}

	public static void Load()
	{
		DialogWrapper.FileOpen(LoadCallback, _fileExtension);
	}

	private static void LoadCallback(string? path)
	{
		if (path == null)
			return;

		using (FileStream fs = new(path, FileMode.Open))
		using (XmlReader reader = XmlReader.Create(fs))
		{
			Level3dData level = XmlFormatSerializer.ReadLevel(reader);
			SetLevel(path, level);
		}

		ClearState();
		LoadScheduleState.Schedule(path);
		Track("Reset");
	}

	public static void Save()
	{
		// TODO
		// if (!IsModified)
		// 	action();
		// else
		// 	_savePromptAction(action);

		if (LevelFilePath != null && File.Exists(LevelFilePath))
			Save(LevelFilePath);
		else
			SaveAs();
	}

	public static void SaveAs()
	{
		DialogWrapper.FileSave(SaveAsCallback, _fileExtension);
	}

	private static void SaveAsCallback(string? path)
	{
		if (path == null)
			return;

		Save(Path.ChangeExtension(path, $".{_fileExtension}"));
	}

	public static void SetHistoryIndex(int index)
	{
		if (index < 0 || index >= History.Count)
			return;

		CurrentHistoryIndex = Math.Clamp(index, 0, History.Count - 1);
		Level = History[CurrentHistoryIndex].Object.DeepCopy();

		LevelEditorState.HighlightedObject = null;
		LevelEditorState.UpdateSelectedWorldObject();
	}

	public static void Track(string editDescription)
	{
		Level3dData copy = Level.DeepCopy();
		byte[] hash = MD5.HashData(GetBytes(copy));

		if (editDescription == "Reset")
		{
			UpdateHistory(new List<HistoryEntry> { new(copy, hash, "Reset") }, 0);
		}
		else
		{
			byte[] originalHash = History[CurrentHistoryIndex].Hash;

			if (originalHash.SequenceEqual(hash))
				return;

			HistoryEntry historyEntry = new(copy, hash, editDescription);

			// Clear any newer history.
			List<HistoryEntry> newHistory = History.ToList();
			newHistory = newHistory.Take(CurrentHistoryIndex + 1).Append(historyEntry).ToList();

			// Remove history if there are too many entries.
			int newCurrentIndex = CurrentHistoryIndex + 1;
			if (newHistory.Count > _maxHistoryEntries)
			{
				newHistory.RemoveAt(0);
				newCurrentIndex--;
			}

			UpdateHistory(newHistory, newCurrentIndex);
		}

		void UpdateHistory(IReadOnlyList<HistoryEntry> history, int currentHistoryIndex)
		{
			History = history;
			CurrentHistoryIndex = currentHistoryIndex;
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
		AssetFileWatcher.Destroy();

		LevelFilePath = levelFilePath;
		Level = level;
		_fileMd5Hash = _memoryMd5Hash;
		IsModified = !_fileMd5Hash.SequenceEqual(_memoryMd5Hash);

		string? baseDirectory = Path.GetDirectoryName(levelFilePath);
		if (baseDirectory != null)
			RefreshAssetFileWatcher(baseDirectory, level.Meshes.Concat(level.Textures));
	}

	private static void RefreshAssetFileWatcher(string baseDirectory, IEnumerable<string> assetPaths)
	{
		List<string> assetDirectories = new();
		foreach (string assetFilePath in assetPaths)
		{
			string? directory = Path.GetDirectoryName(assetFilePath);
			if (directory != null && !assetDirectories.Contains(directory))
				assetDirectories.Add(directory);
		}

		foreach (string assetDirectory in assetDirectories)
			AssetFileWatcher.AddDirectory(Path.Combine(baseDirectory, assetDirectory));
	}

	private static void ClearState()
	{
		LevelEditorState.SetSelectedWorldObject(null);
		WorldObjectEditorWindow.Reset();
	}

	public static bool ReloadAssets(string? levelFilePath)
	{
		try
		{
			MeshContainer.Rebuild(levelFilePath);
			TextureContainer.Rebuild(levelFilePath);
			return true;
		}
		catch (Exception ex)
		{
			DebugState.AddWarning($"Failed to reload assets: {ex.Message}");
			return false;
		}
	}

	public record HistoryEntry(Level3dData Object, byte[] Hash, string EditDescription);
}
