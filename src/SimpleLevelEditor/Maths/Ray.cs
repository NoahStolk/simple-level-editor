namespace SimpleLevelEditor.Maths;

public readonly record struct Ray(Vector3 Position, Vector3 Direction)
{
	public Vector3? Intersects(Sphere sphere)
	{
		Vector3 l = sphere.Origin - Position;
		float tca = Vector3.Dot(l, Direction);
		if (tca < 0)
			return null;

		float d2 = Vector3.Dot(l, l) - tca * tca;
		float r2 = sphere.Radius * sphere.Radius;
		if (d2 > r2)
			return null;

		float thc = MathF.Sqrt(r2 - d2);
		float t0 = tca - thc;
		float t1 = tca + thc;

		if (t0 > t1)
			(t0, t1) = (t1, t0);

		if (t0 < 0)
		{
			t0 = t1; // If t0 is negative, let's use t1 instead.
			if (t0 < 0)
				return null; // Both t0 and t1 are negative.
		}

		return Position + Direction * t0;
	}
}
