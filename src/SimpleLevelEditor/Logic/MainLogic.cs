using Silk.NET.GLFW;
using SimpleLevelEditor.Maths;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.State;

namespace SimpleLevelEditor.Logic;

public static class MainLogic
{
	public static void Run(bool isFocused, Vector2 normalizedMousePosition, Plane nearPlane, float gridSnap)
	{
		CalculateTargetPosition(normalizedMousePosition, nearPlane, gridSnap);
		CalculateHighlightedObject(normalizedMousePosition, isFocused);

		if (LevelEditorState.TargetPosition.HasValue)
			ObjectCreatorState.NewWorldObject.Position = LevelEditorState.TargetPosition.Value;

		if (isFocused && Input.IsButtonPressed(MouseButton.Left))
			OnLeftClick();
	}

	private static void OnLeftClick()
	{
		switch (LevelEditorState.Mode)
		{
			case LevelEditorMode.AddWorldObjects:
				if (ObjectCreatorState.IsValid() && LevelEditorState.TargetPosition.HasValue && !LevelState.Level.WorldObjects.Exists(wo => wo.Position == ObjectCreatorState.NewWorldObject.Position && wo.Mesh == ObjectCreatorState.NewWorldObject.Mesh))
				{
					WorldObject worldObject = ObjectCreatorState.NewWorldObject.DeepCopy();
					LevelState.Level.WorldObjects.Add(worldObject);
				}

				break;
			case LevelEditorMode.EditWorldObjects:
				if (LevelEditorState.HighlightedObject != null)
				{
					ObjectEditorState.SelectedWorldObject = ObjectEditorState.SelectedWorldObject == LevelEditorState.HighlightedObject ? null : LevelEditorState.HighlightedObject;
				}

				break;
		}
	}

	private static void CalculateTargetPosition(Vector2 normalizedMousePosition, Plane nearPlane, float gridSnap)
	{
		Vector3 targetPosition = Camera3d.GetMouseWorldPosition(normalizedMousePosition, new(Vector3.UnitY, -LevelEditorState.TargetHeight));
		LevelEditorState.TargetPosition = Vector3.Dot(targetPosition, nearPlane.Normal) + nearPlane.D >= 0 ? null : new Vector3
		{
			X = gridSnap > 0 ? MathF.Round(targetPosition.X / gridSnap) * gridSnap : targetPosition.X,
			Y = targetPosition.Y,
			Z = gridSnap > 0 ? MathF.Round(targetPosition.Z / gridSnap) * gridSnap : targetPosition.Z,
		};
	}

	private static void CalculateHighlightedObject(Vector2 normalizedMousePosition, bool isFocused)
	{
		Matrix4x4 viewProjection = Camera3d.ViewMatrix * Camera3d.Projection;
		Plane farPlane = new(viewProjection.M13 - viewProjection.M14, viewProjection.M23 - viewProjection.M24, viewProjection.M33 - viewProjection.M34, viewProjection.M43 - viewProjection.M44);
		Vector3 rayEndPosition = Camera3d.GetMouseWorldPosition(normalizedMousePosition, farPlane);
		Ray ray = new(Camera3d.Position, Vector3.Normalize(rayEndPosition - Camera3d.Position));
		Vector3? closestIntersection = null;
		LevelEditorState.HighlightedObject = null;

		if (!isFocused)
			return;

		if (Input.IsButtonHeld(Camera3d.LookButton))
			return;

		for (int i = 0; i < LevelState.Level.WorldObjects.Count; i++)
		{
			WorldObject worldObject = LevelState.Level.WorldObjects[i];
			MeshContainer.Entry? mesh = MeshContainer.GetMesh(worldObject.Mesh);
			if (mesh == null)
				continue;

			Vector3 bbScale = worldObject.Scale * (mesh.BoundingMax - mesh.BoundingMin);
			Vector3 bbOffset = (mesh.BoundingMax + mesh.BoundingMin) / 2;
			float maxScale = Math.Max(bbScale.X, Math.Max(bbScale.Y, bbScale.Z));
			Sphere sphere = new(worldObject.Position + bbOffset, maxScale);
			Vector3? sphereIntersection = ray.Intersects(sphere);
			if (sphereIntersection == null)
				continue;

			Matrix4x4 modelMatrix = worldObject.GetModelMatrix();
			for (int j = 0; j < mesh.Mesh.Indices.Length; j += 3)
			{
				Vector3 p1 = Vector3.Transform(mesh.Mesh.Vertices[mesh.Mesh.Indices[j]].Position, modelMatrix);
				Vector3 p2 = Vector3.Transform(mesh.Mesh.Vertices[mesh.Mesh.Indices[j + 1]].Position, modelMatrix);
				Vector3 p3 = Vector3.Transform(mesh.Mesh.Vertices[mesh.Mesh.Indices[j + 2]].Position, modelMatrix);

				Vector3? triangleIntersection = ray.Intersects(p1, p2, p3);
				if (triangleIntersection == null)
					continue;

				if (closestIntersection == null || Vector3.DistanceSquared(Camera3d.Position, triangleIntersection.Value) < Vector3.DistanceSquared(Camera3d.Position, closestIntersection.Value))
				{
					closestIntersection = triangleIntersection.Value;
					LevelEditorState.HighlightedObject = worldObject;
				}
			}
		}
	}
}
