using Dunet;
using SimpleLevelEditor.Formats.Core;
using System.Diagnostics;
using System.Numerics;

namespace SimpleLevelEditor.Formats.Level;

[Union]
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
			_ => throw new UnreachableException(),
		};
	}

	public string ToDisplayString()
	{
		return this switch
		{
			Point => string.Empty,
			Sphere sphere => sphere.Radius.ToDisplayString(),
			Aabb aabb => aabb.Size.ToDisplayString(),
			_ => throw new UnreachableException(),
		};
	}
}
