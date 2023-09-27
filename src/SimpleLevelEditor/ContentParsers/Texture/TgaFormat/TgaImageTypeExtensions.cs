using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.ContentParsers.Texture.TgaFormat;

internal static class TgaImageTypeExtensions
{
	public static bool IsRunLengthEncoded(this TgaImageType imageType)
	{
		byte imageTypeValue = (byte)imageType;
		return BitUtils.IsBitSet(imageTypeValue, 3);
	}
}
