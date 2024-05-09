using Detach.Utils;
using Silk.NET.OpenGL;
using SimpleLevelEditor.Content;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.Formats.Types;
using SimpleLevelEditor.Formats.Types.EntityConfig;
using SimpleLevelEditor.Formats.Types.Level;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;
using static SimpleLevelEditor.Graphics;

namespace SimpleLevelEditor.Rendering;

public static class SceneRenderer
{
	public const float PointScale = 0.3f;

	private static readonly uint _lineVao = VaoUtils.CreateLineVao([Vector3.Zero, Vector3.UnitZ]);
	private static readonly uint _cubeVao = VaoUtils.CreateLineVao([
		new(-0.5f, -0.5f, -0.5f),
		new(-0.5f, -0.5f, 0.5f),
		new(-0.5f, 0.5f, -0.5f),
		new(-0.5f, 0.5f, 0.5f),
		new(0.5f, -0.5f, -0.5f),
		new(0.5f, -0.5f, 0.5f),
		new(0.5f, 0.5f, -0.5f),
		new(0.5f, 0.5f, 0.5f),

		new(-0.5f, -0.5f, -0.5f),
		new(-0.5f, 0.5f, -0.5f),
		new(-0.5f, -0.5f, 0.5f),
		new(-0.5f, 0.5f, 0.5f),
		new(0.5f, -0.5f, -0.5f),
		new(0.5f, 0.5f, -0.5f),
		new(0.5f, -0.5f, 0.5f),
		new(0.5f, 0.5f, 0.5f),

		new(-0.5f, -0.5f, -0.5f),
		new(0.5f, -0.5f, -0.5f),
		new(-0.5f, -0.5f, 0.5f),
		new(0.5f, -0.5f, 0.5f),
		new(-0.5f, 0.5f, -0.5f),
		new(0.5f, 0.5f, -0.5f),
		new(-0.5f, 0.5f, 0.5f),
		new(0.5f, 0.5f, 0.5f),
	]);

	private static readonly Vector3[] _sphereVertices = GetSphereVertexPositions(8, 16, 1);
	private static readonly uint _sphereVao = VaoUtils.CreateLineVao(_sphereVertices);

	private static readonly Vector3[] _pointVertices = GetSphereVertexPositions(3, 6, 1);
	private static readonly uint _pointVao = VaoUtils.CreateLineVao(_pointVertices);

	private static readonly float[] _planeVertices =
	[
		-0.5f, -0.5f, 0, 0, 0,
		-0.5f, 0.5f, 0, 0, 1,
		0.5f, -0.5f, 0, 1, 0,
		0.5f, 0.5f, 0, 1, 1,
	];
	private static readonly uint _planeVao = VaoUtils.CreatePlaneVao(_planeVertices);
	private static readonly uint[] _planeIndices = [0, 1, 2, 2, 1, 3];

	private static Vector3[] GetSphereVertexPositions(uint horizontalLines, uint verticalLines, float radius)
	{
		List<Vector3> vertices = [];
		for (uint i = 0; i <= horizontalLines; i++)
		{
			float horizontalAngle = MathF.PI * i / horizontalLines;

			for (uint j = 0; j <= verticalLines; j++)
			{
				float verticalAngle = 2 * MathF.PI * j / verticalLines;

				float x = MathF.Sin(horizontalAngle) * MathF.Cos(verticalAngle);
				float y = MathF.Cos(horizontalAngle);
				float z = MathF.Sin(horizontalAngle) * MathF.Sin(verticalAngle);

				if (j != 0 && j != verticalLines)
					vertices.Add(new Vector3(x, y, z) * radius);

				vertices.Add(new Vector3(x, y, z) * radius);
			}
		}

		for (uint i = 0; i <= verticalLines; i++)
		{
			float verticalAngle = 2 * MathF.PI * i / verticalLines;

			for (uint j = 0; j <= horizontalLines; j++)
			{
				float horizontalAngle = MathF.PI * j / horizontalLines;

				float x = MathF.Sin(horizontalAngle) * MathF.Cos(verticalAngle);
				float y = MathF.Cos(horizontalAngle);
				float z = MathF.Sin(horizontalAngle) * MathF.Sin(verticalAngle);

				if (j != 0 && j != horizontalLines)
					vertices.Add(new Vector3(x, y, z) * radius);

				vertices.Add(new Vector3(x, y, z) * radius);
			}
		}

		return vertices.ToArray();
	}

