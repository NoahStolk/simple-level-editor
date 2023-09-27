namespace SimpleLevelEditor.Formats.Level3d.EntityTypes;

public struct StandingCylinder
{
	public StandingCylinder(Vector3 position, float radius, float height)
	{
		Position = position;
		Radius = radius;
		Height = height;
	}

	public Vector3 Position { get; set; }

	public float Radius { get; set; }

	public float Height { get; set; }
}
