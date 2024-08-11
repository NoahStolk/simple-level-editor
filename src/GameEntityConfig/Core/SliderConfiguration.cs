using System.Text.Json.Serialization;

namespace GameEntityConfig.Core;

public sealed record SliderConfiguration
{
	public readonly float Step;
	public readonly float Min;
	public readonly float Max;

	[JsonConstructor]
	public SliderConfiguration(float step, float min, float max)
	{
		Step = step;
		Min = min;
		Max = max;
	}
}