	public static void RenderScene()
	{
		ShaderCacheEntry lineShader = InternalContent.Shaders["Line"];
		Gl.UseProgram(lineShader.Id);

		Gl.UniformMatrix4x4(lineShader.GetUniformLocation("view"), Camera3d.ViewMatrix);
		Gl.UniformMatrix4x4(lineShader.GetUniformLocation("projection"), Camera3d.Projection);

		RenderOrigin(lineShader);
		RenderGrid(lineShader);
		RenderEdges(lineShader);
		RenderEntitiesWithLineShader(lineShader);

		ShaderCacheEntry meshShader = InternalContent.Shaders["Mesh"];
		Gl.UseProgram(meshShader.Id);

		Gl.UniformMatrix4x4(meshShader.GetUniformLocation("view"), Camera3d.ViewMatrix);
		Gl.UniformMatrix4x4(meshShader.GetUniformLocation("projection"), Camera3d.Projection);

		RenderEntitiesWithMeshShader(meshShader);

		if (LevelEditorState.ShouldRenderWorldObjects())
			RenderWorldObjects(meshShader);

		ShaderCacheEntry spriteShader = InternalContent.Shaders["Sprite"];
		Gl.UseProgram(spriteShader.Id);

		Gl.UniformMatrix4x4(spriteShader.GetUniformLocation("view"), Camera3d.ViewMatrix);
		Gl.UniformMatrix4x4(spriteShader.GetUniformLocation("projection"), Camera3d.Projection);

		RenderEntitiesWithSpriteShader(spriteShader);
	}

