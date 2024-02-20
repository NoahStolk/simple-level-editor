namespace SimpleLevelEditor.Formats.Level.Model.EntityShapes;

public record Sphere(float Radius)
{
	public float Radius = Radius;

	public Sphere DeepCopy()
	{
		return new(Radius);
	}
}
