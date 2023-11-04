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

	public Aabb GetCentered(Vector3 center)
	{
		Vector3 diff = Max - Min;
		Vector3 newMin = center - diff / 2;
		Vector3 newMax = center + diff / 2;
		return new(newMin, newMax);
	}
}
