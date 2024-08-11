using Format.GameEntityConfig.Model;
using System.Text.RegularExpressions;

namespace Format.GameEntityConfig;

public static partial class DataTypeBuilder
{
	public static DataType Build(string typeName, List<DataTypeField> fields)
	{
		// Type name must be a valid C# type name.
		if (!IsValidTypeName(typeName))
			throw new ArgumentException($"Type name '{typeName}' is not a valid C# type name.");

		if (fields.GroupBy(field => field.Name).Any(group => group.Count() > 1))
			throw new ArgumentException("Duplicate field names are not allowed.");

		// Field names must be valid C# field names.
		if (fields.Exists(field => !IsValidFieldName(field.Name)))
			throw new ArgumentException("Field names must be valid C# field names.");

		return new DataType(typeName, fields);
	}

	public static bool IsValidTypeName(string typeName)
	{
		return TypeNameRegex().IsMatch(typeName);
	}

	public static bool IsValidFieldName(string fieldName)
	{
		return FieldNameRegex().IsMatch(fieldName);
	}

	[GeneratedRegex("^[a-zA-Z_][a-zA-Z0-9_]*$")]
	private static partial Regex TypeNameRegex();

	[GeneratedRegex("^[a-zA-Z_][a-zA-Z0-9_]*$")]
	private static partial Regex FieldNameRegex();
}
