namespace SimpleLevelEditor.Formats.Level;

internal static class LevelXmlDataFormatter
{
	public static Types.Level.EntityPropertyValue ReadProperty(string str)
	{
		int indexOfSpace = str.IndexOf(' ', StringComparison.Ordinal);
		string typeId = str[..indexOfSpace];
		string value = str[(indexOfSpace + 1)..];

		return Types.Level.EntityPropertyValue.FromTypeId(typeId, value);
	}

	public static Types.Level.ShapeDescriptor ReadShape(string str)
	{
		int indexOfFirstSpace = str.IndexOf(' ', StringComparison.Ordinal);
		string shapeId = indexOfFirstSpace == -1 ? str : str[..indexOfFirstSpace];
		string shapeData = indexOfFirstSpace == -1 ? string.Empty : str[(indexOfFirstSpace + 1)..];

		return Types.Level.ShapeDescriptor.FromShapeId(shapeId, shapeData);
	}
}
