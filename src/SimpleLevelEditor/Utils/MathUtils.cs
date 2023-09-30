using System.Runtime.CompilerServices;

namespace SimpleLevelEditor.Utils;

public static class MathUtils
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ToRadians(float degrees)
	{
		return degrees * MathF.PI / 180;
	}

	public static Matrix4x4 CreateRotationMatrixFromEulerAngles(Vector3 eulerAngles)
	{
		return Matrix4x4.CreateFromYawPitchRoll(ToRadians(eulerAngles.X), ToRadians(eulerAngles.Y), ToRadians(eulerAngles.Z));
	}
}
