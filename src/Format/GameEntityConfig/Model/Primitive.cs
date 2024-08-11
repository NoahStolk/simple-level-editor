namespace Format.GameEntityConfig.Model;

public enum Primitive
{
	Bool,
	I8,
	I16,
	I32,
	I64,
	I128,
	U8,
	U16,
	U32,
	U64,
	U128,
	F16,
	F32,
	F64,
	Str,
}

#pragma warning disable SA1649
public static class Primitives
#pragma warning restore SA1649
{
	private static readonly List<Primitive> _all = Enum.GetValues<Primitive>().ToList();

	public static IReadOnlyList<Primitive> All => _all;
}
