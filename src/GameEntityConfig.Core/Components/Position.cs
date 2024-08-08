using System.Numerics;

namespace GameEntityConfig.Core.Components;

public readonly record struct Position
{
	public readonly Vector3 Value;

	public Position(Vector3 value)
	{
		Value = value;
	}
}
