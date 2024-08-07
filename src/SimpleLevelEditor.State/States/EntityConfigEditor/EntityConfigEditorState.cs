using Microsoft.FSharp.Core;
using SimpleLevelEditor.Formats;
using SimpleLevelEditor.Formats.Types.EntityConfig;
using SimpleLevelEditor.State.States.Messages;
using SimpleLevelEditor.State.Utils;

namespace SimpleLevelEditor.State.States.EntityConfigEditor;

public static class EntityConfigEditorState
{
	private const string _fileExtension = "json";

	private static EntityConfigData _entityConfig = EntityConfigData.CreateDefault();

	public static EntityConfigData EntityConfig
	{
		get => _entityConfig;
		private set
		{
			_entityConfig = value;

			// byte[] fileBytes = GetBytes(_level);
			// _memoryMd5Hash = MD5.HashData(fileBytes);
			// IsModified = !_fileMd5Hash.SequenceEqual(_memoryMd5Hash);
		}
	}

	public static string? EntityConfigFilePath { get; private set; }

	private static void SetEntityConfig(string? levelFilePath, EntityConfigData entityConfig)
	{
		EntityConfigFilePath = levelFilePath;
		EntityConfig = entityConfig;
	}

	public static void New()
	{
		EntityConfig = EntityConfigData.CreateDefault();
	}

	public static void Load()
	{
		DialogWrapper.FileOpen(LoadCallback, _fileExtension);
	}

	private static void LoadCallback(string? path)
	{
		if (path == null)
			return;

		using FileStream fs = new(path, FileMode.Open);
		FSharpOption<EntityConfigData>? entityConfigData = SimpleLevelEditorJsonSerializer.DeserializeEntityConfigFromStream(fs);
		if (entityConfigData == null)
		{
			MessagesState.AddError("Failed to load entity config.");
			return;
		}

		SetEntityConfig(path, entityConfigData.Value);
	}

	public static void Save()
	{
		// TODO
		// if (!IsModified)
		// 	action();
		// else
		// 	_savePromptAction(action);

		if (EntityConfigFilePath != null && File.Exists(EntityConfigFilePath))
			Save(EntityConfigFilePath);
		else
			SaveAs();
	}

	private static void Save(string path)
	{
		using MemoryStream ms = new();

		SimpleLevelEditorJsonSerializer.SerializeEntityConfigToStream(ms, EntityConfig);
		File.WriteAllBytes(path, ms.ToArray());
		SetEntityConfig(path, EntityConfig);
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
}
