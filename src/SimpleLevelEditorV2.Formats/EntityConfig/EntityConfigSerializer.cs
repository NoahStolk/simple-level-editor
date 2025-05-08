using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleLevelEditorV2.Formats.EntityConfig;

public static class EntityConfigSerializer
{
	private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
	{
		WriteIndented = true,
		IncludeFields = true,
		Converters =
		{
			new JsonStringEnumConverter(),
		},
	};

	public static string Serialize(Model.EntityConfigModel config)
	{
		return JsonSerializer.Serialize(config, _jsonSerializerOptions);
	}

	public static Model.EntityConfigModel Deserialize(string json)
	{
		Model.EntityConfigModel? entityConfig = JsonSerializer.Deserialize<Model.EntityConfigModel>(json, _jsonSerializerOptions);
		if (entityConfig == null)
			throw new ArgumentException("Failed to deserialize entity config.");

		return entityConfig;
	}
}
