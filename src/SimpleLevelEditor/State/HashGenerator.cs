using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.Formats.Level.Model.EntityShapes;
using System.Diagnostics;

namespace SimpleLevelEditor.State;

public static class HashGenerator
{
	public static int GenerateHash(this WorldObject input)
	{
		int hash = 17;
		hash = hash * 23 + input.Mesh.GetHashCode(StringComparison.Ordinal);
		hash = hash * 23 + input.Texture.GetHashCode(StringComparison.Ordinal);
		hash = hash * 23 + input.Scale.GetHashCode();
		hash = hash * 23 + input.Rotation.GetHashCode();
		hash = hash * 23 + input.Position.GetHashCode();
		for (int i = 0; i < input.Flags.Count; i++)
			hash = hash * 23 + input.Flags[i].GetHashCode(StringComparison.Ordinal);

		return hash;
	}

	public static int GenerateHash(this Entity input)
	{
		int hash = 17;
		hash = hash * 23 + input.Name.GetHashCode(StringComparison.Ordinal);
		hash = hash * 23 + input.Position.GetHashCode();
		hash = hash * 23 + input.Shape.Value switch
		{
			Point => 17,
			Sphere sphere => sphere.GenerateHash(),
			Aabb aabb => aabb.GenerateHash(),
			_ => throw new UnreachableException(),
		};
		for (int i = 0; i < input.Properties.Count; i++)
		{
			hash = hash * 23 + input.Properties[i].Key.GetHashCode(StringComparison.Ordinal);
			hash = hash * 23 + input.Properties[i].Value.GetHashCode();
		}

		return hash;
	}

	private static int GenerateHash(this Sphere input)
	{
		const int hash = 17;
		return hash * 23 + input.Radius.GetHashCode();
	}

	private static int GenerateHash(this Aabb input)
	{
		int hash = 17;
		hash = hash * 23 + input.Min.GetHashCode();
		hash = hash * 23 + input.Max.GetHashCode();
		return hash;
	}
}
