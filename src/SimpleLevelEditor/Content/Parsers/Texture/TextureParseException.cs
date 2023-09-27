using System.Runtime.Serialization;

namespace SimpleLevelEditor.Content.Parsers.Texture;

[Serializable]
public class TextureParseException : Exception
{
	public TextureParseException()
	{
	}

	public TextureParseException(string? message)
		: base(message)
	{
	}

	public TextureParseException(string? message, Exception? innerException)
		: base(message, innerException)
	{
	}

	protected TextureParseException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
