namespace SimpleLevelEditor.Content.Parsers.Texture.TgaFormat;

internal enum TgaColorMapType : byte
{
	NoColorMap = 0,
	ColorMap = 1,

	// 2..127 are reserved.
	// 128..255 are developer-specific.
}
