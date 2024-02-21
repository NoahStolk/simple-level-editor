namespace SimpleLevelEditor.Formats.Level.Model;

public record WorldObject
{
	/// <summary>
	/// Path to the mesh file.
	/// </summary>
	public required string Mesh;

	/// <summary>
	/// Path to the texture file.
	/// </summary>
	public required string Texture;

	/// <summary>
	/// Scale in units.
	/// </summary>
	public required Vector3 Scale;

	/// <summary>
	/// Rotation in degrees.
	/// </summary>
	public required Vector3 Rotation;

	/// <summary>
	/// Position in units.
	/// </summary>
	public required Vector3 Position;

	/// <summary>
	/// World object flags.
	/// </summary>
	public required List<string> Flags;

	public WorldObject DeepCopy()
	{
		List<string> newFlags = [];
		for (int i = 0; i < Flags.Count; i++)
			newFlags.Add(Flags[i]);

		return this with
		{
			Flags = newFlags,
		};
	}
}
