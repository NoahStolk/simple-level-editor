using SimpleLevelEditor.Formats.Types.EntityConfig;
using SimpleLevelEditor.Formats.Types.Level;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleLevelEditor.Formats;

public static class SimpleLevelEditorJsonSerializer
{
	private static readonly JsonSerializerOptions _defaultSerializerOptions = JsonFSharpOptions.Default().ToJsonSerializerOptions();

	static SimpleLevelEditorJsonSerializer()
	{
		_defaultSerializerOptions.WriteIndented = true;
		_defaultSerializerOptions.IncludeFields = true;
	}

	public static EntityConfigData? DeserializeEntityConfig(string json)
	{
		return JsonSerializer.Deserialize<EntityConfigData>(json, _defaultSerializerOptions);
	}

	public static EntityConfigData? DeserializeEntityConfig(Stream stream)
	{
		return JsonSerializer.Deserialize<EntityConfigData>(stream, _defaultSerializerOptions);
	}

	public static string SerializeEntityConfig(EntityConfigData entityConfig)
	{
		return JsonSerializer.Serialize(entityConfig, _defaultSerializerOptions);
	}

	public static void SerializeEntityConfig(Stream stream, EntityConfigData entityConfig)
	{
		JsonSerializer.Serialize(stream, entityConfig, _defaultSerializerOptions);
	}

	public static Level3dData? DeserializeLevel(string json)
	{
		return JsonSerializer.Deserialize<Level3dData>(json, _defaultSerializerOptions);
	}

	public static Level3dData? DeserializeLevel(Stream stream)
	{
		return JsonSerializer.Deserialize<Level3dData>(stream, _defaultSerializerOptions);
	}

	public static string SerializeLevel(Level3dData level)
	{
		return JsonSerializer.Serialize(level, _defaultSerializerOptions);
	}

	public static void SerializeLevel(Stream stream, Level3dData level)
	{
		JsonSerializer.Serialize(stream, level, _defaultSerializerOptions);
	}
}
