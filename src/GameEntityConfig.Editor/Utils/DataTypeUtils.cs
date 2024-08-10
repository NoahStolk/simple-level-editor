namespace GameEntityConfig.Editor.Utils;

public static class DataTypeUtils
{
	private static readonly Dictionary<Type, string> _primitives = new()
	{
		{ typeof(bool), "bool" },
		{ typeof(sbyte), "i8" },
		{ typeof(short), "i16" },
		{ typeof(int), "i32" },
		{ typeof(long), "i64" },
		{ typeof(Int128), "i128" },
		{ typeof(byte), "u8" },
		{ typeof(ushort), "u16" },
		{ typeof(uint), "u32" },
		{ typeof(ulong), "u64" },
		{ typeof(UInt128), "u128" },
		{ typeof(Half), "f16" },
		{ typeof(float), "f32" },
		{ typeof(double), "f64" },
		{ typeof(decimal), "d128" },
		{ typeof(char), "char" },
		{ typeof(string), "str" },
	};

	public static IReadOnlyDictionary<Type, string> Primitives => _primitives;
}
