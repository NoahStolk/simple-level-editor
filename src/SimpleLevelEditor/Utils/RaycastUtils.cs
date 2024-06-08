using Detach.Collisions;
using Detach.Collisions.Primitives;
using SimpleLevelEditor.Rendering;
using System.Diagnostics.CodeAnalysis;

namespace SimpleLevelEditor.Utils;

public static class RaycastUtils
{
	public static float? RaycastPlane(Matrix4x4 modelMatrix, Ray ray)
	{
		Vector3 p1 = Vector3.Transform(new Vector3(-1, -1, 0), modelMatrix);
		Vector3 p2 = Vector3.Transform(new Vector3(+1, -1, 0), modelMatrix);
		Vector3 p3 = Vector3.Transform(new Vector3(+1, +1, 0), modelMatrix);
		Vector3 p4 = Vector3.Transform(new Vector3(-1, +1, 0), modelMatrix);

		float? t1 = Geometry3D.Raycast(new Triangle(p1, p2, p3), ray, out RaycastResult raycastResultT1) ? raycastResultT1.Distance : null;
		float? t2 = Geometry3D.Raycast(new Triangle(p1, p3, p4), ray, out RaycastResult raycastResultT2) ? raycastResultT2.Distance : null;
		if (t1 == null && t2 == null)
			return null;

		return Math.Min(t1 ?? float.MaxValue, t2 ?? float.MaxValue);
	}

	public static float? RaycastEntityModel(Matrix4x4 modelMatrix, Model? model, Ray ray)
	{
		if (model == null)
			return null;

		Vector3? closestIntersection = null;
		if (!RaycastModel(modelMatrix, model, ray, ref closestIntersection))
			return null;

		return Vector3.Distance(ray.Origin, closestIntersection.Value);
	}

	private static bool RaycastModel(Matrix4x4 modelMatrix, Model model, Ray ray, [NotNullWhen(true)] ref Vector3? closestIntersection)
	{
		for (int i = 0; i < model.Meshes.Count; i++)
		{
			Mesh mesh = model.Meshes[i];
			if (RaycastMesh(modelMatrix, mesh, ray, ref closestIntersection))
				return true;
		}

		return false;
	}

	public static bool RaycastMesh(Matrix4x4 modelMatrix, Mesh mesh, Ray ray, [NotNullWhen(true)] ref Vector3? closestIntersection)
	{
		for (int i = 0; i < mesh.Geometry.Indices.Length; i += 3)
		{
			Vector3 p1 = Vector3.Transform(mesh.Geometry.Vertices[mesh.Geometry.Indices[i + 0]].Position, modelMatrix);
			Vector3 p2 = Vector3.Transform(mesh.Geometry.Vertices[mesh.Geometry.Indices[i + 1]].Position, modelMatrix);
			Vector3 p3 = Vector3.Transform(mesh.Geometry.Vertices[mesh.Geometry.Indices[i + 2]].Position, modelMatrix);

			Vector3? triangleIntersection = Geometry3D.Raycast(new Triangle(p1, p2, p3), ray, out RaycastResult raycastResult) ? raycastResult.Point : null;
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
