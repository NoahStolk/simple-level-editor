using System.Runtime.InteropServices;

namespace SimpleLevelEditor.Content.Parsers.Texture.TgaFormat;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct TgaHeader
{
	public readonly byte IdLength;
	public readonly TgaColorMapType ColorMapType;
	public readonly TgaImageType ImageType;
	public readonly ushort ColorMapStartIndex;
	public readonly ushort ColorMapLength;
	public readonly byte ColorMapEntrySize;
	public readonly ushort OriginX;
	public readonly ushort OriginY;
	public readonly ushort Width;
	public readonly ushort Height;
	public readonly byte PixelDepth;
	public readonly byte ImageDescriptor;

	public TgaHeader(byte idLength, TgaColorMapType colorMapType, TgaImageType imageType, ushort colorMapStartIndex, ushort colorMapLength, byte colorMapEntrySize, ushort originX, ushort originY, ushort width, ushort height, byte pixelDepth, byte imageDescriptor)
	{
		IdLength = idLength;
		ColorMapType = colorMapType;
		ImageType = imageType;
		ColorMapStartIndex = colorMapStartIndex;
		ColorMapLength = colorMapLength;
		ColorMapEntrySize = colorMapEntrySize;
		OriginX = originX;
		OriginY = originY;
		Width = width;
		Height = height;
		PixelDepth = pixelDepth;
		ImageDescriptor = imageDescriptor;
	}

	public static TgaHeader Read(BinaryReader br)
	{
		byte idLength = br.ReadByte();
		TgaColorMapType colorMapType = (TgaColorMapType)br.ReadByte();
		TgaImageType imageType = (TgaImageType)br.ReadByte();
		ushort colorMapStartIndex = br.ReadUInt16();
		ushort colorMapLength = br.ReadUInt16();
		byte colorMapEntrySize = br.ReadByte();
		ushort originX = br.ReadUInt16();
		ushort originY = br.ReadUInt16();
		ushort width = br.ReadUInt16();
		ushort height = br.ReadUInt16();
		byte pixelDepth = br.ReadByte();
		byte imageDescriptor = br.ReadByte();
		return new(idLength, colorMapType, imageType, colorMapStartIndex, colorMapLength, colorMapEntrySize, originX, originY, width, height, pixelDepth, imageDescriptor);
	}

	public void Write(BinaryWriter bw)
	{
		bw.Write(IdLength);
		bw.Write((byte)ColorMapType);
		bw.Write((byte)ImageType);
		bw.Write(ColorMapStartIndex);
		bw.Write(ColorMapLength);
		bw.Write(ColorMapEntrySize);
		bw.Write(OriginX);
		bw.Write(OriginY);
		bw.Write(Width);
		bw.Write(Height);
		bw.Write(PixelDepth);
		bw.Write(ImageDescriptor);
	}
}
