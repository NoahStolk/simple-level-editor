using System.Runtime.Serialization;

namespace SimpleLevelEditor.ContentParsers.Model;

[Serializable]
public class ModelParseException : Exception
{
	public ModelParseException()
	{
	}

	public ModelParseException(string? message)
		: base(message)
	{
	}

	public ModelParseException(string? message, Exception? innerException)
		: base(message, innerException)
	{
	}

	protected ModelParseException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
}
