using OneOf;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Model.EntityTypes;
using SimpleLevelEditor.Model.Enums;

namespace SimpleLevelEditor.Model;

public class Entity
{
	public required string Name { get; set; }

	public required OneOf<Point, Sphere, Aabb, StandingCylinder> Shape { get; set; }

	public required List<EntityProperty> Properties { get; set; }

	public static Entity Read(BinaryReader br)
	{
		string name = br.ReadString();

		EntityType shapeType = (EntityType)br.ReadByte();
		OneOf<Point, Sphere, Aabb, StandingCylinder> shape = shapeType switch
		{
			EntityType.Point => new Point(br.ReadVector3()),
			EntityType.Sphere => new Sphere(br.ReadVector3(), br.ReadSingle()),
			EntityType.Aabb => new Aabb(br.ReadVector3(), br.ReadVector3()),
			EntityType.StandingCylinder => new StandingCylinder(br.ReadVector3(), br.ReadSingle(), br.ReadSingle()),
			_ => throw new InvalidDataException("Invalid shape type."),
		};

		ushort propertyCount = br.ReadUInt16();
		List<EntityProperty> properties = new();
		for (int j = 0; j < propertyCount; j++)
			properties.Add(EntityProperty.Read(br));

		return new()
		{
			Name = name,
			Shape = shape,
			Properties = properties,
		};
	}

	public void Write(BinaryWriter bw)
	{
		bw.Write(Name);
		switch (Shape.Value)
		{
			case Point p:
				bw.Write((byte)EntityType.Point);
				bw.Write(p.Position);
				break;
			case Sphere s:
				bw.Write((byte)EntityType.Sphere);
				bw.Write(s.Position);
				bw.Write(s.Radius);
				break;
			case Aabb a:
				bw.Write((byte)EntityType.Aabb);
				bw.Write(a.Min);
				bw.Write(a.Max);
				break;
			case StandingCylinder sc:
				bw.Write((byte)EntityType.StandingCylinder);
				bw.Write(sc.Position);
				bw.Write(sc.Radius);
				bw.Write(sc.Height);
				break;
		}

		bw.Write((ushort)Properties.Count);
		foreach (EntityProperty property in Properties)
			property.Write(bw);
	}
}
