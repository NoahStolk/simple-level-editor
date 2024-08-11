using System.Text.Json.Serialization;

namespace Format.GameEntityConfig.Model;

public sealed record VaryingComponent
{
	[JsonConstructor]
	public VaryingComponent(DataType dataType, string defaultValue, SliderConfiguration? sliderConfiguration)
	{
		DataType = dataType;
		DefaultValue = defaultValue;
		SliderConfiguration = sliderConfiguration;
	}

	[JsonIgnore]
	public DataType DataType { get; }

	[JsonInclude]
	public string DataTypeName => DataType.Name;

	public string DefaultValue { get; }

	public SliderConfiguration? SliderConfiguration { get; }
}
