namespace SimpleLevelEditor.Formats.Level3d.EntityTypes;

public struct Sphere
{
	public Sphere(Vector3 position, float radius)
	{
		Position = position;
		Radius = radius;
	}

	public Vector3 Position { get; set; }

	public float Radius { get; set; }
}
