namespace SimpleLevelEditor.Formats.Model.Level.EntityShapes;

public record Sphere(float Radius)
{
	public float Radius = Radius;

	public Sphere DeepCopy()
	{
		return new(Radius);
	}
}
