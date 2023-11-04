namespace SimpleLevelEditor.Model.EntityTypes;

public record Aabb
{
	public Vector3 Min;
	public Vector3 Max;

	public Aabb(Vector3 min, Vector3 max)
	{
		Min = min;
		Max = max;
	}
}
