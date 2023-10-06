using System.Runtime.CompilerServices;

namespace SimpleLevelEditor.Utils;

public static class MathUtils
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ToRadians(float degrees)
	{
		return degrees * MathF.PI / 180;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 ToRadians(Vector3 degrees)
	{
		return degrees * MathF.PI / 180;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ToDegrees(float radians)
	{
		return radians * 180 / MathF.PI;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Vector3 ToDegrees(Vector3 radians)
	{
		return radians * 180 / MathF.PI;
	}

	public static Matrix4x4 CreateRotationMatrixFromEulerAngles(Vector3 eulerAngles)
	{
		return Matrix4x4.CreateFromYawPitchRoll(eulerAngles.Y, eulerAngles.X, eulerAngles.Z);
	}
}
