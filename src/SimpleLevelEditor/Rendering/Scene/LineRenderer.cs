using Detach.Utils;
using Silk.NET.OpenGL;
using SimpleLevelEditor.Content;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Formats.Types;
using SimpleLevelEditor.Formats.Types.EntityConfig;
using SimpleLevelEditor.Formats.Types.Level;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;
using static SimpleLevelEditor.Graphics;

namespace SimpleLevelEditor.Rendering.Scene;

public sealed class LineRenderer
{
	private static readonly uint _lineVao = VaoUtils.CreateLineVao([Vector3.Zero, Vector3.UnitZ]);
	private static readonly uint _centeredLineVao = VaoUtils.CreateLineVao([-Vector3.UnitZ, Vector3.UnitZ]);

	private static readonly uint _cubeVao = VaoUtils.CreateLineVao([
		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),

		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),

		new Vector3(-0.5f, -0.5f, -0.5f),
		new Vector3(0.5f, -0.5f, -0.5f),
		new Vector3(-0.5f, -0.5f, 0.5f),
		new Vector3(0.5f, -0.5f, 0.5f),
		new Vector3(-0.5f, 0.5f, -0.5f),
		new Vector3(0.5f, 0.5f, -0.5f),
		new Vector3(-0.5f, 0.5f, 0.5f),
		new Vector3(0.5f, 0.5f, 0.5f),
	]);

	private static readonly Vector3[] _sphereVertices = VertexUtils.GetSphereVertexPositions(8, 16, 1);
	private static readonly uint _sphereVao = VaoUtils.CreateLineVao(_sphereVertices);

	private static readonly Vector3[] _pointVertices = VertexUtils.GetSphereVertexPositions(3, 6, 1);
	private static readonly uint _pointVao = VaoUtils.CreateLineVao(_pointVertices);

	private static readonly uint _planeLineVao = VaoUtils.CreateLineVao([
		new Vector3(-0.5f, -0.5f, 0),
		new Vector3(-0.5f, 0.5f, 0),
		new Vector3(0.5f, 0.5f, 0),
		new Vector3(0.5f, -0.5f, 0),
		new Vector3(-0.5f, -0.5f, 0),
	]);
	private static readonly uint[] _planeLineIndices = [0, 1, 1, 2, 2, 3, 3, 0];

	private readonly ShaderCacheEntry _lineShader;
	private readonly int _modelUniform;
	private readonly int _colorUniform;

	public LineRenderer()
	{
		_lineShader = InternalContent.Shaders["Line"];
		_modelUniform = _lineShader.GetUniformLocation("model");
		_colorUniform = _lineShader.GetUniformLocation("color");
	}

	public void Render()
	{
		Gl.UseProgram(_lineShader.Id);

		Gl.UniformMatrix4x4(_lineShader.GetUniformLocation("view"), Camera3d.ViewMatrix);
		Gl.UniformMatrix4x4(_lineShader.GetUniformLocation("projection"), Camera3d.Projection);

		Gl.BindVertexArray(_lineVao);
		RenderOrigin();
		RenderGrid(Camera3d.FocusPointTarget with { Y = LevelEditorState.TargetHeight }, new Vector4(1, 1, 1, 0.25f), LevelEditorState.GridCellCount, LevelEditorState.GridCellSize);
		if (LevelEditorState.MoveTargetPosition.HasValue)
		{
			Vector3? selectedPosition = LevelEditorState.SelectedWorldObject?.Position ?? LevelEditorState.SelectedEntity?.Position;
			if (selectedPosition != null)
			{
				bool movedHorizontally = MathF.Abs(LevelEditorState.MoveTargetPosition.Value.X - selectedPosition.Value.X) > float.Epsilon || MathF.Abs(LevelEditorState.MoveTargetPosition.Value.Z - selectedPosition.Value.Z) > float.Epsilon;
				if (movedHorizontally && MathF.Abs(LevelEditorState.TargetHeight - selectedPosition.Value.Y) > float.Epsilon)
					RenderGrid(Camera3d.FocusPointTarget with { Y = selectedPosition.Value.Y }, new Vector4(1, 1, 0, 0.25f), LevelEditorState.GridCellCount, LevelEditorState.GridCellSize);
			}
		}

		RenderWorldObjectEdges();
		RenderEntities();

		Gl.BindVertexArray(_centeredLineVao);
		RenderFocusAxes();
		RenderMoveComparison();
	}

	private void RenderLine(Matrix4x4 modelMatrix, Vector4 color)
	{
		Gl.UniformMatrix4x4(_modelUniform, modelMatrix);
		Gl.Uniform4(_colorUniform, color);
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);
	}

	private void RenderOrigin()
	{
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(1, 1, 256);

		Gl.LineWidth(4);
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2), new Vector4(1, 0, 0, 1));
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 1.5f), new Vector4(0, 1, 0, 1));
		RenderLine(scaleMatrix, new Vector4(0, 0, 1, 1));

		Gl.LineWidth(2);
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, -MathF.PI / 2), new Vector4(1, 0, 0, 0.5f));
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2), new Vector4(0, 1, 0, 0.5f));
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI), new Vector4(0, 0, 1, 0.5f));
	}

	private void RenderFocusAxes()
	{
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(1, 1, 128);
		Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(Camera3d.FocusPointTarget);

		Gl.LineWidth(1);
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2) * translationMatrix, new Vector4(1, 0.5f, 0, 0.5f));
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 1.5f) * translationMatrix, new Vector4(0, 1, 0.5f, 0.5f));
		RenderLine(scaleMatrix * translationMatrix, new Vector4(0.5f, 0, 1, 0.5f));
	}

	private void RenderMoveComparison()
	{
		if (!LevelEditorState.MoveTargetPosition.HasValue)
			return;

		Vector3? selectedObjectPosition = LevelEditorState.SelectedWorldObject?.Position ?? LevelEditorState.SelectedEntity?.Position;
		if (selectedObjectPosition == null)
			return;

		Vector3 moveTarget = LevelEditorState.MoveTargetPosition.Value;

		Gl.LineWidth(6);
		Vector4 color = new(1, 1, 0, 1);

		float halfDistanceX = MathF.Abs(selectedObjectPosition.Value.X - moveTarget.X) / 2;
		float halfDistanceY = MathF.Abs(selectedObjectPosition.Value.Y - moveTarget.Y) / 2;
		float halfDistanceZ = MathF.Abs(selectedObjectPosition.Value.Z - moveTarget.Z) / 2;

		Vector3 averageOnX = selectedObjectPosition.Value with { X = (selectedObjectPosition.Value.X + moveTarget.X) / 2 };
		Vector3 averageOnY = selectedObjectPosition.Value with { Y = (selectedObjectPosition.Value.Y + moveTarget.Y) / 2 };
		Vector3 averageOnZ = selectedObjectPosition.Value with { Z = (selectedObjectPosition.Value.Z + moveTarget.Z) / 2 };

		RenderLine(Matrix4x4.CreateScale(1, 1, halfDistanceX) * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2) * Matrix4x4.CreateTranslation(averageOnX with { Z = moveTarget.Z }), color);
		RenderLine(Matrix4x4.CreateScale(1, 1, halfDistanceY) * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2) * Matrix4x4.CreateTranslation(averageOnY), color);
		RenderLine(Matrix4x4.CreateScale(1, 1, halfDistanceZ) * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI) * Matrix4x4.CreateTranslation(averageOnZ), color);
	}

	private void RenderGrid(Vector3 origin, Vector4 color, int cellCount, int cellSize)
	{
		Gl.Uniform4(_colorUniform, color);
		Gl.LineWidth(1);

		int min = -cellCount;
		int max = cellCount;
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(new Vector3(1, 1, (max - min) * cellSize));
		Vector3 offset = new(MathF.Round(origin.X), 0, MathF.Round(origin.Z));
		offset.X = MathF.Round(offset.X / cellSize) * cellSize;
		offset.Z = MathF.Round(offset.Z / cellSize) * cellSize;

		for (int i = min; i <= max; i++)
		{
			// Prevent rendering grid lines on top of origin lines (Z-fighting).
			if (!origin.Y.IsZero() || !(i * cellSize + offset.X).IsZero())
			{
				Gl.UniformMatrix4x4(_modelUniform, scaleMatrix * Matrix4x4.CreateTranslation(new Vector3(i * cellSize, origin.Y, min * cellSize) + offset));
				Gl.DrawArrays(PrimitiveType.Lines, 0, 2);
			}

			if (!origin.Y.IsZero() || !(i * cellSize + offset.Z).IsZero())
			{
				Gl.UniformMatrix4x4(_modelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2) * Matrix4x4.CreateTranslation(new Vector3(min * cellSize, origin.Y, i * cellSize) + offset));
				Gl.DrawArrays(PrimitiveType.Lines, 0, 2);
			}
		}
	}

	private void RenderWorldObjectEdges()
	{
		Gl.LineWidth(2);

		for (int i = 0; i < LevelState.Level.WorldObjects.Length; i++)
		{
			WorldObject worldObject = LevelState.Level.WorldObjects[i];
			Vector4 color = GetWorldObjectLineColor(worldObject);

			Model? model = ModelContainer.LevelContainer.GetModel(worldObject.ModelPath);
			if (model == null)
				continue;

			Matrix4x4 rotationMatrix = MathUtils.CreateRotationMatrixFromEulerAngles(MathUtils.ToRadians(worldObject.Rotation));
			Matrix4x4 modelMatrix = Matrix4x4.CreateScale(worldObject.Scale) * rotationMatrix * Matrix4x4.CreateTranslation(worldObject.Position);
			RenderModelEdges(model, modelMatrix, color);
		}

		if (LevelEditorState.MoveTargetPosition.HasValue && LevelEditorState.SelectedWorldObject != null)
		{
			Model? model = ModelContainer.LevelContainer.GetModel(LevelEditorState.SelectedWorldObject.ModelPath);
			if (model != null)
				RenderModelEdges(model, LevelEditorState.SelectedWorldObject.GetModelMatrix(LevelEditorState.MoveTargetPosition.Value), GetWorldObjectLineColor(LevelEditorState.SelectedWorldObject));
		}
	}

	private void RenderModelEdges(Model model, Matrix4x4 modelMatrix, Vector4 color)
	{
		if (color.W < float.Epsilon)
			return;

		Gl.UniformMatrix4x4(_modelUniform, modelMatrix);
		Gl.Uniform4(_colorUniform, color);

		for (int i = 0; i < model.Meshes.Count; i++)
		{
			Mesh mesh = model.Meshes[i];
			RenderEdges(mesh.LineVao, mesh.LineIndices);
		}
	}

	private static unsafe void RenderEdges(uint lineVao, uint[] lineIndices)
	{
		Gl.BindVertexArray(lineVao);
		fixed (uint* index = &lineIndices[0])
			Gl.DrawElements(PrimitiveType.Lines, (uint)lineIndices.Length, DrawElementsType.UnsignedInt, index);
	}

	private void RenderEntities()
	{
		for (int i = 0; i < LevelState.Level.Entities.Length; i++)
		{
			Entity entity = LevelState.Level.Entities[i];
			if (!LevelEditorState.ShouldRenderEntity(entity))
				continue;

			RenderEntity(entity, entity.Position);
		}

		if (LevelEditorState.MoveTargetPosition.HasValue && LevelEditorState.SelectedEntity != null)
			RenderEntity(LevelEditorState.SelectedEntity, LevelEditorState.MoveTargetPosition.Value);
	}

	private void RenderEntity(Entity entity, Vector3 entityPosition)
	{
		EntityShapeDescriptor? entityShape = EntityConfigState.GetEntityShape(entity);
		if (entityShape is EntityShapeDescriptor.Point point)
		{
			if (point.Visualization is PointEntityVisualization.SimpleSphere simpleSphere)
			{
				Gl.UniformMatrix4x4(_modelUniform, Matrix4x4.CreateScale(simpleSphere.Radius) * Matrix4x4.CreateTranslation(entityPosition));
				Gl.Uniform4(_colorUniform, GetEntityLineColor(entity, simpleSphere.Color, simpleSphere.Color.ToVector4()));
				Gl.BindVertexArray(_pointVao);
				Gl.DrawArrays(PrimitiveType.Lines, 0, (uint)_pointVertices.Length);
			}
			else if (point.Visualization is PointEntityVisualization.Model mesh)
			{
				Model? model = ModelContainer.EntityConfigContainer.GetModel(mesh.ModelPath);
				if (model != null)
					RenderModelEdges(model, Matrix4x4.CreateTranslation(entityPosition), GetEntityLineColor(entity, new Rgb(191, 63, 63), Vector4.Zero));
			}
			else if (point.Visualization is PointEntityVisualization.BillboardSprite billboardSprite)
			{
				Matrix4x4 modelMatrix = Matrix4x4.CreateScale(billboardSprite.Size) * EntityMatrixUtils.GetBillboardMatrix(entityPosition);

				Gl.UniformMatrix4x4(_modelUniform, modelMatrix);
				Gl.Uniform4(_colorUniform, GetEntityLineColor(entity, new Rgb(63, 63, 191), Vector4.Zero));
				RenderEdges(_planeLineVao, _planeLineIndices);
			}
		}
		else if (entityShape is EntityShapeDescriptor.Sphere sphere)
		{
			if (entity.Shape is not EntityShape.Sphere sphereDescriptor)
				throw new InvalidOperationException($"Entity '{entity.Name}' is of shape type '{entity.Shape}' which does not match shape type '{entityShape}' from the EntityConfig.");

			Gl.UniformMatrix4x4(_modelUniform, Matrix4x4.CreateScale(sphereDescriptor.Radius) * Matrix4x4.CreateTranslation(entityPosition));
			Gl.Uniform4(_colorUniform, GetEntityLineColor(entity, sphere.Color, sphere.Color.ToVector4()));
			Gl.BindVertexArray(_sphereVao);
			Gl.DrawArrays(PrimitiveType.Lines, 0, (uint)_sphereVertices.Length);
		}
		else if (entityShape is EntityShapeDescriptor.Aabb aabb)
		{
			if (entity.Shape is not EntityShape.Aabb aabbDescriptor)
				throw new InvalidOperationException($"Entity '{entity.Name}' is of shape type '{entity.Shape}' which does not match shape type '{entityShape}' from the EntityConfig.");

			Gl.UniformMatrix4x4(_modelUniform, Matrix4x4.CreateScale(aabbDescriptor.Size) * Matrix4x4.CreateTranslation(entityPosition));
			Gl.Uniform4(_colorUniform, GetEntityLineColor(entity, aabb.Color, aabb.Color.ToVector4()));
			Gl.BindVertexArray(_cubeVao);
			Gl.DrawArrays(PrimitiveType.Lines, 0, 24);
		}
	}

	private static Vector4 GetEntityLineColor(Entity entity, Rgb rgb, Vector4 defaultColor)
	{
		float timeAddition = MathF.Sin((float)Glfw.GetTime() * 10) * 0.5f + 0.5f;
		timeAddition *= 0.5f;

		Vector4 color = rgb.ToVector4();

		// ReSharper disable PossibleUnintendedReferenceComparison
		if (entity == LevelEditorState.SelectedEntity && entity == LevelEditorState.HighlightedEntity && Camera3d.Mode == CameraMode.None)
			return color + new Vector4(0.5f + timeAddition, 1, 0.5f + timeAddition, 1);
		if (entity == LevelEditorState.SelectedEntity)
			return color + new Vector4(0, 0.75f, 0, 1);
		if (entity == LevelEditorState.HighlightedEntity && Camera3d.Mode == CameraMode.None)
			return color + new Vector4(1, 0.5f + timeAddition, 1, 1);

		// ReSharper restore PossibleUnintendedReferenceComparison
		return defaultColor;
	}

	private static Vector4 GetWorldObjectLineColor(WorldObject worldObject)
	{
		float timeAddition = MathF.Sin((float)Glfw.GetTime() * 10) * 0.5f + 0.5f;
		timeAddition *= 0.5f;

		// ReSharper disable PossibleUnintendedReferenceComparison
		if (worldObject == LevelEditorState.SelectedWorldObject && worldObject == LevelEditorState.HighlightedObject && Camera3d.Mode == CameraMode.None)
			return new Vector4(0.5f + timeAddition, 1, 0.5f + timeAddition, 1);
		if (worldObject == LevelEditorState.SelectedWorldObject)
			return new Vector4(0, 0.75f, 0, 1);
		if (worldObject == LevelEditorState.HighlightedObject && Camera3d.Mode == CameraMode.None)
			return new Vector4(1, 0.5f + timeAddition, 1, 1);

		// ReSharper restore PossibleUnintendedReferenceComparison
		return Vector4.Zero;
	}
}
