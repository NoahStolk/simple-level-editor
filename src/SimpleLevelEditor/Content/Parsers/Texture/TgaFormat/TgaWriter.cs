using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace SimpleLevelEditor.Content.Parsers.Texture.TgaFormat;

public static class TgaWriter
{
	public static byte[] Write(TextureData textureData)
	{
		TgaHeader header = new(
			idLength: 0,
			colorMapType: TgaColorMapType.NoColorMap,
			imageType: TgaImageType.RunLengthEncodedTrueColor,
			colorMapStartIndex: 0,
			colorMapLength: 0,
			colorMapEntrySize: 0,
			originX: 0,
			originY: 0,
			width: textureData.Width,
			height: textureData.Height,
			pixelDepth: 32,
			imageDescriptor: 0b00101000);

		byte[] colorData = textureData.ColorData;
		TgaImageData imageData = new(textureData.Width, textureData.Height, Unsafe.As<byte[], ImmutableArray<byte>>(ref colorData), rightToLeft: false, topToBottom: true, runLengthEncoding: true, TgaPixelDepth.Bgra);

		using MemoryStream ms = new();
		using BinaryWriter bw = new(ms);
		header.Write(bw);
		imageData.Write(bw);
		return ms.ToArray();
	}
}
