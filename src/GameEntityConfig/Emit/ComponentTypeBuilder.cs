using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;

namespace GameEntityConfig.Emit;

public static partial class ComponentTypeBuilder
{
	public static TypeInfo CompileResultTypeInfo(string typeName, List<FieldDescriptor> fields)
	{
		// Type name must be a valid C# type name.
		if (!IsValidTypeName(typeName))
			throw new ArgumentException($"Type name '{typeName}' is not a valid C# type name.");

		if (fields.GroupBy(field => field.FieldName).Any(group => group.Count() > 1))
			throw new ArgumentException("Duplicate field names are not allowed.");

		// Field names must be valid C# field names.
		if (fields.Exists(field => !IsValidFieldName(field.FieldName)))
			throw new ArgumentException("Field names must be valid C# field names.");

		TypeBuilder typeBuilder = GetTypeBuilder(typeName);

		foreach (FieldDescriptor field in fields)
			typeBuilder.DefineField(field.FieldName, field.FieldType, FieldAttributes.Public | FieldAttributes.InitOnly);

		return typeBuilder.CreateTypeInfo();
	}

	public static bool IsValidTypeName(string typeName)
	{
		return TypeNameRegex().IsMatch(typeName);
	}

	public static bool IsValidFieldName(string fieldName)
	{
		return FieldNameRegex().IsMatch(fieldName);
	}

	private static TypeBuilder GetTypeBuilder(string typeName)
	{
		const TypeAttributes typeAttributes = TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout;

		AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);
		ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
		return moduleBuilder.DefineType(typeName, typeAttributes, null);
	}

	[GeneratedRegex("^[a-zA-Z_][a-zA-Z0-9_]*$")]
	private static partial Regex TypeNameRegex();

	[GeneratedRegex("^[a-zA-Z_][a-zA-Z0-9_]*$")]
	private static partial Regex FieldNameRegex();
}
