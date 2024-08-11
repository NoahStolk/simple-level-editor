namespace GameEntityConfig.Core;

public sealed record VaryingComponent
{
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
