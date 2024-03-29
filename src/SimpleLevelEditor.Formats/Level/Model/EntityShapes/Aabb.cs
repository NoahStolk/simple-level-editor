namespace SimpleLevelEditor.Formats.Level.Model.EntityShapes;

public record Aabb(Vector3 Min, Vector3 Max)
{
	public Vector3 Min = Min;
	public Vector3 Max = Max;

	public Aabb DeepCopy()
	{
		return new(Min, Max);
	}
}
