using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleLevelEditor.Formats;

public class JsonVector2Converter : JsonConverter<Vector2>
{
	public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		string? token = reader.GetString();
		if (token == null)
			throw new JsonException($"Expected string value for {nameof(Vector2)}.");

		string[] values = token.Split(',');
		if (values.Length != 2)
			throw new JsonException($"Invalid format for {nameof(Vector2)}. Expected 'x,y', got '{token}'.");

		if (!float.TryParse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) ||
			!float.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y))
		{
			throw new JsonException($"Invalid format for {nameof(Vector2)}. Expected 'x,y', got '{token}'.");
		}

		return new Vector2(x, y);
	}

	public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
	{
		string x = value.X.ToString(CultureInfo.InvariantCulture);
		string y = value.Y.ToString(CultureInfo.InvariantCulture);

		writer.WriteStringValue($"{x},{y}");
	}
}
