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

public static class LineRenderer
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

	public static void Render()
	{
		ShaderCacheEntry lineShader = InternalContent.Shaders["Line"];
		Gl.UseProgram(lineShader.Id);

		Gl.UniformMatrix4x4(lineShader.GetUniformLocation("view"), Camera3d.ViewMatrix);
		Gl.UniformMatrix4x4(lineShader.GetUniformLocation("projection"), Camera3d.Projection);

		Gl.BindVertexArray(_lineVao);
		RenderOrigin(lineShader);
		RenderGrid(lineShader);
		RenderEdges(lineShader);
		RenderEntitiesWithLineShader(lineShader);

		Gl.BindVertexArray(_centeredLineVao);
		RenderFocusAxes(lineShader);
	}

	private static void RenderOrigin(ShaderCacheEntry lineShader)
	{
		int lineModelUniform = lineShader.GetUniformLocation("model");
		int lineColorUniform = lineShader.GetUniformLocation("color");

		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(1, 1, 256);

		Gl.LineWidth(4);

		// X axis
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2));
		Gl.Uniform4(lineColorUniform, new Vector4(1, 0, 0, 1));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

		// Y axis
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 1.5f));
		Gl.Uniform4(lineColorUniform, new Vector4(0, 1, 0, 1));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

		// Z axis
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix);
		Gl.Uniform4(lineColorUniform, new Vector4(0, 0, 1, 1));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

		Gl.LineWidth(2);

		// X axis (negative)
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, -MathF.PI / 2));
		Gl.Uniform4(lineColorUniform, new Vector4(1, 0, 0, 0.5f));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

		// Y axis (negative)
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2));
		Gl.Uniform4(lineColorUniform, new Vector4(0, 1, 0, 0.5f));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

		// Z axis (negative)
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI));
		Gl.Uniform4(lineColorUniform, new Vector4(0, 0, 1, 0.5f));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);
	}

	private static void RenderFocusAxes(ShaderCacheEntry lineShader)
	{
		int lineModelUniform = lineShader.GetUniformLocation("model");
		int lineColorUniform = lineShader.GetUniformLocation("color");

		Matrix4x4 scaleMatrix = Matrix4x4.CreateScale(1, 1, 128);
		Matrix4x4 translationMatrix = Matrix4x4.CreateTranslation(Camera3d.FocusPointTarget);

		Gl.LineWidth(1);

		// X axis
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2) * translationMatrix);
		Gl.Uniform4(lineColorUniform, new Vector4(1, 0.5f, 0, 0.5f));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

		// Y axis
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 1.5f) * translationMatrix);
		Gl.Uniform4(lineColorUniform, new Vector4(0, 1, 0.5f, 0.5f));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

		// Z axis
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * translationMatrix);
		Gl.Uniform4(lineColorUniform, new Vector4(0.5f, 0, 1, 0.5f));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);
	}

	private static void RenderGrid(ShaderCacheEntry lineShader)
	{
		int lineModelUniform = lineShader.GetUniformLocation("model");
		int lineColorUniform = lineShader.GetUniformLocation("color");

		Gl.Uniform4(lineColorUniform, new Vector4(1, 1, 1, 0.25f));
		Gl.LineWidth(1);

		int min = -LevelEditorState.GridCellCount;
		int max = LevelEditorState.GridCellCount;
		Vector3 scale = new(1, 1, (max - min) * LevelEditorState.GridCellSize);
		Matrix4x4 scaleMat = Matrix4x4.CreateScale(scale);
		Vector3 offset = new(MathF.Round(Camera3d.Position.X), 0, MathF.Round(Camera3d.Position.Z));
		offset.X = MathF.Round(offset.X / LevelEditorState.GridCellSize) * LevelEditorState.GridCellSize;
		offset.Z = MathF.Round(offset.Z / LevelEditorState.GridCellSize) * LevelEditorState.GridCellSize;

		for (int i = min; i <= max; i++)
		{
			// Prevent rendering grid lines on top of origin lines (Z-fighting).
			if (!LevelEditorState.TargetHeight.IsZero() || !(i * LevelEditorState.GridCellSize + offset.X).IsZero())
			{
				Gl.UniformMatrix4x4(lineModelUniform, scaleMat * Matrix4x4.CreateTranslation(new Vector3(i * LevelEditorState.GridCellSize, LevelEditorState.TargetHeight, min * LevelEditorState.GridCellSize) + offset));
				Gl.DrawArrays(PrimitiveType.Lines, 0, 2);
			}

			if (!LevelEditorState.TargetHeight.IsZero() || !(i * LevelEditorState.GridCellSize + offset.Z).IsZero())
			{
				Gl.UniformMatrix4x4(lineModelUniform, scaleMat * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2) * Matrix4x4.CreateTranslation(new Vector3(min * LevelEditorState.GridCellSize, LevelEditorState.TargetHeight, i * LevelEditorState.GridCellSize) + offset));
				Gl.DrawArrays(PrimitiveType.Lines, 0, 2);
			}
		}
	}

	private static void RenderEdges(ShaderCacheEntry lineShader)
	{
		Gl.LineWidth(2);

		for (int i = 0; i < LevelState.Level.WorldObjects.Length; i++)
		{
			WorldObject worldObject = LevelState.Level.WorldObjects[i];
			RenderEdges(lineShader, worldObject, GetWorldObjectColor(worldObject));
		}
	}

	private static void RenderEdges(ShaderCacheEntry lineShader, WorldObject worldObject, Vector4 color)
	{
		if (color.W < float.Epsilon)
			return;

		MeshEntry? mesh = MeshContainer.GetMesh(worldObject.Mesh);
		if (mesh == null)
			return;

		Matrix4x4 rotationMatrix = MathUtils.CreateRotationMatrixFromEulerAngles(MathUtils.ToRadians(worldObject.Rotation));
		Matrix4x4 modelMatrix = Matrix4x4.CreateScale(worldObject.Scale) * rotationMatrix * Matrix4x4.CreateTranslation(worldObject.Position);
		RenderEdges(lineShader, mesh.LineVao, mesh.LineIndices, modelMatrix, color);
	}

	private static void RenderEdges(ShaderCacheEntry lineShader, string meshName, Vector3 position, Vector4 color)
	{
		if (color.W < float.Epsilon)
			return;

		MeshEntry? mesh = MeshContainer.GetMesh(meshName);
		if (mesh != null)
			RenderEdges(lineShader, mesh.LineVao, mesh.LineIndices, Matrix4x4.CreateTranslation(position), color);
	}

	private static unsafe void RenderEdges(ShaderCacheEntry lineShader, uint lineVao, uint[] meshLineIndices, Matrix4x4 modelMatrix, Vector4 color)
	{
		if (color.W < float.Epsilon)
			return;

		int lineModelUniform = lineShader.GetUniformLocation("model");
		int lineColorUniform = lineShader.GetUniformLocation("color");

		Gl.Uniform4(lineColorUniform, color);

		Gl.UniformMatrix4x4(lineModelUniform, modelMatrix);

		Gl.BindVertexArray(lineVao);
		fixed (uint* index = &meshLineIndices[0])
			Gl.DrawElements(PrimitiveType.Lines, (uint)meshLineIndices.Length, DrawElementsType.UnsignedInt, index);
	}

	private static void RenderEntitiesWithLineShader(ShaderCacheEntry lineShader)
	{
		int modelUniform = lineShader.GetUniformLocation("model");
		int colorUniform = lineShader.GetUniformLocation("color");
		for (int i = 0; i < LevelState.Level.Entities.Length; i++)
		{
			Entity entity = LevelState.Level.Entities[i];
			if (!LevelEditorState.ShouldRenderEntity(entity))
				continue;

			EntityShape? entityShape = EntityConfigState.GetEntityShape(entity);
			if (entityShape is EntityShape.Point point)
			{
				if (point.Visualization is PointEntityVisualization.SimpleSphere simpleSphere)
				{
					Gl.UniformMatrix4x4(modelUniform, Matrix4x4.CreateScale(simpleSphere.Radius) * Matrix4x4.CreateTranslation(entity.Position));
					Gl.Uniform4(colorUniform, GetEntityColor(entity, simpleSphere.Color, simpleSphere.Color.ToVector4()));
					Gl.BindVertexArray(_pointVao);
					Gl.DrawArrays(PrimitiveType.Lines, 0, (uint)_pointVertices.Length);
				}
				else if (point.Visualization is PointEntityVisualization.Mesh mesh)
				{
					RenderEdges(lineShader, mesh.MeshName, entity.Position, GetEntityColor(entity, new Rgb(191, 63, 63), Vector4.Zero));
				}
				else if (point.Visualization is PointEntityVisualization.BillboardSprite billboardSprite)
				{
					Matrix4x4 modelMatrix = Matrix4x4.CreateScale(billboardSprite.Size) * EntityMatrixUtils.GetBillboardMatrix(entity);
					RenderEdges(lineShader, _planeLineVao, _planeLineIndices, modelMatrix, GetEntityColor(entity, new Rgb(63, 63, 191), Vector4.Zero));
				}
			}
			else if (entityShape is EntityShape.Sphere sphere)
			{
				if (entity.Shape is not ShapeDescriptor.Sphere sphereDescriptor)
					throw new InvalidOperationException($"Entity '{entity.Name}' is of shape type '{entity.Shape}' which does not match shape type '{entityShape}' from the EntityConfig.");

				Gl.UniformMatrix4x4(modelUniform, Matrix4x4.CreateScale(sphereDescriptor.Radius) * Matrix4x4.CreateTranslation(entity.Position));
				Gl.Uniform4(colorUniform, GetEntityColor(entity, sphere.Color, sphere.Color.ToVector4()));
				Gl.BindVertexArray(_sphereVao);
				Gl.DrawArrays(PrimitiveType.Lines, 0, (uint)_sphereVertices.Length);
			}
			else if (entityShape is EntityShape.Aabb aabb)
			{
				if (entity.Shape is not ShapeDescriptor.Aabb aabbDescriptor)
					throw new InvalidOperationException($"Entity '{entity.Name}' is of shape type '{entity.Shape}' which does not match shape type '{entityShape}' from the EntityConfig.");

				Gl.UniformMatrix4x4(modelUniform, Matrix4x4.CreateScale(aabbDescriptor.Size) * Matrix4x4.CreateTranslation(entity.Position));
				Gl.Uniform4(colorUniform, GetEntityColor(entity, aabb.Color, aabb.Color.ToVector4()));
				Gl.BindVertexArray(_cubeVao);
				Gl.DrawArrays(PrimitiveType.Lines, 0, 24);
			}
		}
	}

	private static Vector4 GetEntityColor(Entity entity, Rgb rgb, Vector4 defaultColor)
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

	private static Vector4 GetWorldObjectColor(WorldObject worldObject)
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
