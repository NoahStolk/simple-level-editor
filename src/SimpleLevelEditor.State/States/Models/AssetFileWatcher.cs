using SimpleLevelEditor.State.States.Level;
using SimpleLevelEditor.State.States.Messages;

namespace SimpleLevelEditor.State.States.Models;

public static class AssetFileWatcher
{
	private static readonly List<string> _directories = [];
	private static readonly List<FileSystemWatcher> _fileSystemWatchers = [];

	public static IReadOnlyList<string> Directories => _directories;

	public static void AddDirectory(string? directory)
	{
		try
		{
			if (directory == null || !Directory.Exists(directory))
			{
				MessagesState.AddError($"Asset directory {directory} does not exist");
				return;
			}

			// FileSystemWatcher is weird and will crash if we set the directory via the property. Must use the constructor.
			FileSystemWatcher fileSystemWatcher = new(directory, "*.*")
			{
				NotifyFilter = NotifyFilters.LastWrite,
				IncludeSubdirectories = true,
				EnableRaisingEvents = true,
			};
			fileSystemWatcher.Changed += (_, _) =>
			{
				MessagesState.AddInfo("Asset file changed");
				AssetLoadScheduleState.Schedule(LevelState.LevelFilePath);
			};

			_fileSystemWatchers.Add(fileSystemWatcher);
			_directories.Add(directory);
		}
		catch (Exception ex)
		{
			MessagesState.AddError($"Failed to watch directory '{directory}': {ex.Message}");
		}
	}

	public static void Destroy()
	{
		foreach (FileSystemWatcher fileSystemWatcher in _fileSystemWatchers)
			fileSystemWatcher.Dispose();

		_directories.Clear();
		_fileSystemWatchers.Clear();
	}
}
