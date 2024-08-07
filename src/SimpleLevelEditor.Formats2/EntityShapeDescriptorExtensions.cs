using SimpleLevelEditor.Formats.EntityConfig;
using SimpleLevelEditor.Formats.Level;
using System.Diagnostics;
using System.Numerics;

namespace SimpleLevelEditor.Formats2;

public static class EntityShapeDescriptorExtensions
{
	public static EntityShape GetDefaultEntityShape(this EntityShapeDescriptor entityShapeDescriptor)
	{
		return entityShapeDescriptor switch
		{
			EntityShapeDescriptor.Point => new EntityShape.Point(),
			EntityShapeDescriptor.Sphere => new EntityShape.Sphere(2),
			EntityShapeDescriptor.Aabb => new EntityShape.Aabb(Vector3.One),
			_ => throw new UnreachableException(),
		};
	}
}
