namespace GameEntityConfig.Core;

public record struct SliderConfiguration<T>
{
	public SliderConfiguration(T step, T min, T max)
	{
		Step = step;
		Min = min;
		Max = max;
	}

	public readonly T Step;
	public readonly T Min;
	public readonly T Max;
}
