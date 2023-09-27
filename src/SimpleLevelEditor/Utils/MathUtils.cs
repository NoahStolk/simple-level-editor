using System.Runtime.CompilerServices;

namespace SimpleLevelEditor.Utils;

public static class MathUtils
{
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static float ToRadians(float degrees)
		=> degrees * MathF.PI / 180;
}
