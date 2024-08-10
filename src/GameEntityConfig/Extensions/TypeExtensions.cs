namespace GameEntityConfig.Extensions;

public static class TypeExtensions
{
	public static Type GetFirstTypeParameter(this Type type)
	{
		if (!type.IsGenericType)
			throw new ArgumentException($"Type '{type.Name}' is not generic.");

		Type[] genericArguments = type.GetGenericArguments();
		return genericArguments[0];
	}
}
