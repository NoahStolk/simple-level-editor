using Detach.Collisions;
using Detach.Collisions.Primitives3D;
using Silk.NET.GLFW;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Formats.Types.EntityConfig;
using SimpleLevelEditor.Formats.Types.Level;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.State;
using SimpleLevelEditor.State.Level;
using SimpleLevelEditor.State.Models;
using SimpleLevelEditor.Ui.ChildWindows;
using SimpleLevelEditor.Utils;
using System.Diagnostics;

namespace SimpleLevelEditor.Logic;

public static class MainLogic
{
	public static void Run(bool isFocused, Vector2 normalizedMousePosition, Plane nearPlane, float gridSnap)
	{
		AssetLoadScheduleState.LoadIfScheduled(Graphics.Gl);

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
		if (referenceWorldObject.ModelPath.Length == 0)
			return; // TODO: Show popup.

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
		Ray ray = new(rayStartPosition, rayDirection);
		LevelEditorState.ClearHighlight();

		if (!isFocused)
			return;

		if (Input.GlfwInput.IsMouseButtonDown(Camera3d.LookButton))
			return;

		float? closestDistance = null;

		if (LevelEditorState.ShouldRenderWorldObjects)
			RaycastWorldObjects(ray, ref closestDistance);

		RaycastEntities(ray, closestDistance);
	}

	private static void RaycastWorldObjects(Ray ray, ref float? closestIntersection)
	{
		for (int i = 0; i < LevelState.Level.WorldObjects.Length; i++)
		{
			WorldObject worldObject = LevelState.Level.WorldObjects[i];
			Model? model = ModelContainer.LevelContainer.GetModel(worldObject.ModelPath);
			if (model == null)
				continue;

			for (int j = 0; j < model.Meshes.Count; j++)
			{
				Mesh mesh = model.Meshes[j];
				Vector3 bbScale = worldObject.Scale * (mesh.BoundingMax - mesh.BoundingMin);
				Vector3 bbOffset = (mesh.BoundingMax + mesh.BoundingMin) / 2;
				float maxScale = Math.Max(bbScale.X, Math.Max(bbScale.Y, bbScale.Z));
				if (!Geometry3D.Raycast(new Sphere(worldObject.Position + bbOffset, maxScale), ray, out float _))
					continue;

				Matrix4x4 modelMatrix = worldObject.GetModelMatrix();
				if (RaycastUtils.RaycastMesh(modelMatrix, mesh, ray, ref closestIntersection))
				{
					// TODO: For clarity, consider returning the world object instead of setting it as highlighted on every iteration.
					LevelEditorState.SetHighlightedWorldObject(worldObject);
				}
			}
		}
	}

	private static void RaycastEntities(Ray ray, float? closestDistance)
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

		float? GetIntersection(Entity entity)
		{
			if (entity.Shape.IsPoint)
			{
				EntityShapeDescriptor? entityShape = EntityConfigState.EntityConfig.Entities.FirstOrDefault(e => e.Name == entity.Name)?.Shape;
				if (entityShape is not EntityShapeDescriptor.Point point)
					return null;

				return point.Visualization switch
				{
					PointEntityVisualization.SimpleSphere simpleSphere => Geometry3D.Raycast(new Sphere(entity.Position, simpleSphere.Radius), ray, out float distance) ? distance : null,
					PointEntityVisualization.BillboardSprite billboardSprite => RaycastUtils.RaycastPlane(Matrix4x4.CreateScale(billboardSprite.Size * 0.5f) * EntityMatrixUtils.GetBillboardMatrix(entity.Position), ray),
					PointEntityVisualization.Model model => RaycastUtils.RaycastEntityModel(Matrix4x4.CreateScale(model.Size * 2) * Matrix4x4.CreateTranslation(entity.Position), ModelContainer.EntityConfigContainer.GetModel(model.ModelPath), ray),
					_ => throw new InvalidOperationException($"Unknown point entity visualization: {point.Visualization}"),
				};
			}

			return entity.Shape switch
			{
				EntityShape.Sphere sphere => Geometry3D.Raycast(new Sphere(entity.Position, sphere.Radius), ray, out float distance) ? distance : null,
				EntityShape.Aabb aabb => Geometry3D.Raycast(new Aabb(entity.Position, aabb.Size), ray, out float distance) ? distance : null,
				_ => throw new UnreachableException($"Unknown entity shape: {entity.Shape}"),
			};
		}
	}
}
