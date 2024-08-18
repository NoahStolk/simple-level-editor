using Silk.NET.OpenGL;
using System.Numerics;

namespace SimpleLevelEditorV2.Rendering.Internals;

internal sealed class LineRenderer
{
	private readonly GL _gl;

	private readonly uint _lineVao;
	private readonly uint _centeredLineVao;

	private readonly Vector3[] _sphereVertices;
	private readonly uint _sphereVao;

	private readonly Vector3[] _pointVertices = VertexUtils.GetSphereVertexPositions(3, 6, 1);
	private readonly uint _pointVao;

	private readonly uint _planeLineVao;
	private readonly uint[] _planeLineIndices = [0, 1, 1, 2, 2, 3, 3, 0];

	private readonly uint _cubeVao;

	private readonly ShaderCacheEntry _lineShader;
	private readonly int _modelUniform;
	private readonly int _colorUniform;
	private readonly int _fadeOutUniform;

	public LineRenderer(GL gl)
	{
		_gl = gl;
		_lineShader = InternalContent.Shaders["Line"];
		_modelUniform = _lineShader.GetUniformLocation(_gl, "model");
		_colorUniform = _lineShader.GetUniformLocation(_gl, "color");
		_fadeOutUniform = _lineShader.GetUniformLocation(_gl, "fadeOut");

		_lineVao = VaoUtils.CreateLineVao(_gl, [Vector3.Zero, Vector3.UnitZ]);
		_centeredLineVao = VaoUtils.CreateLineVao(_gl, [-Vector3.UnitZ, Vector3.UnitZ]);

		_sphereVertices = VertexUtils.GetSphereVertexPositions(8, 16, 1);
		_sphereVao = VaoUtils.CreateLineVao(_gl, _sphereVertices);

		_pointVao = VaoUtils.CreateLineVao(_gl, _pointVertices);

		_planeLineVao = VaoUtils.CreateLineVao(_gl, [
			new Vector3(-0.5f, -0.5f, 0),
			new Vector3(-0.5f, 0.5f, 0),
			new Vector3(0.5f, 0.5f, 0),
			new Vector3(0.5f, -0.5f, 0),
			new Vector3(-0.5f, -0.5f, 0),
		]);

		_cubeVao = VaoUtils.CreateLineVao(_gl, [
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
	}

	public void Render(
		float gridCellFadeOutMinDistance,
		float gridCellFadeOutMaxDistance,
		Vector3? moveTargetPosition,
		float targetHeight,
		int gridCellInterval,
		Vector3? selectedPosition,
		Matrix4x4 view,
		Matrix4x4 projection,
		Vector3 cameraPosition,
		Vector3 focusPointTarget)
	{
		float fadeOutMinDistance = Math.Min(gridCellFadeOutMinDistance, gridCellFadeOutMaxDistance);
		float fadeOutMaxDistance = Math.Max(gridCellFadeOutMinDistance, gridCellFadeOutMaxDistance);

		_gl.UseProgram(_lineShader.Id);

		_gl.UniformMatrix4x4(_lineShader.GetUniformLocation(_gl, "view"), view);
		_gl.UniformMatrix4x4(_lineShader.GetUniformLocation(_gl, "projection"), projection);
		_gl.Uniform3(_lineShader.GetUniformLocation(_gl, "cameraPosition"), cameraPosition);
		_gl.Uniform1(_lineShader.GetUniformLocation(_gl, "fadeMinDistance"), fadeOutMinDistance);
		_gl.Uniform1(_lineShader.GetUniformLocation(_gl, "fadeMaxDistance"), fadeOutMaxDistance);

		_gl.BindVertexArray(_lineVao);

		_gl.Uniform1(_fadeOutUniform, 0);
		RenderOrigin();

		_gl.Uniform1(_fadeOutUniform, 1);
		RenderGrid(focusPointTarget with { Y = targetHeight }, new Vector4(1, 1, 1, 0.25f), fadeOutMaxDistance, gridCellInterval);
		if (moveTargetPosition.HasValue)
		{
			if (selectedPosition != null)
			{
				bool movedHorizontally = MathF.Abs(moveTargetPosition.Value.X - selectedPosition.Value.X) > float.Epsilon || MathF.Abs(moveTargetPosition.Value.Z - selectedPosition.Value.Z) > float.Epsilon;
				if (movedHorizontally && MathF.Abs(targetHeight - selectedPosition.Value.Y) > float.Epsilon)
					RenderGrid(focusPointTarget with { Y = selectedPosition.Value.Y }, new Vector4(1, 1, 0, 0.25f), fadeOutMaxDistance, gridCellInterval);
			}
		}

		_gl.Uniform1(_fadeOutUniform, 0);
		// RenderEntities();

		_gl.BindVertexArray(_centeredLineVao);
		RenderFocusAxes(focusPointTarget);
		RenderMoveComparison(moveTargetPosition, selectedPosition);
	}

	private void RenderLine(Matrix4x4 modelMatrix, Vector4 color)
	{
		_gl.UniformMatrix4x4(_modelUniform, modelMatrix);
		_gl.Uniform4(_colorUniform, color);
		_gl.DrawArrays(PrimitiveType.Lines, 0, 2);
	}

	private void RenderOrigin()
	{
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(1, 1, 256);

		_gl.LineWidth(4);
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2), new Vector4(1, 0, 0, 1));
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 1.5f), new Vector4(0, 1, 0, 1));
		RenderLine(scaleMatrix, new Vector4(0, 0, 1, 1));

		_gl.LineWidth(2);
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, -MathF.PI / 2), new Vector4(1, 0, 0, 0.5f));
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2), new Vector4(0, 1, 0, 0.5f));
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI), new Vector4(0, 0, 1, 0.5f));
	}

	private void RenderFocusAxes(Vector3 focusPointTarget)
	{
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(1, 1, 128);
		Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(focusPointTarget);

		_gl.LineWidth(1);
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2) * translationMatrix, new Vector4(1, 0.5f, 0, 0.5f));
		RenderLine(scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 1.5f) * translationMatrix, new Vector4(0, 1, 0.5f, 0.5f));
		RenderLine(scaleMatrix * translationMatrix, new Vector4(0.5f, 0, 1, 0.5f));
	}

	private void RenderMoveComparison(Vector3? moveTargetPosition, Vector3? selectedPosition)
	{
		if (!moveTargetPosition.HasValue)
			return;

		if (selectedPosition == null)
			return;

		Vector3 moveTarget = moveTargetPosition.Value;

		_gl.LineWidth(6);
		Vector4 color = new(1, 1, 0, 1);

		float halfDistanceX = MathF.Abs(selectedPosition.Value.X - moveTarget.X) / 2;
		float halfDistanceY = MathF.Abs(selectedPosition.Value.Y - moveTarget.Y) / 2;
		float halfDistanceZ = MathF.Abs(selectedPosition.Value.Z - moveTarget.Z) / 2;

		Vector3 averageOnX = selectedPosition.Value with { X = (selectedPosition.Value.X + moveTarget.X) / 2 };
		Vector3 averageOnY = selectedPosition.Value with { Y = (selectedPosition.Value.Y + moveTarget.Y) / 2 };
		Vector3 averageOnZ = selectedPosition.Value with { Z = (selectedPosition.Value.Z + moveTarget.Z) / 2 };

		RenderLine(Matrix4x4.CreateScale(1, 1, halfDistanceX) * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2) * Matrix4x4.CreateTranslation(averageOnX with { Z = moveTarget.Z }), color);
		RenderLine(Matrix4x4.CreateScale(1, 1, halfDistanceY) * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2) * Matrix4x4.CreateTranslation(averageOnY), color);
		RenderLine(Matrix4x4.CreateScale(1, 1, halfDistanceZ) * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI) * Matrix4x4.CreateTranslation(averageOnZ), color);
	}

	private void RenderGrid(Vector3 origin, Vector4 color, float cellCount, int interval)
	{
		interval = Math.Max(1, interval);

		_gl.Uniform4(_colorUniform, color);
		int lineWidthCache = 1; // Prevents unnecessary calls to Gl.LineWidth.

		int min = (int)-cellCount;
		int max = (int)cellCount;
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(new Vector3(1, 1, max - min));
		Vector3 offset = new(MathF.Round(origin.X), 0, MathF.Round(origin.Z));

		for (int i = min; i <= max; i++)
		{
			// Prevent rendering grid lines on top of origin lines (Z-fighting).
			if (!origin.Y.IsZero() || !(i + offset.X).IsZero())
			{
				UpdateLineWidth(i + (int)offset.X);

				_gl.UniformMatrix4x4(_modelUniform, scaleMatrix * Matrix4x4.CreateTranslation(new Vector3(i, origin.Y, min) + offset));
				_gl.DrawArrays(PrimitiveType.Lines, 0, 2);
			}

			if (!origin.Y.IsZero() || !(i + offset.Z).IsZero())
			{
				UpdateLineWidth(i + (int)offset.Z);

				_gl.UniformMatrix4x4(_modelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2) * Matrix4x4.CreateTranslation(new Vector3(min, origin.Y, i) + offset));
				_gl.DrawArrays(PrimitiveType.Lines, 0, 2);
			}
		}

		void UpdateLineWidth(int i)
		{
			int newLineWidth = i % interval == 0 ? 2 : 1;
			if (newLineWidth != lineWidthCache)
			{
				_gl.LineWidth(newLineWidth);
				lineWidthCache = newLineWidth;
			}
		}
	}

	// private void RenderModelEdges(Model model, Matrix4x4 modelMatrix, Vector4 color)
	// {
	// 	if (color.W < float.Epsilon)
	// 		return;
	//
	// 	_gl.UniformMatrix4x4(_modelUniform, modelMatrix);
	// 	_gl.Uniform4(_colorUniform, color);
	//
	// 	for (int i = 0; i < model.Meshes.Count; i++)
	// 	{
	// 		Mesh mesh = model.Meshes[i];
	// 		RenderEdges(mesh.LineVao, mesh.LineIndices);
	// 	}
	// }
	//
	// private unsafe void RenderEdges(uint lineVao, uint[] lineIndices)
	// {
	// 	_gl.BindVertexArray(lineVao);
	// 	fixed (uint* index = &lineIndices[0])
	// 		_gl.DrawElements(PrimitiveType.Lines, (uint)lineIndices.Length, DrawElementsType.UnsignedInt, index);
	// }
	//
	// private void RenderEntities()
	// {
	// 	for (int i = 0; i < LevelState.Level.Entities.Count; i++)
	// 	{
	// 		Entity entity = LevelState.Level.Entities[i];
	// 		if (!LevelEditorState.ShouldRenderEntity(entity))
	// 			continue;
	//
	// 		RenderEntity(entity, entity.Position);
	// 	}
	//
	// 	if (LevelEditorState.MoveTargetPosition.HasValue && LevelEditorState.SelectedEntity != null)
	// 		RenderEntity(LevelEditorState.SelectedEntity, LevelEditorState.MoveTargetPosition.Value);
	// }
	//
	// private void RenderEntity(Entity entity, Vector3 entityPosition)
	// {
	// 	EntityShapeDescriptor? entityShapeDescriptor = EntityConfigState.GetEntityShapeDescriptor(entity);
	// 	if (entityShapeDescriptor is EntityShapeDescriptor.Point point)
	// 	{
	// 		if (point.Visualization is PointEntityVisualization.SimpleSphere simpleSphere)
	// 		{
	// 			_gl.UniformMatrix4x4(_modelUniform, Matrix4x4.CreateScale(simpleSphere.Radius) * Matrix4x4.CreateTranslation(entityPosition));
	// 			_gl.Uniform4(_colorUniform, GetEntityLineColor(entity, simpleSphere.Color, simpleSphere.Color.ToVector4()));
	// 			_gl.BindVertexArray(_pointVao);
	// 			_gl.DrawArrays(PrimitiveType.Lines, 0, (uint)_pointVertices.Length);
	// 		}
	// 		else if (point.Visualization is PointEntityVisualization.Model modelVisualization)
	// 		{
	// 			Model? model = ModelContainer.EntityConfigContainer.GetModel(modelVisualization.ModelPath);
	// 			if (model != null)
	// 				RenderModelEdges(model, Matrix4x4.CreateTranslation(entityPosition), GetEntityLineColor(entity, new Rgb(191, 63, 63), Vector4.Zero));
	// 		}
	// 		else if (point.Visualization is PointEntityVisualization.BillboardSprite billboardSprite)
	// 		{
	// 			Matrix4x4 modelMatrix = Matrix4x4.CreateScale(billboardSprite.Size) * EntityMatrixUtils.GetBillboardMatrix(entityPosition);
	//
	// 			_gl.UniformMatrix4x4(_modelUniform, modelMatrix);
	// 			_gl.Uniform4(_colorUniform, GetEntityLineColor(entity, new Rgb(63, 63, 191), Vector4.Zero));
	// 			RenderEdges(_planeLineVao, _planeLineIndices);
	// 		}
	// 	}
	// 	else if (entityShapeDescriptor is EntityShapeDescriptor.Sphere sphere)
	// 	{
	// 		if (entity.Shape is not EntityShape.Sphere sphereDescriptor)
	// 			throw new InvalidOperationException($"Entity '{entity.Name}' is of shape type '{entity.Shape}' which does not match shape type '{entityShapeDescriptor}' from the EntityConfig.");
	//
	// 		_gl.UniformMatrix4x4(_modelUniform, Matrix4x4.CreateScale(sphereDescriptor.Radius) * Matrix4x4.CreateTranslation(entityPosition));
	// 		_gl.Uniform4(_colorUniform, GetEntityLineColor(entity, sphere.Color, sphere.Color.ToVector4()));
	// 		_gl.BindVertexArray(_sphereVao);
	// 		_gl.DrawArrays(PrimitiveType.Lines, 0, (uint)_sphereVertices.Length);
	// 	}
	// 	else if (entityShapeDescriptor is EntityShapeDescriptor.Aabb aabb)
	// 	{
	// 		if (entity.Shape is not EntityShape.Aabb aabbDescriptor)
	// 			throw new InvalidOperationException($"Entity '{entity.Name}' is of shape type '{entity.Shape}' which does not match shape type '{entityShapeDescriptor}' from the EntityConfig.");
	//
	// 		_gl.UniformMatrix4x4(_modelUniform, Matrix4x4.CreateScale(aabbDescriptor.Size) * Matrix4x4.CreateTranslation(entityPosition));
	// 		_gl.Uniform4(_colorUniform, GetEntityLineColor(entity, aabb.Color, aabb.Color.ToVector4()));
	// 		_gl.BindVertexArray(_cubeVao);
	// 		_gl.DrawArrays(PrimitiveType.Lines, 0, 24);
	// 	}
	// }
	//
	// private static Vector4 GetEntityLineColor(Entity entity, Rgb rgb, Vector4 defaultColor)
	// {
	// 	float timeAddition = MathF.Sin((float)Glfw.GetTime() * 10) * 0.5f + 0.5f;
	// 	timeAddition *= 0.5f;
	//
	// 	Vector4 color = rgb.ToVector4();
	//
	// 	// ReSharper disable PossibleUnintendedReferenceComparison
	// 	if (entity == LevelEditorState.SelectedEntity && entity == LevelEditorState.HighlightedEntity && Camera3d.Mode == CameraMode.None)
	// 		return color + new Vector4(0.5f + timeAddition, 1, 0.5f + timeAddition, 1);
	// 	if (entity == LevelEditorState.SelectedEntity)
	// 		return color + new Vector4(0, 0.75f, 0, 1);
	// 	if (entity == LevelEditorState.HighlightedEntity && Camera3d.Mode == CameraMode.None)
	// 		return color + new Vector4(1, 0.5f + timeAddition, 1, 1);
	//
	// 	// ReSharper restore PossibleUnintendedReferenceComparison
	// 	return defaultColor;
	// }
}
