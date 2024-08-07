using Dunet;
using SimpleLevelEditor.Formats.Core;
using System.Diagnostics;

namespace SimpleLevelEditor.Formats.Level;

[Union]
public partial record EntityPropertyValue
{
	// TODO: Use a better naming convention:
	// Bool
	// I32
	// F32
	// Vec2
	// Vec3
	// Vec4
	// Str
	// Rgba
	// Rgb
	public sealed partial record Bool(bool Value);
	public sealed partial record Int(int Value);
	public sealed partial record Float(float Value);
	public sealed partial record Vector2(System.Numerics.Vector2 Value);
	public sealed partial record Vector3(System.Numerics.Vector3 Value);
	public sealed partial record Vector4(System.Numerics.Vector4 Value);
	public sealed partial record String(string Value);
	public sealed partial record Rgba(Formats.Core.Rgba Value);
	public sealed partial record Rgb(Formats.Core.Rgb Value);

	public EntityPropertyValue DeepCopy()
	{
		return this switch
		{
			Bool b => new Bool(b.Value),
			Int i => new Int(i.Value),
			Float f => new Float(f.Value),
			Vector2 v2 => new Vector2(v2.Value),
			Vector3 v3 => new Vector3(v3.Value),
			Vector4 v4 => new Vector4(v4.Value),
			String s => new String(s.Value),
			Rgba rgba => new Rgba(rgba.Value),
			Rgb rgb => new Rgb(rgb.Value),
			_ => throw new UnreachableException(),
		};
	}

	public string ToDisplayString()
	{
		return this switch
		{
			Bool b => b.Value.ToDisplayString(),
			Int i => i.Value.ToDisplayString(),
			Float f => f.Value.ToDisplayString(),
			Vector2 v2 => v2.Value.ToDisplayString(),
			Vector3 v3 => v3.Value.ToDisplayString(),
			Vector4 v4 => v4.Value.ToDisplayString(),
			String s => s.Value,
			Rgba rgba => rgba.Value.ToDisplayString(),
			Rgb rgb => rgb.Value.ToDisplayString(),
			_ => throw new UnreachableException(),
		};
	}
}
