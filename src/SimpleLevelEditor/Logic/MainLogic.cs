using Detach.Collisions;
using Silk.NET.GLFW;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Formats.Types.EntityConfig;
using SimpleLevelEditor.Formats.Types.Level;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Ui.ChildWindows;
using SimpleLevelEditor.Utils;
using System.Diagnostics;

namespace SimpleLevelEditor.Logic;

public static class MainLogic
{
	public static void Run(bool isFocused, Vector2 normalizedMousePosition, Plane nearPlane, float gridSnap)
	{
		AssetLoadScheduleState.LoadIfScheduled();

		CalculateTargetPosition(normalizedMousePosition, nearPlane, gridSnap);
		CalculateHighlightedObject(normalizedMousePosition, isFocused);

		if (Input.GlfwInput.IsKeyDown(Keys.ControlLeft) || Input.GlfwInput.IsKeyDown(Keys.ControlRight))
		{
			float scroll = Input.GlfwInput.MouseWheelY;
			if (!scroll.IsZero())
				LevelEditorState.TargetHeight = Math.Clamp(LevelEditorState.TargetHeight - scroll, -512, 512);
		}

		if (isFocused && Input.GlfwInput.IsMouseButtonPressed(MouseButton.Left))
		{
			// ReSharper disable PossibleUnintendedReferenceComparison
			if (LevelEditorState.HighlightedObject != null)
			{
				LevelEditorState.SetSelectedWorldObject(LevelEditorState.SelectedWorldObject == LevelEditorState.HighlightedObject ? null : LevelEditorState.HighlightedObject);
			}
			else if (LevelEditorState.HighlightedEntity != null)
			{
				LevelEditorState.SetSelectedEntity(LevelEditorState.SelectedEntity == LevelEditorState.HighlightedEntity ? null : LevelEditorState.HighlightedEntity);
			}
			else
			{
				LevelEditorState.ClearSelectedEntity();
				LevelEditorState.ClearSelectedWorldObject();
			}

			// ReSharper restore PossibleUnintendedReferenceComparison
		}
	}

	public static void Focus()
	{
		if (LevelEditorState.SelectedWorldObject != null)
			Camera3d.SetFocusPoint(LevelEditorState.SelectedWorldObject.Position);
		else if (LevelEditorState.SelectedEntity != null)
			Camera3d.SetFocusPoint(LevelEditorState.SelectedEntity.Position);
	}

	public static void AddNew()
	{
		switch (LevelEditorState.Mode)
		{
			case LevelEditorState.EditMode.WorldObjects: AddNewWorldObject(); break;
			case LevelEditorState.EditMode.Entities: AddNewEntity(); break;
			default: throw new UnreachableException($"Unknown edit mode: {LevelEditorState.Mode}");
		}
	}

	private static void AddNewWorldObject()
	{
		if (!LevelEditorState.TargetPosition.HasValue)
			return;

		WorldObject referenceWorldObject = LevelEditorState.SelectedWorldObject ?? WorldObjectEditorWindow.DefaultObject;
		if (referenceWorldObject.Mesh.Length == 0 || referenceWorldObject.Texture.Length == 0)
			return;

		WorldObject worldObject = referenceWorldObject.CloneAndPlaceAtPosition(LevelState.Level.WorldObjects.Length > 0 ? LevelState.Level.WorldObjects.Max(o => o.Id) + 1 : 0, LevelEditorState.TargetPosition.Value);
		LevelState.Level.AddWorldObject(worldObject);

		LevelEditorState.SetSelectedWorldObject(worldObject);
		LevelEditorState.SetHighlightedWorldObject(worldObject);
		LevelState.Track("Added object");
	}

	private static void AddNewEntity()
	{
		if (!LevelEditorState.TargetPosition.HasValue)
			return;

		Entity referenceEntity = LevelEditorState.SelectedEntity ?? EntityEditorWindow.DefaultEntity;

		Entity entity = referenceEntity.CloneAndPlaceAtPosition(LevelState.Level.Entities.Length > 0 ? LevelState.Level.Entities.Max(o => o.Id) + 1 : 0, LevelEditorState.TargetPosition.Value);
		LevelState.Level.AddEntity(entity);

		LevelEditorState.SetSelectedEntity(entity);
		LevelEditorState.SetHighlightedEntity(entity);
		LevelState.Track("Added entity");
	}

	public static void Remove()
	{
		if (LevelEditorState.SelectedWorldObject != null)
		{
			LevelState.Level.RemoveWorldObject(LevelEditorState.SelectedWorldObject);
			LevelEditorState.SetSelectedWorldObject(null);
			LevelState.Track("Deleted world object");
		}
		else if (LevelEditorState.SelectedEntity != null)
		{
			LevelState.Level.RemoveEntity(LevelEditorState.SelectedEntity);
			LevelEditorState.SetSelectedEntity(null);
			LevelState.Track("Deleted entity");
		}
	}

