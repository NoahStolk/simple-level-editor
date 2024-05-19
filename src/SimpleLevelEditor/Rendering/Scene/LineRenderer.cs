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
		RenderGrid(LevelEditorState.TargetHeight);
		RenderWorldObjectEdges();
		RenderEntities();

		Gl.BindVertexArray(_centeredLineVao);
		RenderFocusAxes();
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

	private void RenderGrid(float height)
	{
		Gl.Uniform4(_colorUniform, new Vector4(1, 1, 1, 0.25f));
		Gl.LineWidth(1);

		int min = -LevelEditorState.GridCellCount;
		int max = LevelEditorState.GridCellCount;
		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(new Vector3(1, 1, (max - min) * LevelEditorState.GridCellSize));
		Vector3 offset = new(MathF.Round(Camera3d.Position.X), 0, MathF.Round(Camera3d.Position.Z));
		offset.X = MathF.Round(offset.X / LevelEditorState.GridCellSize) * LevelEditorState.GridCellSize;
		offset.Z = MathF.Round(offset.Z / LevelEditorState.GridCellSize) * LevelEditorState.GridCellSize;

		for (int i = min; i <= max; i++)
		{
			// Prevent rendering grid lines on top of origin lines (Z-fighting).
			if (!height.IsZero() || !(i * LevelEditorState.GridCellSize + offset.X).IsZero())
			{
				Gl.UniformMatrix4x4(_modelUniform, scaleMatrix * Matrix4x4.CreateTranslation(new Vector3(i * LevelEditorState.GridCellSize, height, min * LevelEditorState.GridCellSize) + offset));
				Gl.DrawArrays(PrimitiveType.Lines, 0, 2);
			}

			if (!height.IsZero() || !(i * LevelEditorState.GridCellSize + offset.Z).IsZero())
			{
				Gl.UniformMatrix4x4(_modelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2) * Matrix4x4.CreateTranslation(new Vector3(min * LevelEditorState.GridCellSize, height, i * LevelEditorState.GridCellSize) + offset));
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
			if (color.W < float.Epsilon)
				continue;

			MeshEntry? mesh = MeshContainer.GetMesh(worldObject.Mesh);
			if (mesh == null)
				continue;

			Matrix4x4 rotationMatrix = MathUtils.CreateRotationMatrixFromEulerAngles(MathUtils.ToRadians(worldObject.Rotation));
			Matrix4x4 modelMatrix = Matrix4x4.CreateScale(worldObject.Scale) * rotationMatrix * Matrix4x4.CreateTranslation(worldObject.Position);
			RenderEdges(mesh.LineVao, mesh.LineIndices, modelMatrix, color);
		}
	}

	private void RenderMeshEdges(string meshName, Vector3 position, Vector4 color)
	{
		if (color.W < float.Epsilon)
			return;

		MeshEntry? mesh = MeshContainer.GetMesh(meshName);
		if (mesh != null)
			RenderEdges(mesh.LineVao, mesh.LineIndices, Matrix4x4.CreateTranslation(position), color);
	}

	private unsafe void RenderEdges(uint lineVao, uint[] lineIndices, Matrix4x4 modelMatrix, Vector4 color)
	{
		if (color.W < float.Epsilon)
			return;

		Gl.UniformMatrix4x4(_modelUniform, modelMatrix);
		Gl.Uniform4(_colorUniform, color);

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
		EntityShape? entityShape = EntityConfigState.GetEntityShape(entity);
		if (entityShape is EntityShape.Point point)
		{
			if (point.Visualization is PointEntityVisualization.SimpleSphere simpleSphere)
			{
				Gl.UniformMatrix4x4(_modelUniform, Matrix4x4.CreateScale(simpleSphere.Radius) * Matrix4x4.CreateTranslation(entityPosition));
				Gl.Uniform4(_colorUniform, GetEntityLineColor(entity, simpleSphere.Color, simpleSphere.Color.ToVector4()));
				Gl.BindVertexArray(_pointVao);
				Gl.DrawArrays(PrimitiveType.Lines, 0, (uint)_pointVertices.Length);
			}
			else if (point.Visualization is PointEntityVisualization.Mesh mesh)
			{
				RenderMeshEdges(mesh.MeshName, entityPosition, GetEntityLineColor(entity, new Rgb(191, 63, 63), Vector4.Zero));
			}
			else if (point.Visualization is PointEntityVisualization.BillboardSprite billboardSprite)
			{
				Matrix4x4 modelMatrix = Matrix4x4.CreateScale(billboardSprite.Size) * EntityMatrixUtils.GetBillboardMatrix(entity, entity.Position);
				RenderEdges(_planeLineVao, _planeLineIndices, modelMatrix, GetEntityLineColor(entity, new Rgb(63, 63, 191), Vector4.Zero));
			}
		}
		else if (entityShape is EntityShape.Sphere sphere)
		{
			if (entity.Shape is not ShapeDescriptor.Sphere sphereDescriptor)
				throw new InvalidOperationException($"Entity '{entity.Name}' is of shape type '{entity.Shape}' which does not match shape type '{entityShape}' from the EntityConfig.");

			Gl.UniformMatrix4x4(_modelUniform, Matrix4x4.CreateScale(sphereDescriptor.Radius) * Matrix4x4.CreateTranslation(entityPosition));
			Gl.Uniform4(_colorUniform, GetEntityLineColor(entity, sphere.Color, sphere.Color.ToVector4()));
			Gl.BindVertexArray(_sphereVao);
			Gl.DrawArrays(PrimitiveType.Lines, 0, (uint)_sphereVertices.Length);
		}
		else if (entityShape is EntityShape.Aabb aabb)
		{
			if (entity.Shape is not ShapeDescriptor.Aabb aabbDescriptor)
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
