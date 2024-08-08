using System.Numerics;

namespace GameEntityConfig.Core;

public abstract record VaryingComponent;

public sealed record VaryingComponent<T> : VaryingComponent
{
	internal VaryingComponent(T defaultValue)
	{
		DefaultValue = defaultValue;
	}

	public T DefaultValue { get; }
}

public sealed record VaryingComponent<T, TSlider> : VaryingComponent
	where TSlider : struct, INumber<TSlider>
{
	internal VaryingComponent(T defaultValue, SliderConfiguration<TSlider>? sliderConfiguration)
	{
		DefaultValue = defaultValue;
		SliderConfiguration = sliderConfiguration;
	}

	public T DefaultValue { get; }

	public SliderConfiguration<TSlider>? SliderConfiguration { get; }
}
