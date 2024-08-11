using System.Text.Json.Serialization;

namespace Format.GameEntityConfig.Model;

public sealed record FixedComponent
{
	[JsonConstructor]
	public FixedComponent(DataType dataType, string value)
	{
		DataType = dataType;
		Value = value;
	}

	public DataType DataType { get; }
	public string Value { get; }
}
