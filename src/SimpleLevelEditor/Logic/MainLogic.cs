using Detach.Collisions;
using Silk.NET.GLFW;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Model.EntityTypes;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Ui.ChildWindows;
using Sphere = SimpleLevelEditor.Model.EntityTypes.Sphere;

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

		switch (LevelEditorState.Mode)
		{
			case LevelEditorState.EditMode.WorldObjects:
				if (isFocused && Input.IsButtonPressed(MouseButton.Left) && LevelEditorState.HighlightedObject != null)
					LevelEditorState.SetSelectedWorldObject(LevelEditorState.SelectedWorldObject == LevelEditorState.HighlightedObject ? null : LevelEditorState.HighlightedObject);

				break;
			case LevelEditorState.EditMode.Entities:
				if (isFocused && Input.IsButtonPressed(MouseButton.Left) && LevelEditorState.HighlightedEntity != null)
					LevelEditorState.SetSelectedEntity(LevelEditorState.SelectedEntity == LevelEditorState.HighlightedEntity ? null : LevelEditorState.HighlightedEntity);

				break;
		}
	}

	public static void Focus()
	{
		switch (LevelEditorState.Mode)
		{
			case LevelEditorState.EditMode.WorldObjects:
				if (LevelEditorState.SelectedWorldObject != null)
					Camera3d.SetFocusPoint(LevelEditorState.SelectedWorldObject.Position);

				break;
			case LevelEditorState.EditMode.Entities:
				if (LevelEditorState.SelectedEntity != null)
					Camera3d.SetFocusPoint(LevelEditorState.SelectedEntity.GetPosition());

				break;
		}
	}

	public static void AddNew()
	{
		switch (LevelEditorState.Mode)
		{
			case LevelEditorState.EditMode.WorldObjects: AddNewWorldObject(); break;
			case LevelEditorState.EditMode.Entities: AddNewEntity(); break;
		}
	}

	private static void AddNewWorldObject()
	{
		if (!LevelEditorState.TargetPosition.HasValue)
			return;

		WorldObject referenceWorldObject = LevelEditorState.SelectedWorldObject ?? WorldObjectEditorWindow.DefaultObject;
		if (referenceWorldObject.Mesh.Length == 0 || referenceWorldObject.Texture.Length == 0)
			return;

		WorldObject worldObject = referenceWorldObject with
		{
			Id = LevelState.Level.WorldObjects.Count > 0 ? LevelState.Level.WorldObjects.Max(o => o.Id) + 1 : 0,
			Position = LevelEditorState.TargetPosition.Value,
		};
		LevelState.Level.WorldObjects.Add(worldObject);

		LevelEditorState.SetSelectedWorldObject(worldObject);
		LevelEditorState.HighlightedObject = worldObject;
		LevelState.Track("Added object");
	}

	private static void AddNewEntity()
	{
		if (!LevelEditorState.TargetPosition.HasValue)
			return;

		Entity referenceEntity = LevelEditorState.SelectedEntity ?? EntityEditorWindow.DefaultEntity;

		Entity entity = referenceEntity with
		{
			Id = LevelState.Level.WorldObjects.Count > 0 ? LevelState.Level.WorldObjects.Max(o => o.Id) + 1 : 0,
			Shape = referenceEntity.Shape.Value switch
			{
				Point => new Point(LevelEditorState.TargetPosition.Value),
				Sphere s => new Sphere(LevelEditorState.TargetPosition.Value, s.Radius),
				Aabb a => a.GetCentered(LevelEditorState.TargetPosition.Value),
				StandingCylinder c => new StandingCylinder(LevelEditorState.TargetPosition.Value, c.Radius, c.Height),
				_ => throw new NotImplementedException(),
			},
		};
		LevelState.Level.Entities.Add(entity);

		LevelEditorState.SetSelectedEntity(entity);
		LevelEditorState.HighlightedEntity = entity;
		LevelState.Track("Added entity");
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
		LevelEditorState.HighlightedObject = null;
		LevelEditorState.HighlightedEntity = null;

		if (!isFocused)
			return;

		if (Input.IsButtonHeld(Camera3d.LookButton))
			return;

		switch (LevelEditorState.Mode)
		{
			case LevelEditorState.EditMode.WorldObjects:
				CalculateHighlightedWorldObject(rayStartPosition, rayDirection);
				break;
			case LevelEditorState.EditMode.Entities:
				CalculateHighlightedEntity(rayStartPosition, rayDirection);
				break;
		}
	}

	private static void CalculateHighlightedWorldObject(Vector3 rayStartPosition, Vector3 rayDirection)
	{
		Vector3? closestIntersection = null;

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

	private static void CalculateHighlightedEntity(Vector3 rayStartPosition, Vector3 rayDirection)
	{
		float? closestDistance = null;

		for (int i = 0; i < LevelState.Level.Entities.Count; i++)
		{
			Entity entity = LevelState.Level.Entities[i];

			float? intersection = entity.Shape.Value switch
			{
				Point point => IntersectsSphere(point.Position, 0.1f),
				Sphere sphere => IntersectsSphere(sphere.Position, sphere.Radius),
				Aabb aabb => Ray.IntersectsAxisAlignedBoundingBox(rayStartPosition, rayDirection, aabb.Min, aabb.Max)?.Distance,
				_ => throw new NotImplementedException(),
			};

			if (closestDistance == null || intersection < closestDistance)
			{
				closestDistance = intersection;
				LevelEditorState.HighlightedEntity = entity;
			}
		}

		float? IntersectsSphere(Vector3 position, float radius)
		{
			Vector3? intersection = Ray.IntersectsSphere(rayStartPosition, rayDirection, position, radius);
			if (!intersection.HasValue)
				return null;

			return (rayStartPosition - intersection.Value).Length();
		}
	}
}
