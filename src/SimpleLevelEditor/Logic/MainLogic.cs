using Detach.Collisions;
using Silk.NET.GLFW;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Ui.ChildWindows;

namespace SimpleLevelEditor.Logic;

public static class MainLogic
{
	public static void Run(bool isFocused, Vector2 normalizedMousePosition, Plane nearPlane, float gridSnap)
	{
		LoadScheduleState.LoadIfScheduled();

		CalculateTargetPosition(normalizedMousePosition, nearPlane, gridSnap);
		CalculateHighlightedObject(normalizedMousePosition, isFocused);

		if (Input.IsKeyHeld(Keys.ControlLeft) || Input.IsKeyHeld(Keys.ControlRight))
		{
			float scroll = Input.GetScroll();
			if (scroll != 0)
				LevelEditorState.TargetHeight = Math.Clamp(LevelEditorState.TargetHeight - scroll, -512, 512);
		}

		if (isFocused && Input.IsButtonPressed(MouseButton.Left) && LevelEditorState.HighlightedObject != null)
			LevelEditorState.SetSelectedWorldObject(LevelEditorState.SelectedWorldObject == LevelEditorState.HighlightedObject ? null : LevelEditorState.HighlightedObject);
	}

	public static void AddNewWorldObject()
	{
		if (!LevelEditorState.TargetPosition.HasValue)
			return;

		WorldObject reference = LevelEditorState.SelectedWorldObject ?? WorldObjectEditorWindow.DefaultObject;
		if (reference.Mesh.Length == 0 || reference.Texture.Length == 0)
			return;

		WorldObject worldObject = reference with
		{
			Id = LevelState.Level.WorldObjects.Count > 0 ? LevelState.Level.WorldObjects.Max(o => o.Id) + 1 : 0,
			Position = LevelEditorState.TargetPosition.Value,
		};
		LevelState.Level.WorldObjects.Add(worldObject);

		LevelEditorState.SetSelectedWorldObject(worldObject);
		LevelEditorState.HighlightedObject = worldObject;
		LevelState.Track("Added object");
	}

	public static void RemoveWorldObject()
	{
		if (LevelEditorState.SelectedWorldObject == null)
			return;

		LevelState.Level.WorldObjects.Remove(LevelEditorState.SelectedWorldObject);
		LevelEditorState.SetSelectedWorldObject(null);
		LevelState.Track("Deleted world object");
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
		Vector3 rayStartPosition = Camera3d.Position;
		Vector3 rayEndPosition = Camera3d.GetMouseWorldPosition(normalizedMousePosition, farPlane);
		Vector3 rayDirection = Vector3.Normalize(rayEndPosition - rayStartPosition);
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
			Vector3? sphereIntersection = Ray.IntersectsSphere(rayStartPosition, rayDirection, worldObject.Position + bbOffset, maxScale);
			if (sphereIntersection == null)
				continue;

			Matrix4x4 modelMatrix = worldObject.GetModelMatrix();
			for (int j = 0; j < mesh.Mesh.Indices.Length; j += 3)
			{
				Vector3 p1 = Vector3.Transform(mesh.Mesh.Vertices[mesh.Mesh.Indices[j]].Position, modelMatrix);
				Vector3 p2 = Vector3.Transform(mesh.Mesh.Vertices[mesh.Mesh.Indices[j + 1]].Position, modelMatrix);
				Vector3 p3 = Vector3.Transform(mesh.Mesh.Vertices[mesh.Mesh.Indices[j + 2]].Position, modelMatrix);

				Vector3? triangleIntersection = Ray.IntersectsTriangle(rayStartPosition, rayDirection, p1, p2, p3);
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
