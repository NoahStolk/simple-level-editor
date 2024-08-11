using System.Text.Json;
using System.Text.Json.Serialization;

namespace Format.GameEntityConfig;

public static class GameEntityConfigSerializer
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

	public static string Serialize(Model.GameEntityConfig config)
	{
		return JsonSerializer.Serialize(config, _jsonSerializerOptions);
	}

	public static Model.GameEntityConfig Deserialize(string json)
	{
		Model.GameEntityConfig? gameEntityConfig = JsonSerializer.Deserialize<Model.GameEntityConfig>(json, _jsonSerializerOptions);
		if (gameEntityConfig == null)
			throw new ArgumentException("Failed to deserialize GameEntityConfig");

		return gameEntityConfig;
	}
}