	private static void RenderOrigin(ShaderCacheEntry lineShader)
	{
		int lineModelUniform = lineShader.GetUniformLocation("model");
		int lineColorUniform = lineShader.GetUniformLocation("color");

		Gl.BindVertexArray(_lineVao);

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

	private static void RenderGrid(ShaderCacheEntry lineShader)
	{
		int lineModelUniform = lineShader.GetUniformLocation("model");
		int lineColorUniform = lineShader.GetUniformLocation("color");

		Gl.Uniform4(lineColorUniform, new Vector4(0.5f, 0.5f, 0.5f, 1));
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

		for (int i = 0; i < LevelState.Level.WorldObjects.Count; i++)
		{
			WorldObject worldObject = LevelState.Level.WorldObjects[i];
			RenderEdges(lineShader, worldObject, GetColor(worldObject));
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

	private static unsafe void RenderWorldObjects(ShaderCacheEntry meshShader)
	{
		int modelUniform = meshShader.GetUniformLocation("model");
		for (int i = 0; i < LevelState.Level.WorldObjects.Count; i++)
		{
			WorldObject worldObject = LevelState.Level.WorldObjects[i];

			MeshEntry? mesh = MeshContainer.GetMesh(worldObject.Mesh);
			if (mesh == null)
				continue;

			uint? textureId = TextureContainer.GetTexture(worldObject.Texture);
			if (textureId == null)
				continue;

			Gl.UniformMatrix4x4(modelUniform, worldObject.GetModelMatrix());

			Gl.BindTexture(TextureTarget.Texture2D, textureId.Value);

			Gl.BindVertexArray(mesh.MeshVao);
			fixed (uint* index = &mesh.Mesh.Indices[0])
				Gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Mesh.Indices.Length, DrawElementsType.UnsignedInt, index);
		}
	}

	private static void RenderEntitiesWithLineShader(ShaderCacheEntry lineShader)
	{
		int modelUniform = lineShader.GetUniformLocation("model");
		int colorUniform = lineShader.GetUniformLocation("color");
		for (int i = 0; i < LevelState.Level.Entities.Count; i++)
		{
			Entity entity = LevelState.Level.Entities[i];
			if (!LevelEditorState.ShouldRenderEntity(entity))
				continue;

			EntityShape? entityShape = GetEntityShape(entity);
			if (entityShape is EntityShape.Point point)
			{
				if (point.Visualization is PointEntityVisualization.SimpleSphere simpleSphere)
				{
					Gl.UniformMatrix4x4(modelUniform, Matrix4x4.CreateScale(simpleSphere.Radius) * Matrix4x4.CreateTranslation(entity.Position));
					Gl.Uniform4(colorUniform, GetColor(entity, simpleSphere.Color));
					Gl.BindVertexArray(_pointVao);
					Gl.DrawArrays(PrimitiveType.Lines, 0, (uint)_pointVertices.Length);
				}
				else if (point.Visualization is PointEntityVisualization.Mesh mesh)
				{
					RenderEdges(lineShader, mesh.MeshName, entity.Position, GetColor(entity, new Rgb(255, 127, 63)));
				}
				else if (point.Visualization is PointEntityVisualization.BillboardSprite billboardSprite)
				{
					// TODO: Use different VAO and line indices.
					Matrix4x4 modelMatrix = Matrix4x4.CreateScale(billboardSprite.Size) * EntityMatrixUtils.GetBillboardMatrix(entity);
					RenderEdges(lineShader, _planeVao, _planeIndices, modelMatrix, GetColor(entity, new Rgb(255, 127, 63)));
				}
			}
			else if (entityShape is EntityShape.Sphere sphere)
			{
				if (entity.Shape is not ShapeDescriptor.Sphere sphereDescriptor)
					throw new InvalidOperationException($"Entity '{entity.Name}' is of shape type '{entity.Shape}' which does not match shape type '{entityShape}' from the EntityConfig.");

				Gl.UniformMatrix4x4(modelUniform, Matrix4x4.CreateScale(sphereDescriptor.Radius) * Matrix4x4.CreateTranslation(entity.Position));
				Gl.Uniform4(colorUniform, GetColor(entity, sphere.Color));
				Gl.BindVertexArray(_sphereVao);
				Gl.DrawArrays(PrimitiveType.Lines, 0, (uint)_sphereVertices.Length);
			}
			else if (entityShape is EntityShape.Aabb aabb)
			{
				if (entity.Shape is not ShapeDescriptor.Aabb aabbDescriptor)
					throw new InvalidOperationException($"Entity '{entity.Name}' is of shape type '{entity.Shape}' which does not match shape type '{entityShape}' from the EntityConfig.");

				Gl.UniformMatrix4x4(modelUniform, Matrix4x4.CreateScale(aabbDescriptor.Size) * Matrix4x4.CreateTranslation(entity.Position));
				Gl.Uniform4(colorUniform, GetColor(entity, aabb.Color));
				Gl.BindVertexArray(_cubeVao);
				Gl.DrawArrays(PrimitiveType.Lines, 0, 24);
			}
		}
	}

	private static unsafe void RenderEntitiesWithMeshShader(ShaderCacheEntry meshShader)
	{
		int modelUniform = meshShader.GetUniformLocation("model");
		for (int i = 0; i < LevelState.Level.Entities.Count; i++)
		{
			Entity entity = LevelState.Level.Entities[i];
			if (!LevelEditorState.ShouldRenderEntity(entity))
				continue;

			EntityShape? entityShape = GetEntityShape(entity);
			if (entityShape is EntityShape.Point { Visualization: PointEntityVisualization.Mesh meshVisualization })
			{
				MeshEntry? mesh = MeshContainer.GetMesh(meshVisualization.MeshName);
				if (mesh == null)
					continue;

				uint? textureId = TextureContainer.GetTexture(meshVisualization.TextureName);
				if (textureId == null)
					continue;

				Gl.UniformMatrix4x4(modelUniform, Matrix4x4.CreateTranslation(entity.Position));

				Gl.BindTexture(TextureTarget.Texture2D, textureId.Value);

				Gl.BindVertexArray(mesh.MeshVao);
				fixed (uint* index = &mesh.Mesh.Indices[0])
					Gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Mesh.Indices.Length, DrawElementsType.UnsignedInt, index);
			}
		}
	}

	private static unsafe void RenderEntitiesWithSpriteShader(ShaderCacheEntry spriteShader)
	{
		int modelUniform = spriteShader.GetUniformLocation("model");
		for (int i = 0; i < LevelState.Level.Entities.Count; i++)
		{
			Entity entity = LevelState.Level.Entities[i];
			if (!LevelEditorState.ShouldRenderEntity(entity))
				continue;

			EntityShape? entityShape = GetEntityShape(entity);
			if (entityShape is EntityShape.Point { Visualization: PointEntityVisualization.BillboardSprite billboardSprite })
			{
				uint? textureId = TextureContainer.GetTexture(billboardSprite.TextureName);
				if (textureId == null)
					continue;

				Gl.UniformMatrix4x4(modelUniform, Matrix4x4.CreateScale(new Vector3(billboardSprite.Size, billboardSprite.Size, 1)) * EntityMatrixUtils.GetBillboardMatrix(entity));

				Gl.BindTexture(TextureTarget.Texture2D, textureId.Value);

				Gl.BindVertexArray(_planeVao);
				fixed (uint* indexPtr = &_planeIndices[0])
					Gl.DrawElements(PrimitiveType.Triangles, (uint)_planeIndices.Length, DrawElementsType.UnsignedInt, indexPtr);
			}
		}
	}

	private static EntityShape? GetEntityShape(Entity entity)
	{
		if (EntityConfigState.EntityConfig.Entities.Count == 0)
			return null; // EntityConfig not loaded yet.

		EntityShape? entityShape = EntityConfigState.EntityConfig.Entities.Find(e => e.Name == entity.Name)?.Shape;
		if (entityShape == null)
			throw new InvalidOperationException($"Entity '{entity.Name}' does not have a shape defined in the EntityConfig.");

		return entityShape;
	}

	private static Vector4 GetColor(Entity entity, Rgb rgb)
	{
		float timeAddition = MathF.Sin((float)Glfw.GetTime() * 10) * 0.5f + 0.5f;
		timeAddition *= 0.5f;
		Vector4 color = rgb.ToVector4();
		if (entity == LevelEditorState.SelectedEntity && entity == LevelEditorState.HighlightedEntity && Camera3d.Mode == CameraMode.None)
			return color + new Vector4(0.5f + timeAddition, 1, 0.5f + timeAddition, 1);
		if (entity == LevelEditorState.SelectedEntity)
			return color + new Vector4(0, 0.75f, 0, 1);
		if (entity == LevelEditorState.HighlightedEntity && Camera3d.Mode == CameraMode.None)
			return color + new Vector4(1, 0.5f + timeAddition, 1, 1);

		return color + new Vector4(0.75f, 0, 0.75f, 1);
	}

	private static Vector4 GetColor(WorldObject worldObject)
	{
		float timeAddition = MathF.Sin((float)Glfw.GetTime() * 10) * 0.5f + 0.5f;
		timeAddition *= 0.5f;

		if (worldObject == LevelEditorState.SelectedWorldObject && worldObject == LevelEditorState.HighlightedObject && Camera3d.Mode == CameraMode.None)
			return new Vector4(0.5f + timeAddition, 1, 0.5f + timeAddition, 1);
		if (worldObject == LevelEditorState.SelectedWorldObject)
			return new Vector4(0, 0.75f, 0, 1);
		if (worldObject == LevelEditorState.HighlightedObject && Camera3d.Mode == CameraMode.None)
			return new Vector4(1, 0.5f + timeAddition, 1, 1);

		return Vector4.Zero;
	}
}
