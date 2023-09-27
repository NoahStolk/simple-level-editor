using System.Runtime.CompilerServices;

namespace SimpleLevelEditor.Utils;

public static class MathUtils
{
	private const float _toRad = MathF.PI / 180;
	private const float _toDeg = 180 / MathF.PI;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ToDegrees(float radians)
		=> radians * _toDeg;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ToRadians(float degrees)
		=> degrees * _toRad;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float Lerp(float value1, float value2, float amount)
		=> value1 + (value2 - value1) * amount;

	public static bool IsFloatReal(float value)
		=> !float.IsNaN(value) && !float.IsInfinity(value);
}
