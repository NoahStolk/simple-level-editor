using Detach.Collisions;
using Detach.Collisions.Primitives3D;
using ImGuiGlfw;
using Silk.NET.GLFW;
using SimpleLevelEditorV2.Formats.Level.Model;
using System.Diagnostics;
using System.Numerics;

namespace SimpleLevelEditorV2.States.LevelEditor.Controllers;

public sealed class LevelEditorController
{
	public float TargetHeight { get; private set; }

	public Vector3? TargetPosition { get; private set; }

	public LevelEntity? HighlightedEntity { get; private set; }

	public LevelEntity? SelectedEntity { get; private set; }

	public void Update(GlfwInput input, bool isFocused, Vector2 normalizedMousePosition, Plane nearPlane, float gridSnap, CameraController cameraController, LevelModelState levelModelState)
	{
		CalculateTargetPosition(normalizedMousePosition, nearPlane, gridSnap, cameraController);
		CalculateHighlightedObject(input, normalizedMousePosition, isFocused, cameraController, levelModelState);

		if (input.IsKeyDown(Keys.ControlLeft) || input.IsKeyDown(Keys.ControlRight))
		{
			float scroll = input.MouseWheelY;
			if (!scroll.IsZero())
				TargetHeight = Math.Clamp(TargetHeight - scroll, -512, 512);
		}

		if (isFocused && input.IsMouseButtonPressed(MouseButton.Left))
		{
			if (HighlightedEntity != null)
				SelectedEntity = SelectedEntity == HighlightedEntity ? null : HighlightedEntity;
			else
				SelectedEntity = null;
		}
	}

	public void Focus(CameraController cameraController)
	{
		Vector3? selectedEntityPosition = SelectedEntity?.Position;
		if (selectedEntityPosition.HasValue)
			cameraController.SetFocusPoint(selectedEntityPosition.Value);
	}

	public void AddNewEntity(LevelModelState levelModelState)
	{
		if (!TargetPosition.HasValue)
			return;

		// LevelEntity referenceEntity = SelectedEntity ?? EntityEditorState.DefaultEntity;
		//
		// LevelEntity entity = referenceEntity.CloneAndPlaceAtPosition(LevelState.Level.Entities.Count > 0 ? LevelState.Level.Entities.Max(o => o.Id) + 1 : 0, LevelEditorState.TargetPosition.Value);
		// levelModelState.Level.AddEntity(entity);
		//
		// SelectedEntity = entity;
		// HighlightedEntity = entity;
		// LevelState.Track("Added entity");
	}

	public void Remove(LevelModelState levelModelState)
	{
		if (SelectedEntity == null || levelModelState.Level == null)
			return;

		// levelModelState.Level.RemoveEntity(SelectedEntity);
		// SelectedEntity = null;
		// LevelState.Track("Deleted entity");
	}

	private void CalculateTargetPosition(Vector2 normalizedMousePosition, Plane nearPlane, float gridSnap, CameraController cameraController)
	{
		Vector3 targetPosition = cameraController.GetMouseWorldPosition(normalizedMousePosition, new Plane(Vector3.UnitY, -TargetHeight));
		TargetPosition = Vector3.Dot(targetPosition, nearPlane.Normal) + nearPlane.D >= 0 ? null : new Vector3
		{
			X = gridSnap > 0 ? MathF.Round(targetPosition.X / gridSnap) * gridSnap : targetPosition.X,
			Y = targetPosition.Y,
			Z = gridSnap > 0 ? MathF.Round(targetPosition.Z / gridSnap) * gridSnap : targetPosition.Z,
		};
	}

	private void CalculateHighlightedObject(GlfwInput input, Vector2 normalizedMousePosition, bool isFocused, CameraController cameraController, LevelModelState levelModelState)
	{
		Matrix4x4 viewProjection = cameraController.ViewMatrix * cameraController.ProjectionMatrix;
		Plane farPlane = new(viewProjection.M13 - viewProjection.M14, viewProjection.M23 - viewProjection.M24, viewProjection.M33 - viewProjection.M34, viewProjection.M43 - viewProjection.M44);
		Vector3 rayStartPosition = cameraController.Position;
		Vector3 rayEndPosition = cameraController.GetMouseWorldPosition(normalizedMousePosition, farPlane);
		Vector3 rayDirection = Vector3.Normalize(rayEndPosition - rayStartPosition);
		Ray ray = new(rayStartPosition, rayDirection);
		HighlightedEntity = null;

		if (!isFocused)
			return;

		if (input.IsMouseButtonDown(CameraController.LookButton))
			return;

		float? closestDistance = null;

		RaycastEntities(ray, closestDistance, cameraController, levelModelState);
	}

	private void RaycastEntities(Ray ray, float? closestDistance, CameraController cameraController, LevelModelState levelModelState)
	{
		if (levelModelState.Level == null)
			return;

		for (int i = 0; i < levelModelState.Level.LevelEntities.Count; i++)
		{
			LevelEntity entity = levelModelState.Level.LevelEntities[i];
			// if (!LevelEditorState.ShouldRenderEntity(entity))
			// 	continue;

			float? intersection = GetIntersection(entity);
			if (!intersection.HasValue)
				continue;

			if (closestDistance == null || intersection < closestDistance)
			{
				closestDistance = intersection;
				HighlightedEntity = entity;
			}
		}

		float? GetIntersection(LevelEntity entity)
		{
			if (!entity.Position.HasValue)
				return null;

			// if (entity.IsModel)
			// 	return RaycastUtils.RaycastEntityModel(Matrix4x4.CreateScale(model.Size * 2) * Matrix4x4.CreateTranslation(entity.Position.Value), ModelContainer.EntityConfigContainer.GetModel(model.ModelPath), ray);
			//
			// if (entity.IsBillboard)
			// 	return RaycastUtils.RaycastPlane(Matrix4x4.CreateScale(billboardSprite.Size * 0.5f) * GetBillboardMatrix(entity.Position.Value), ray);

			if (entity.IsWireframe)
			{
				Vector3 scale = entity.Scale ?? Vector3.One;
				float maxScale = Math.Max(scale.X, Math.Max(scale.Y, scale.Z));

				return entity.WireframeShape switch
				{
					"Sphere" => Geometry3D.Raycast(new Sphere(entity.Position.Value, maxScale), ray, out float distance) ? distance : null,
					"Aabb" => Geometry3D.Raycast(new Aabb(entity.Position.Value, scale), ray, out float distance) ? distance : null,
					_ => throw new UnreachableException($"Unknown entity wireframe shape: {entity.WireframeShape}"),
				};
			}

			return null;

			Matrix4x4 GetBillboardMatrix(Vector3 entityPosition)
			{
				// Negate the camera up vector because rendered textures are flipped vertically.
				return Matrix4x4.CreateBillboard(entityPosition, cameraController.Position, -cameraController.UpDirection, cameraController.LookDirection);
			}
		}
	}
}
