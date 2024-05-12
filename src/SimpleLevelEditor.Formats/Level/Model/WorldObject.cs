namespace SimpleLevelEditor.Formats.Level.Model;

public record WorldObject
{
	/// <summary>
	/// The Id is only used to keep track of the object in the editor.
	/// </summary>
	// TODO: Move to UI layer.
	public required int Id;

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
		newFlags.AddRange(Flags);

		return this with
		{
			Flags = newFlags,
		};
	}
}
