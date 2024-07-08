using Detach.Collisions;
using Detach.Collisions.Primitives;
using ImGuiNET;
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

		// TODO: Raycast with triangle might be incorrect? Add unit tests to Detach.Collisions.
		bool raycastT1 = Geometry3D.Raycast(new Triangle(p1, p3, p2), ray, out RaycastResult raycastResultT1);
		bool raycastT2 = Geometry3D.Raycast(new Triangle(p1, p4, p3), ray, out RaycastResult raycastResultT2);

		if (raycastT1 || raycastT2)
			ImGui.SetTooltip($"{ray.Origin}\n{ray.Direction}\n\n{p1}\n{p2}\n{p3}\n{p4}\n\n{raycastT1}\n{raycastT2}\n{raycastResultT1}\n{raycastResultT2}");

		return (raycastT1, raycastT2) switch
		{
			(true, true) => Math.Min(raycastResultT1.Distance, raycastResultT2.Distance),
			(true, false) => raycastResultT1.Distance,
			(false, true) => raycastResultT2.Distance,
			_ => null,
		};
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
