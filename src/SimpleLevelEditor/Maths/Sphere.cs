namespace SimpleLevelEditor.Maths;

public record struct Sphere(Vector3 Origin, float Radius)
{
	public bool Contains(Vector3 point)
	{
		return Vector3.DistanceSquared(Origin, point) <= Radius * Radius;
	}

	public bool Intersects(Sphere other)
	{
		return Vector3.DistanceSquared(Origin, other.Origin) <= (Radius + other.Radius) * (Radius + other.Radius);
	}
}
