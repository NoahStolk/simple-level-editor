namespace GameEntityConfig.Core;

public sealed record FixedComponent
{
	public FixedComponent(DataType dataType, string value)
	{
		DataType = dataType;
		Value = value;
	}

	public DataType DataType { get; }
	public string Value { get; }
}
