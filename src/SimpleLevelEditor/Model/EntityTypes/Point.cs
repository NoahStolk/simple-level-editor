namespace SimpleLevelEditor.Model.EntityTypes;

public struct Point
{
	public Point(Vector3 position)
	{
		Position = position;
	}

	public Vector3 Position { get; set; }
}
