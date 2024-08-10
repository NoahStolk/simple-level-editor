using System.Reflection;
using System.Reflection.Emit;

namespace GameEntityConfig.Emit;

public static class ComponentTypeBuilder
{
	public static TypeInfo CompileResultTypeInfo(string typeName, List<FieldDescriptor> fields)
	{
		TypeBuilder typeBuilder = GetTypeBuilder(typeName);

		foreach (FieldDescriptor field in fields)
			typeBuilder.DefineField(field.FieldName, field.FieldType, FieldAttributes.Public | FieldAttributes.InitOnly);

		return typeBuilder.CreateTypeInfo();
	}

	private static TypeBuilder GetTypeBuilder(string typeName)
	{
		const TypeAttributes typeAttributes = TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout;

		AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.Run);
		ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
		return moduleBuilder.DefineType(typeName, typeAttributes, null);
	}
}
