// using Detach.Collisions;
// using Detach.Collisions.Primitives3D;
// using System.Diagnostics.CodeAnalysis;
//
// namespace SimpleLevelEditorV2.Utils;
//
// public static class RaycastUtils
// {
// 	public static float? RaycastPlane(Matrix4x4 modelMatrix, Ray ray)
// 	{
// 		Vector3 p1 = Vector3.Transform(new Vector3(-1, -1, 0), modelMatrix);
// 		Vector3 p2 = Vector3.Transform(new Vector3(+1, -1, 0), modelMatrix);
// 		Vector3 p3 = Vector3.Transform(new Vector3(+1, +1, 0), modelMatrix);
// 		Vector3 p4 = Vector3.Transform(new Vector3(-1, +1, 0), modelMatrix);
//
// 		bool raycastT1 = Geometry3D.Raycast(new Triangle3D(p1, p3, p2), ray, out float distanceT1);
// 		bool raycastT2 = Geometry3D.Raycast(new Triangle3D(p1, p4, p3), ray, out float distanceT2);
// 		return (raycastT1, raycastT2) switch
// 		{
// 			(true, true) => Math.Min(distanceT1, distanceT2),
// 			(true, false) => distanceT1,
// 			(false, true) => distanceT2,
// 			_ => null,
// 		};
// 	}
//
// 	public static float? RaycastEntityModel(Matrix4x4 modelMatrix, Model? model, Ray ray)
// 	{
// 		if (model == null)
// 			return null;
//
// 		float? closestDistance = null;
// 		if (!RaycastModel(modelMatrix, model, ray, ref closestDistance))
// 			return null;
//
// 		return closestDistance.Value;
// 	}
//
// 	private static bool RaycastModel(Matrix4x4 modelMatrix, Model model, Ray ray, [NotNullWhen(true)] ref float? closestIntersection)
// 	{
// 		for (int i = 0; i < model.Meshes.Count; i++)
// 		{
// 			Mesh mesh = model.Meshes[i];
// 			if (RaycastMesh(modelMatrix, mesh, ray, ref closestIntersection))
// 				return true;
// 		}
//
// 		return false;
// 	}
//
// 	public static bool RaycastMesh(Matrix4x4 modelMatrix, Mesh mesh, Ray ray, [NotNullWhen(true)] ref float? closestIntersection)
// 	{
// 		for (int i = 0; i < mesh.Geometry.Indices.Length; i += 3)
// 		{
// 			Vector3 p1 = Vector3.Transform(mesh.Geometry.Vertices[mesh.Geometry.Indices[i + 0]].Position, modelMatrix);
// 			Vector3 p2 = Vector3.Transform(mesh.Geometry.Vertices[mesh.Geometry.Indices[i + 1]].Position, modelMatrix);
// 			Vector3 p3 = Vector3.Transform(mesh.Geometry.Vertices[mesh.Geometry.Indices[i + 2]].Position, modelMatrix);
//
// 			if (!Geometry3D.Raycast(new Triangle3D(p1, p2, p3), ray, out float intersectionDistance))
// 				continue;
//
// 			if (closestIntersection == null || intersectionDistance < closestIntersection.Value)
// 			{
// 				closestIntersection = intersectionDistance;
// 				return true;
// 			}
// 		}
//
// 		return false;
// 	}
// }
