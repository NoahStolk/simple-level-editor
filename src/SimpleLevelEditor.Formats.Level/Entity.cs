using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace SimpleLevelEditor.Formats.Level;

public sealed record Entity
{
	public required int Id { get; init; }

	public required string Name { get; set; }

	public required Vector3 Position { get; set; }

	public required EntityShape Shape { get; set; }

	public required List<EntityProperty> Properties { get; set; }

	[SetsRequiredMembers]
	public Entity(int id, string name, Vector3 position, EntityShape shape, List<EntityProperty> properties)
	{
		Id = id;
		Name = name;
		Position = position;
		Shape = shape;
		Properties = properties;
	}

	public Entity DeepCopy()
	{
		return this with
		{
			Shape = Shape.DeepCopy(),
			Properties = Properties.Select(p => p.DeepCopy()).ToList(),
		};
	}

	public Entity CloneAndPlaceAtPosition(int entityId, Vector3 position)
	{
		return this with
		{
			Id = entityId,
			Position = position,
			Shape = Shape.DeepCopy(),
			Properties = Properties.Select(p => p.DeepCopy()).ToList(),
		};
	}

	public static Entity CreateDefault()
	{
		return new Entity(0, string.Empty, Vector3.Zero, new EntityShape.Point(), []);
	}
}
