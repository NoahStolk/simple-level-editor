using System.Numerics;

namespace GameEntityConfig.Core.Components;

public readonly record struct Rotation
{
	public readonly Vector3 Value;

	public Rotation(Vector3 value)
	{
		Value = value;
	}
}
