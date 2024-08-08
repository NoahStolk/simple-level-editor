using System.Numerics;

namespace GameEntityConfig.Core.Components;

public readonly record struct Scale
{
	public readonly Vector3 Value;

	public Scale(Vector3 value)
	{
		Value = value;
	}
}
