namespace SimpleLevelEditor.Model.EntityTypes;

public struct Aabb
{
	public Vector3 Min;
	public Vector3 Max;

	public Aabb(Vector3 min, Vector3 max)
	{
		Min = min;
		Max = max;
	}
}
