using SimpleLevelEditor.Utils;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace SimpleLevelEditor.ContentParsers.Texture.TgaFormat;

public static class TgaParser
{
	public static TextureData Parse(byte[] fileContents)
	{
		using MemoryStream ms = new(fileContents);
		using BinaryReader br = new(ms);

		// Read and validate the header.
		TgaHeader header = TgaHeader.Read(br);

		if (header.ColorMapType != TgaColorMapType.NoColorMap)
			throw new TextureParseException($"TGA with color map type {header.ColorMapType} is not supported.");

		if (header.ImageType is not (TgaImageType.TrueColor or TgaImageType.RunLengthEncodedTrueColor))
			throw new TextureParseException($"TGA with image type {header.ImageType} is not supported.");

		// Determine how to decode the image data.
		bool rightToLeft = BitUtils.IsBitSet(header.ImageDescriptor, 4);
		bool topToBottom = BitUtils.IsBitSet(header.ImageDescriptor, 5);

		TgaPixelDepth pixelDepth = header.PixelDepth switch
		{
			32 => TgaPixelDepth.Bgra,
			24 => TgaPixelDepth.Bgr,
			_ => throw new TextureParseException($"TGA with pixel depth {header.PixelDepth} is not supported."),
		};

		// Skip image ID and color map.
		br.BaseStream.Seek(header.IdLength, SeekOrigin.Current);
		br.BaseStream.Seek(header.ColorMapLength, SeekOrigin.Current);

		byte[] remainingBuffer = br.ReadBytes((int)(br.BaseStream.Length - br.BaseStream.Position));
		TgaImageData imageData = new(header.Width, header.Height, Unsafe.As<byte[], ImmutableArray<byte>>(ref remainingBuffer), rightToLeft, topToBottom, header.ImageType.IsRunLengthEncoded(), pixelDepth);
		return new(header.Width, header.Height, imageData.Read());
	}
}
