namespace SimpleLevelEditor.Formats.Extensions;

internal static class BinaryWriterExtensions
{
	public static void Write(this BinaryWriter bw, Vector2 vector)
	{
		bw.Write(vector.X);
		bw.Write(vector.Y);
	}

	public static void Write(this BinaryWriter bw, Vector3 vector)
	{
		bw.Write(vector.X);
		bw.Write(vector.Y);
		bw.Write(vector.Z);
	}

	public static void Write(this BinaryWriter bw, Vector4 vector)
	{
		bw.Write(vector.X);
		bw.Write(vector.Y);
		bw.Write(vector.Z);
		bw.Write(vector.W);
	}

	public static void Write(this BinaryWriter bw, Rgb rgb)
	{
		bw.Write(rgb.R);
		bw.Write(rgb.G);
		bw.Write(rgb.B);
	}

	public static void Write(this BinaryWriter bw, Rgba rgba)
	{
		bw.Write(rgba.R);
		bw.Write(rgba.G);
		bw.Write(rgba.B);
		bw.Write(rgba.A);
	}
}
