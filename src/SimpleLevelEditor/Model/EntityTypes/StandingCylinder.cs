namespace SimpleLevelEditor.Model.EntityTypes;

public struct StandingCylinder
{
	public Vector3 Position;
	public float Radius;
	public float Height;

	public StandingCylinder(Vector3 position, float radius, float height)
	{
		Position = position;
		Radius = radius;
		Height = height;
	}
}
