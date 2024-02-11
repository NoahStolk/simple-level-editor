namespace SimpleLevelEditor.Model.Level.EntityShapes;

public record Sphere(float Radius) : IEntityShape
{
	public float Radius = Radius;

	public Sphere DeepCopy()
	{
		return new(Radius);
	}
}
