using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleLevelEditor.Formats;

public class JsonVector3Converter : JsonConverter<Vector3>
{
	public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		string? token = reader.GetString();
		if (token == null)
			throw new JsonException($"Expected string value for {nameof(Vector3)}.");

		string[] values = token.Split(',');
		if (values.Length != 3)
			throw new JsonException($"Invalid format for {nameof(Vector3)}. Expected 'x,y,z', got '{token}'.");

		if (!float.TryParse(values[0], NumberStyles.Float, CultureInfo.InvariantCulture, out float x) ||
			!float.TryParse(values[1], NumberStyles.Float, CultureInfo.InvariantCulture, out float y) ||
			!float.TryParse(values[2], NumberStyles.Float, CultureInfo.InvariantCulture, out float z))
		{
			throw new JsonException($"Invalid format for {nameof(Vector3)}. Expected 'x,y,z', got '{token}'.");
		}

		return new Vector3(x, y, z);
	}

	public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
	{
		string x = value.X.ToString(CultureInfo.InvariantCulture);
		string y = value.Y.ToString(CultureInfo.InvariantCulture);
		string z = value.Z.ToString(CultureInfo.InvariantCulture);

		writer.WriteStringValue($"{x},{y},{z}");
	}
}
