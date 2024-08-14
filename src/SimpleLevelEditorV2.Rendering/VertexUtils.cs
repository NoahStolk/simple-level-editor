using System.Numerics;

namespace SimpleLevelEditorV2.Rendering;

public static class VertexUtils
{
	public static Vector3[] GetSphereVertexPositions(uint horizontalLines, uint verticalLines, float radius)
	{
		List<Vector3> vertices = [];
		for (uint i = 0; i <= horizontalLines; i++)
		{
			float horizontalAngle = MathF.PI * i / horizontalLines;

			for (uint j = 0; j <= verticalLines; j++)
			{
				float verticalAngle = 2 * MathF.PI * j / verticalLines;

				float x = MathF.Sin(horizontalAngle) * MathF.Cos(verticalAngle);
				float y = MathF.Cos(horizontalAngle);
				float z = MathF.Sin(horizontalAngle) * MathF.Sin(verticalAngle);

				if (j != 0 && j != verticalLines)
					vertices.Add(new Vector3(x, y, z) * radius);

				vertices.Add(new Vector3(x, y, z) * radius);
			}
		}

		for (uint i = 0; i <= verticalLines; i++)
		{
			float verticalAngle = 2 * MathF.PI * i / verticalLines;

			for (uint j = 0; j <= horizontalLines; j++)
			{
				float horizontalAngle = MathF.PI * j / horizontalLines;

				float x = MathF.Sin(horizontalAngle) * MathF.Cos(verticalAngle);
				float y = MathF.Cos(horizontalAngle);
				float z = MathF.Sin(horizontalAngle) * MathF.Sin(verticalAngle);

				if (j != 0 && j != horizontalLines)
					vertices.Add(new Vector3(x, y, z) * radius);

				vertices.Add(new Vector3(x, y, z) * radius);
			}
		}

		return vertices.ToArray();
	}
}
