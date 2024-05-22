namespace SimpleLevelEditor.Utils;

public sealed class NormalVectorComparer : IEqualityComparer<Vector3>
{
	public static readonly NormalVectorComparer Instance = new();

	public bool Equals(Vector3 x, Vector3 y)
	{
		const float epsilon = 0.01f;
		return Math.Abs(x.X - y.X) < epsilon && Math.Abs(x.Y - y.Y) < epsilon && Math.Abs(x.Z - y.Z) < epsilon;
	}

	public int GetHashCode(Vector3 obj)
	{
		return obj.GetHashCode();
	}
}
