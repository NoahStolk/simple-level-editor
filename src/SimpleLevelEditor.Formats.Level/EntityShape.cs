using Dunet;
using SimpleLevelEditor.Formats.Core;
using System.Diagnostics;
using System.Numerics;
using System.Text.Json.Serialization;

namespace SimpleLevelEditor.Formats.Level;

[Union]
[JsonDerivedType(typeof(Point), typeDiscriminator: nameof(Point))]
[JsonDerivedType(typeof(Sphere), typeDiscriminator: nameof(Sphere))]
[JsonDerivedType(typeof(Aabb), typeDiscriminator: nameof(Aabb))]
public partial record EntityShape
{
	public sealed partial record Point;
	public sealed partial record Sphere(float Radius);
	public sealed partial record Aabb(Vector3 Size);

	public EntityShape DeepCopy()
	{
		return this switch
		{
			Point => new Point(),
			Sphere sphere => new Sphere(sphere.Radius),
			Aabb aabb => new Aabb(aabb.Size),
			_ => throw new UnreachableException($"Unknown type: {GetType().FullName}"),
		};
	}

	public string ToDisplayString()
	{
		return this switch
		{
			Point => string.Empty,
			Sphere sphere => sphere.Radius.ToDisplayString(),
			Aabb aabb => aabb.Size.ToDisplayString(),
			_ => throw new UnreachableException($"Unknown type: {GetType().FullName}"),
		};
	}
}
