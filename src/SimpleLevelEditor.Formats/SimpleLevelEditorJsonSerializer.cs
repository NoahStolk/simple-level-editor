using SimpleLevelEditor.Formats.EntityConfig;
using SimpleLevelEditor.Formats.Level;
using System.Text.Json;

namespace SimpleLevelEditor.Formats;

public static class SimpleLevelEditorJsonSerializer
{
	private static readonly JsonSerializerOptions _defaultSerializerOptions = new() { WriteIndented = true, IncludeFields = true };

	public static EntityConfigData? DeserializeEntityConfigFromString(string json)
	{
		return DeserializeFromString<EntityConfigData>(json);
	}

	public static EntityConfigData? DeserializeEntityConfigFromStream(Stream stream)
	{
		return DeserializeFromStream<EntityConfigData>(stream);
	}

	public static string SerializeEntityConfigToString(EntityConfigData entityConfigData)
	{
		return SerializeToString(entityConfigData);
	}

	public static void SerializeEntityConfigToStream(Stream stream, EntityConfigData entityConfigData)
	{
		SerializeToStream(stream, entityConfigData);
	}

	public static Level3dData? DeserializeLevelFromString(string json)
	{
		return DeserializeFromString<Level3dData>(json);
	}

	public static Level3dData? DeserializeLevelFromStream(Stream stream)
	{
		return DeserializeFromStream<Level3dData>(stream);
	}

	public static string SerializeLevelToString(Level3dData level3dData)
	{
		return SerializeToString(level3dData);
	}

	public static void SerializeLevelToStream(Stream stream, Level3dData level3dData)
	{
		SerializeToStream(stream, level3dData);
	}

	private static T? DeserializeFromString<T>(string json)
	{
		try
		{
			return JsonSerializer.Deserialize<T>(json, _defaultSerializerOptions);
		}
		catch (Exception)
		{
			return default;
		}
	}

	private static T? DeserializeFromStream<T>(Stream stream)
	{
		try
		{
			return JsonSerializer.Deserialize<T>(stream, _defaultSerializerOptions);
		}
		catch (Exception)
		{
			return default;
		}
	}

	private static string SerializeToString<T>(T obj)
	{
		return JsonSerializer.Serialize(obj, _defaultSerializerOptions);
	}

	private static void SerializeToStream<T>(Stream stream, T obj)
	{
		JsonSerializer.Serialize(stream, obj, _defaultSerializerOptions);
	}
}
