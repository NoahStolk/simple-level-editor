namespace SimpleLevelEditor.Formats.Extensions;

internal static class BinaryReaderExtensions
{
	public static Vector2 ReadVector2(this BinaryReader br)
	{
		return new(br.ReadSingle(), br.ReadSingle());
	}

	public static Vector3 ReadVector3(this BinaryReader br)
	{
		return new(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
	}

	public static Vector4 ReadVector4(this BinaryReader br)
	{
		return new(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
	}

	public static Rgb ReadRgb(this BinaryReader br)
	{
		return new(br.ReadByte(), br.ReadByte(), br.ReadByte());
	}

	public static Rgba ReadRgba(this BinaryReader br)
	{
		return new(br.ReadByte(), br.ReadByte(), br.ReadByte(), br.ReadByte());
	}
}
