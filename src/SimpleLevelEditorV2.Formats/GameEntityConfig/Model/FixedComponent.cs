using System.Text.Json.Serialization;

namespace SimpleLevelEditorV2.Formats.GameEntityConfig.Model;

public sealed record FixedComponent
{
	[JsonConstructor]
	public FixedComponent(DataType dataType, string value)
	{
		DataType = dataType;
		Value = value;
	}

	[JsonIgnore]
	public DataType DataType { get; }

	[JsonInclude]
	public string DataTypeName => DataType.Name;

	public string Value { get; }
}
