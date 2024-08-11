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

	public static string Serialize(Model.GameEntityConfigModel config)
	{
		return JsonSerializer.Serialize(config, _jsonSerializerOptions);
	}

	public static Model.GameEntityConfigModel Deserialize(string json)
	{
		Model.GameEntityConfigModel? gameEntityConfig = JsonSerializer.Deserialize<Model.GameEntityConfigModel>(json, _jsonSerializerOptions);
		if (gameEntityConfig == null)
			throw new ArgumentException("Failed to deserialize GameEntityConfig");

		return gameEntityConfig;
	}
}
