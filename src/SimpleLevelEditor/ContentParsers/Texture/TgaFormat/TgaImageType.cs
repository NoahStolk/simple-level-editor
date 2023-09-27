namespace SimpleLevelEditor.ContentParsers.Texture.TgaFormat;

internal enum TgaImageType : byte
{
	NoImageData = 0,
	ColorMapped = 1,
	TrueColor = 2,
	Grayscale = 3,
	RunLengthEncodedColorMapped = 9,
	RunLengthEncodedTrueColor = 10,
	RunLengthEncodedGrayscale = 11,
}
