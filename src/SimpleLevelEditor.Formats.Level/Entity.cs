using System.Numerics;

namespace SimpleLevelEditor.Formats.Level;

public sealed record Entity
{
	public required int Id { get; init; }

	public required string Name { get; set; }

	public required Vector3 Position { get; set; }

	public required EntityShape Shape { get; set; }

	public required IReadOnlyList<EntityProperty> Properties { get; set; }

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
		return new Entity
		{
			Id = 0,
			Name = string.Empty,
			Position = Vector3.Zero,
			Shape = new EntityShape.Point(),
			Properties = [],
		};
	}
}
