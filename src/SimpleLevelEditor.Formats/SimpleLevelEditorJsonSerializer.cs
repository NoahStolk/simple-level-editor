using System.Text.Json;
using System.Text.Json.Serialization;

namespace SimpleLevelEditor.Formats;

public static class SimpleLevelEditorJsonSerializer
{
	static SimpleLevelEditorJsonSerializer()
	{
		DefaultSerializerOptions.WriteIndented = true;
		DefaultSerializerOptions.IncludeFields = true;
	}

	// TODO: Make this private.
	public static JsonSerializerOptions DefaultSerializerOptions { get; } = JsonFSharpOptions.Default().ToJsonSerializerOptions();

	// TODO: Add methods to serialize/deserialize levels and EntityConfig files.

	// TODO: Add this static class to the F# library as a module.

	// TODO: Rename the F# library to SimpleLevelEditor.Formats, delete this C# project, and replace the current NuGet package with the F# library.
}
