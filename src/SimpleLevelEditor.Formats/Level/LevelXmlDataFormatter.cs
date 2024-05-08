using Microsoft.FSharp.Core;
using SimpleLevelEditor.Formats.Types.Level;

namespace SimpleLevelEditor.Formats.Level;

// TODO: Move to F#.
internal static class LevelXmlDataFormatter
{
	public static EntityPropertyValue ReadProperty(string str)
	{
		int indexOfSpace = str.IndexOf(' ', StringComparison.Ordinal);
		string typeId = str[..indexOfSpace];
		string value = str[(indexOfSpace + 1)..];

		// TODO: The null check probably doesn't even work because of the F# option type.
		FSharpOption<EntityPropertyValue>? parseResult = EntityPropertyValue.FromTypeId(typeId, value);
		if (parseResult == null)
			throw new InvalidOperationException($"Failed to parse property: {str}");

		return parseResult.Value;
	}

	public static ShapeDescriptor ReadShape(string str)
	{
		int indexOfFirstSpace = str.IndexOf(' ', StringComparison.Ordinal);
		string shapeId = indexOfFirstSpace == -1 ? str : str[..indexOfFirstSpace];
		string shapeData = indexOfFirstSpace == -1 ? string.Empty : str[(indexOfFirstSpace + 1)..];

		// TODO: The null check probably doesn't even work because of the F# option type.
		FSharpOption<ShapeDescriptor>? parseResult = ShapeDescriptor.FromShapeId(shapeId, shapeData);
		if (parseResult == null)
			throw new InvalidOperationException($"Failed to parse shape: {str}");

		return parseResult.Value;
	}
}
