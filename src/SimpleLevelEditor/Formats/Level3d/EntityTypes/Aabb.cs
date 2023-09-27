namespace SimpleLevelEditor.Formats.Level3d.EntityTypes;

public struct Aabb
{
	public Aabb(Vector3 min, Vector3 max)
	{
		Min = min;
		Max = max;
	}

	public Vector3 Min { get; set; }

	public Vector3 Max { get; set; }
}
