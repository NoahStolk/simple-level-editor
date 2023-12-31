namespace SimpleLevelEditor.Model.EntityShapes;

public record Sphere(float Radius) : IEntityShape
{
	public float Radius = Radius;

	public Sphere DeepCopy()
	{
		return new(Radius);
	}
}
