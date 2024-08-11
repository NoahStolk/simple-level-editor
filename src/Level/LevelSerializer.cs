using System.Text.Json;
using System.Text.Json.Serialization;

namespace Level;

public static class LevelSerializer
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

	public static string Serialize(Core.Level level)
	{
		return JsonSerializer.Serialize(level, _jsonSerializerOptions);
	}

	public static Core.Level Deserialize(string json)
	{
		Core.Level? level = JsonSerializer.Deserialize<Core.Level>(json, _jsonSerializerOptions);
		if (level == null)
			throw new ArgumentException("Failed to deserialize Level");

		return level;
	}
}
