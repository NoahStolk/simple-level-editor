using SimpleLevelEditor.Formats.Types.EntityConfig;
using SimpleLevelEditor.Formats.Types.Level;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleLevelEditor.Formats;

[RequiresUnreferencedCode("This class is used for serialization and deserialization of types that are not known at compile time.")]
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
		try
		{
			return JsonSerializer.Deserialize<EntityConfigData>(json, _defaultSerializerOptions);
		}
		catch (JsonException)
		{
			return null;
		}
	}

	public static EntityConfigData? DeserializeEntityConfig(Stream stream)
	{
		try
		{
			return JsonSerializer.Deserialize<EntityConfigData>(stream, _defaultSerializerOptions);
		}
		catch (JsonException)
		{
			return null;
		}
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
		try
		{
			return JsonSerializer.Deserialize<Level3dData>(json, _defaultSerializerOptions);
		}
		catch (JsonException)
		{
			return null;
		}
	}

	public static Level3dData? DeserializeLevel(Stream stream)
	{
		try
		{
			return JsonSerializer.Deserialize<Level3dData>(stream, _defaultSerializerOptions);
		}
		catch (JsonException)
		{
			return null;
		}
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
