using Detach.Collisions;
using SimpleLevelEditor.Rendering;
using System.Diagnostics.CodeAnalysis;

namespace SimpleLevelEditor.Utils;

public static class RaycastUtils
{
	public static float? RaycastPlane(Matrix4x4 modelMatrix, Vector3 rayStartPosition, Vector3 rayDirection)
	{
		Vector3 p1 = Vector3.Transform(new Vector3(-1, -1, 0), modelMatrix);
		Vector3 p2 = Vector3.Transform(new Vector3(+1, -1, 0), modelMatrix);
		Vector3 p3 = Vector3.Transform(new Vector3(+1, +1, 0), modelMatrix);
		Vector3 p4 = Vector3.Transform(new Vector3(-1, +1, 0), modelMatrix);

		Vector3? t1 = Ray.IntersectsTriangle(rayStartPosition, rayDirection, p1, p2, p3);
		Vector3? t2 = Ray.IntersectsTriangle(rayStartPosition, rayDirection, p1, p3, p4);
		if (t1 == null && t2 == null)
			return null;

		float t1Distance = t1.HasValue ? Vector3.DistanceSquared(rayStartPosition, t1.Value) : float.MaxValue;
		float t2Distance = t2.HasValue ? Vector3.DistanceSquared(rayStartPosition, t2.Value) : float.MaxValue;
		return Math.Min(t1Distance, t2Distance);
	}

	public static float? RaycastEntityModel(Matrix4x4 modelMatrix, Model? mesh, Vector3 rayStartPosition, Vector3 rayDirection)
	{
		if (mesh == null)
			return null;

		Vector3? closestIntersection = null;
		if (!RaycastModel(modelMatrix, mesh, rayStartPosition, rayDirection, ref closestIntersection))
			return null;

		return Vector3.Distance(rayStartPosition, closestIntersection.Value);
	}

	public static bool RaycastModel(Matrix4x4 modelMatrix, Model model, Vector3 rayStartPosition, Vector3 rayDirection, [NotNullWhen(true)] ref Vector3? closestIntersection)
	{
		for (int i = 0; i < model.Meshes.Count; i++)
		{
			Mesh mesh = model.Meshes[i];
			if (RaycastMesh(modelMatrix, mesh, rayStartPosition, rayDirection, ref closestIntersection))
				return true;
		}

		return false;
	}

	public static bool RaycastMesh(Matrix4x4 modelMatrix, Mesh mesh, Vector3 rayStartPosition, Vector3 rayDirection, [NotNullWhen(true)] ref Vector3? closestIntersection)
	{
		for (int i = 0; i < mesh.Geometry.Indices.Length; i += 3)
		{
			Vector3 p1 = Vector3.Transform(mesh.Geometry.Vertices[mesh.Geometry.Indices[i + 0]].Position, modelMatrix);
			Vector3 p2 = Vector3.Transform(mesh.Geometry.Vertices[mesh.Geometry.Indices[i + 1]].Position, modelMatrix);
			Vector3 p3 = Vector3.Transform(mesh.Geometry.Vertices[mesh.Geometry.Indices[i + 2]].Position, modelMatrix);

			Vector3? triangleIntersection = Ray.IntersectsTriangle(rayStartPosition, rayDirection, p1, p2, p3);
			if (triangleIntersection == null)
				continue;

			if (closestIntersection == null || Vector3.DistanceSquared(Camera3d.Position, triangleIntersection.Value) < Vector3.DistanceSquared(Camera3d.Position, closestIntersection.Value))
			{
				closestIntersection = triangleIntersection.Value;
				return true;
			}
		}

		return false;
	}
}
