using Dunet;

namespace GameEntityConfig.Core;

[Union]
public partial record Primitive
{
	public sealed partial record Bool;
	public sealed partial record I8;
	public sealed partial record I16;
	public sealed partial record I32;
	public sealed partial record I64;
	public sealed partial record I128;
	public sealed partial record U8;
	public sealed partial record U16;
	public sealed partial record U32;
	public sealed partial record U64;
	public sealed partial record U128;
	public sealed partial record F16;
	public sealed partial record F32;
	public sealed partial record F64;
	public sealed partial record Str;

	public static List<Primitive> All { get; } =
	[
		new Bool(),
		new I8(),
		new I16(),
		new I32(),
		new I64(),
		new I128(),
		new U8(),
		new U16(),
		new U32(),
		new U64(),
		new U128(),
		new F16(),
		new F32(),
		new F64(),
		new Str(),
	];

	public string SerializeAs(object obj)
	{
		return Match(
			_ => As<bool>(obj),
			_ => As<sbyte>(obj),
			_ => As<short>(obj),
			_ => As<int>(obj),
			_ => As<long>(obj),
			_ => As<Int128>(obj),
			_ => As<byte>(obj),
			_ => As<ushort>(obj),
			_ => As<uint>(obj),
			_ => As<ulong>(obj),
			_ => As<UInt128>(obj),
			_ => As<Half>(obj),
			_ => As<float>(obj),
			_ => As<double>(obj),
			_ => As<string>(obj));

		static string As<T>(object obj)
		{
			if (obj is T val)
				return val.ToString() ?? string.Empty;

			throw new ArgumentException($"Expected {nameof(obj)} to be of type {typeof(T).Name}.");
		}
	}
}
