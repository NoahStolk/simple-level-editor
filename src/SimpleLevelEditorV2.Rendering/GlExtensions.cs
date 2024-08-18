using Silk.NET.OpenGL;
using System.Numerics;

namespace SimpleLevelEditorV2.Rendering;

public static class GlExtensions
{
	// ReSharper disable once InconsistentNaming
	public static void UniformMatrix4x4(this GL gl, int uniformLocation, Matrix4x4 value)
	{
		Span<float> data = stackalloc float[16]
		{
			value.M11, value.M12, value.M13, value.M14,
			value.M21, value.M22, value.M23, value.M24,
			value.M31, value.M32, value.M33, value.M34,
			value.M41, value.M42, value.M43, value.M44,
		};
		gl.UniformMatrix4(uniformLocation, 1, false, data);
	}
}
