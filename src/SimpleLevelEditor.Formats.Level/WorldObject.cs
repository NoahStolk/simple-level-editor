using System.Numerics;

namespace SimpleLevelEditor.Formats.Level;

public sealed record WorldObject
{
	public required int Id { get; init; }

	public required string ModelPath { get; set; }

	public required Vector3 Scale { get; set; }

	public required Vector3 Rotation { get; set; }

	public required Vector3 Position { get; set; }

	public required List<string> Flags { get; set; }

	public WorldObject DeepCopy()
	{
		return this with
		{
			Flags = Flags.ToList(),
		};
	}

	public WorldObject CloneAndPlaceAtPosition(int objectId, Vector3 position)
	{
		return this with
		{
			Id = objectId,
			Position = position,
			Flags = Flags.ToList(),
		};
	}

	public void ChangeFlagAtIndex(int index, string flag)
	{
		if (index < 0 || index >= Flags.Count)
			return;

		Flags[index] = flag;
	}

	public void AddFlag(string flag)
	{
		Flags.Add(flag);
	}

	public void RemoveFlagAtIndex(int index)
	{
		if (index < 0 || index >= Flags.Count)
			return;

		Flags.RemoveAt(index);
	}

	public static WorldObject CreateDefault()
	{
		return new WorldObject
		{
			Id = 0,
			ModelPath = string.Empty,
			Scale = Vector3.One,
			Rotation = Vector3.Zero,
			Position = Vector3.Zero,
			Flags = [],
		};
	}
}
