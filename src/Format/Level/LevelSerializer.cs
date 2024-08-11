using System.Text.Json;
using System.Text.Json.Serialization;

namespace Format.Level;

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

	public static string Serialize(Model.Level level)
	{
		return JsonSerializer.Serialize(level, _jsonSerializerOptions);
	}

	public static Model.Level Deserialize(string json)
	{
		Model.Level? level = JsonSerializer.Deserialize<Model.Level>(json, _jsonSerializerOptions);
		if (level == null)
			throw new ArgumentException("Failed to deserialize Level");

		return level;
	}
}
