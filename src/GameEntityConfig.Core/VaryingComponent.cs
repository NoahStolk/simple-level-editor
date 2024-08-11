using System.Text.Json.Serialization;

namespace GameEntityConfig.Core;

public sealed record VaryingComponent
{
	[JsonConstructor]
	public VaryingComponent(DataType dataType, string defaultValue, SliderConfiguration? sliderConfiguration)
	{
		DataType = dataType;
		DefaultValue = defaultValue;
		SliderConfiguration = sliderConfiguration;
	}

	public DataType DataType { get; }
	public string DefaultValue { get; }
	public SliderConfiguration? SliderConfiguration { get; }
}
