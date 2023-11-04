namespace SimpleLevelEditor.Model.EntityTypes;

public record Point
{
	public Vector3 Position;

	public Point(Vector3 position)
	{
		Position = position;
	}
}
