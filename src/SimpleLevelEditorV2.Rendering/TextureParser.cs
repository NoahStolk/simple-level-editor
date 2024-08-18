using Detach.Parsers.Texture;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Security;

namespace SimpleLevelEditorV2.Rendering;

public static class TextureParser
{
	public static TextureData? Parse(string absolutePath)
	{
		if (!File.Exists(absolutePath))
			return null;

		Image<Rgba32>? image = GetImage(absolutePath);
		if (image == null)
			return null;

		byte[] rgbaPixelData = new byte[image.Width * image.Height * 4];
		image.CopyPixelDataTo(rgbaPixelData);
		return new TextureData((ushort)image.Width, (ushort)image.Height, rgbaPixelData);
	}

	private static Image<Rgba32>? GetImage(string absolutePath)
	{
		try
		{
			byte[] data = File.ReadAllBytes(absolutePath);
			Image<Rgba32> image = Image.Load<Rgba32>(data);
			return image;
		}
		catch (Exception ex) when (ex is PathTooLongException or IOException or UnauthorizedAccessException or SecurityException)
		{
			// TODO: Logging.
			return null;
		}
		catch (Exception ex) when (ex is NotSupportedException or InvalidImageContentException or UnknownImageFormatException)
		{
			// TODO: Logging.
			return null;
		}
	}
}
