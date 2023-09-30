namespace SimpleLevelEditor.Maths;

public readonly record struct Ray(Vector3 Position, Vector3 Direction)
{
	public Vector3? Intersects(Vector3 p1, Vector3 p2, Vector3 p3)
	{
		const float epsilon = 0.0000001f;

		Vector3 edge1 = p2 - p1;
		Vector3 edge2 = p3 - p1;
		Vector3 h = Vector3.Cross(Direction, edge2);
		float a = Vector3.Dot(edge1, h);
		if (a is > -epsilon and < epsilon)
			return null; // Ray is parallel to the triangle.

		float f = 1.0f / a;
		Vector3 s = Position - p1;
		float u = f * Vector3.Dot(s, h);
		if (u is < 0.0f or > 1.0f)
			return null;

		Vector3 q = Vector3.Cross(s, edge1);
		float v = f * Vector3.Dot(Direction, q);
		if (v < 0.0f || u + v > 1.0f)
			return null;

		// At this stage we can compute t to find out where the intersection point is on the line.
		float t = f * Vector3.Dot(edge2, q);
		if (t <= epsilon)
		{
			// This means that there is a line intersection but not a ray intersection.
			return null;
		}

		return Position + Direction * t;
	}

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
