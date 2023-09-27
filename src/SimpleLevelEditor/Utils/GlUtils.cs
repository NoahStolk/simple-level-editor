using Silk.NET.OpenGL;

namespace SimpleLevelEditor.Utils;

public static class GlUtils
{
	public static unsafe void BufferData<T>(BufferTargetARB target, T[] data, BufferUsageARB usage)
		where T : unmanaged
	{
		fixed (T* d = data)
			Graphics.Gl.BufferData(target, (uint)(data.Length * sizeof(T)), d, usage);
	}

	public static unsafe void TexImageRgba2D(uint width, uint height, byte[] pixels)
	{
		fixed (byte* b = pixels)
			Graphics.Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, width, height, 0, GLEnum.Rgba, PixelType.UnsignedByte, b);
	}
}