	private static void CalculateTargetPosition(Vector2 normalizedMousePosition, Plane nearPlane, float gridSnap)
	{
		Vector3 targetPosition = Camera3d.GetMouseWorldPosition(normalizedMousePosition, new Plane(Vector3.UnitY, -LevelEditorState.TargetHeight));
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
		LevelEditorState.ClearHighlight();

		if (!isFocused)
			return;

		if (Input.GlfwInput.IsMouseButtonDown(Camera3d.LookButton))
			return;

		Vector3? closestIntersection = null;

		if (LevelEditorState.ShouldRenderWorldObjects())
			RaycastWorldObjects(rayStartPosition, rayDirection, ref closestIntersection);

		float? closestDistance = closestIntersection.HasValue ? Vector3.Distance(Camera3d.Position, closestIntersection.Value) : null;

		RaycastEntities(rayStartPosition, rayDirection, closestDistance);
	}

	private static void RaycastWorldObjects(Vector3 rayStartPosition, Vector3 rayDirection, ref Vector3? closestIntersection)
	{
		for (int i = 0; i < LevelState.Level.WorldObjects.Length; i++)
		{
			WorldObject worldObject = LevelState.Level.WorldObjects[i];
			MeshEntry? mesh = MeshContainer.GetMesh(worldObject.Mesh);
			if (mesh == null)
				continue;

			Vector3 bbScale = worldObject.Scale * (mesh.BoundingMax - mesh.BoundingMin);
			Vector3 bbOffset = (mesh.BoundingMax + mesh.BoundingMin) / 2;
			float maxScale = Math.Max(bbScale.X, Math.Max(bbScale.Y, bbScale.Z));
			Vector3? sphereIntersection = Ray.IntersectsSphere(rayStartPosition, rayDirection, worldObject.Position + bbOffset, maxScale);
			if (sphereIntersection == null)
				continue;

			Matrix4x4 modelMatrix = worldObject.GetModelMatrix();
			if (RaycastUtils.RaycastMesh(modelMatrix, mesh.Mesh, rayStartPosition, rayDirection, ref closestIntersection))
			{
				LevelEditorState.SetHighlightedWorldObject(worldObject);
			}
		}
	}

	private static void RaycastEntities(Vector3 rayStartPosition, Vector3 rayDirection, float? closestDistance)
	{
		for (int i = 0; i < LevelState.Level.Entities.Length; i++)
		{
			Entity entity = LevelState.Level.Entities[i];
			if (!LevelEditorState.ShouldRenderEntity(entity))
				continue;

			float? intersection = GetIntersection(entity);
			if (!intersection.HasValue)
				continue;

			if (closestDistance == null || intersection < closestDistance)
			{
				closestDistance = intersection;
				LevelEditorState.SetHighlightedEntity(entity);
			}
		}

		float? IntersectsSphere(Vector3 position, float radius)
		{
			Vector3? intersection = Ray.IntersectsSphere(rayStartPosition, rayDirection, position, radius);
			if (!intersection.HasValue)
				return null;

			return (rayStartPosition - intersection.Value).Length();
		}

		float? GetIntersection(Entity entity)
		{
			if (entity.Shape.IsPoint)
			{
				EntityShape? entityShape = EntityConfigState.EntityConfig.Entities.FirstOrDefault(e => e.Name == entity.Name)?.Shape;
				if (entityShape is not EntityShape.Point point)
					return null;

				return point.Visualization switch
				{
					PointEntityVisualization.SimpleSphere simpleSphere => IntersectsSphere(entity.Position, simpleSphere.Radius),
					PointEntityVisualization.BillboardSprite billboardSprite => RaycastUtils.RaycastPlane(Matrix4x4.CreateScale(billboardSprite.Size * 0.5f) * EntityMatrixUtils.GetBillboardMatrix(entity.Position), rayStartPosition, rayDirection),
					PointEntityVisualization.Mesh mesh => RaycastUtils.RaycastEntityMesh(Matrix4x4.CreateScale(mesh.Size * 2) * Matrix4x4.CreateTranslation(entity.Position), MeshContainer.GetMesh(mesh.MeshName)?.Mesh, rayStartPosition, rayDirection),
					_ => throw new InvalidOperationException($"Unknown point entity visualization: {point.Visualization}"),
				};
			}

			return entity.Shape switch
			{
				ShapeDescriptor.Sphere sphere => IntersectsSphere(entity.Position, sphere.Radius),
				ShapeDescriptor.Aabb aabb => Ray.IntersectsAxisAlignedBoundingBox(rayStartPosition, rayDirection, entity.Position - aabb.Size / 2f, entity.Position + aabb.Size / 2f)?.Distance,
				_ => throw new UnreachableException($"Unknown entity shape: {entity.Shape}"),
			};
		}
	}
}
