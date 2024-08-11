using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameEntityConfig;

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

	public static string Serialize(Core.GameEntityConfig config)
	{
		return JsonSerializer.Serialize(config, _jsonSerializerOptions);
	}

	public static Core.GameEntityConfig Deserialize(string json)
	{
		Core.GameEntityConfig? gameEntityConfig = JsonSerializer.Deserialize<Core.GameEntityConfig>(json, _jsonSerializerOptions);
		if (gameEntityConfig == null)
			throw new ArgumentException("Failed to deserialize GameEntityConfig");

		return gameEntityConfig;
	}
}
