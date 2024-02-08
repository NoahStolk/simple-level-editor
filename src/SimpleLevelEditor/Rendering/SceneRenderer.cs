using Detach.Utils;
using Silk.NET.OpenGL;
using SimpleLevelEditor.Content;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Model.EntityShapes;
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
		RenderEntities(lineShader);

		ShaderCacheEntry meshShader = InternalContent.Shaders["Mesh"];
		Gl.UseProgram(meshShader.Id);

		Gl.UniformMatrix4x4(meshShader.GetUniformLocation("view"), Camera3d.ViewMatrix);
		Gl.UniformMatrix4x4(meshShader.GetUniformLocation("projection"), Camera3d.Projection);

		RenderWorldObjects(meshShader);
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
		Gl.UniformVector4(lineColorUniform, new Vector4(1, 0, 0, 1));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

		// Y axis
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 1.5f));
		Gl.UniformVector4(lineColorUniform, new Vector4(0, 1, 0, 1));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

		// Z axis
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix);
		Gl.UniformVector4(lineColorUniform, new Vector4(0, 0, 1, 1));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

		Gl.LineWidth(2);

		// X axis (negative)
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, -MathF.PI / 2));
		Gl.UniformVector4(lineColorUniform, new Vector4(1, 0, 0, 0.5f));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

		// Y axis (negative)
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2));
		Gl.UniformVector4(lineColorUniform, new Vector4(0, 1, 0, 0.5f));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);

		// Z axis (negative)
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI));
		Gl.UniformVector4(lineColorUniform, new Vector4(0, 0, 1, 0.5f));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 2);
	}

	private static void RenderGrid(ShaderCacheEntry lineShader)
	{
		int lineModelUniform = lineShader.GetUniformLocation("model");
		int lineColorUniform = lineShader.GetUniformLocation("color");

		Gl.UniformVector4(lineColorUniform, new Vector4(0.5f, 0.5f, 0.5f, 1));
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
			if (LevelEditorState.TargetHeight != 0 || i * LevelEditorState.GridCellSize + offset.X != 0)
			{
				Gl.UniformMatrix4x4(lineModelUniform, scaleMat * Matrix4x4.CreateTranslation(new Vector3(i * LevelEditorState.GridCellSize, LevelEditorState.TargetHeight, min * LevelEditorState.GridCellSize) + offset));
				Gl.DrawArrays(PrimitiveType.Lines, 0, 2);
			}

			if (LevelEditorState.TargetHeight != 0 || i * LevelEditorState.GridCellSize + offset.Z != 0)
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

			float timeAddition = MathF.Sin((float)Glfw.GetTime() * 10) * 0.5f + 0.5f;
			timeAddition *= 0.5f;

			Vector4 color;
			if (worldObject == LevelEditorState.SelectedWorldObject && worldObject == LevelEditorState.HighlightedObject && Camera3d.Mode == CameraMode.None)
				color = new(0.5f + timeAddition, 1, 0.5f + timeAddition, 1);
			else if (worldObject == LevelEditorState.SelectedWorldObject)
				color = new(0, 0.75f, 0, 1);
			else if (worldObject == LevelEditorState.HighlightedObject && Camera3d.Mode == CameraMode.None)
				color = new(1, 0.5f + timeAddition, 1, 1);
			else
				continue;

			RenderEdges(lineShader, worldObject, color);
		}
	}

	private static unsafe void RenderEdges(ShaderCacheEntry lineShader, WorldObject worldObject, Vector4 color)
	{
		MeshContainer.Entry? mesh = MeshContainer.GetMesh(worldObject.Mesh);
		if (mesh == null)
			return;

		int lineModelUniform = lineShader.GetUniformLocation("model");
		int lineColorUniform = lineShader.GetUniformLocation("color");

		Gl.UniformVector4(lineColorUniform, color);

		Matrix4x4 rotationMatrix = MathUtils.CreateRotationMatrixFromEulerAngles(MathUtils.ToRadians(worldObject.Rotation));
		Matrix4x4 modelMatrix = Matrix4x4.CreateScale(worldObject.Scale) * rotationMatrix * Matrix4x4.CreateTranslation(worldObject.Position);
		Gl.UniformMatrix4x4(lineModelUniform, modelMatrix);

		Gl.BindVertexArray(mesh.LineVao);
		fixed (uint* index = &mesh.LineIndices[0])
			Gl.DrawElements(PrimitiveType.Lines, (uint)mesh.LineIndices.Length, DrawElementsType.UnsignedInt, index);
	}

	private static unsafe void RenderWorldObjects(ShaderCacheEntry meshShader)
	{
		int modelUniform = meshShader.GetUniformLocation("model");
		for (int i = 0; i < LevelState.Level.WorldObjects.Count; i++)
		{
			WorldObject worldObject = LevelState.Level.WorldObjects[i];
			Gl.UniformMatrix4x4(modelUniform, worldObject.GetModelMatrix());

			MeshContainer.Entry? mesh = MeshContainer.GetMesh(worldObject.Mesh);
			if (mesh == null)
				continue;

			uint? textureId = TextureContainer.GetTexture(worldObject.Texture);
			if (textureId == null)
				continue;

			Gl.BindTexture(TextureTarget.Texture2D, textureId.Value);

			Gl.BindVertexArray(mesh.MeshVao);
			fixed (uint* index = &mesh.Mesh.Indices[0])
				Gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Mesh.Indices.Length, DrawElementsType.UnsignedInt, index);
		}
	}

	private static void RenderEntities(ShaderCacheEntry lineShader)
	{
		int modelUniform = lineShader.GetUniformLocation("model");
		int colorUniform = lineShader.GetUniformLocation("color");
		for (int i = 0; i < LevelState.Level.Entities.Count; i++)
		{
			Entity entity = LevelState.Level.Entities[i];

			float timeAddition = MathF.Sin((float)Glfw.GetTime() * 10) * 0.5f + 0.5f;
			timeAddition *= 0.5f;

			Vector4 color;
			if (entity == LevelEditorState.SelectedEntity && entity == LevelEditorState.HighlightedEntity && Camera3d.Mode == CameraMode.None)
				color = new(0.5f + timeAddition, 1, 0.5f + timeAddition, 1);
			else if (entity == LevelEditorState.SelectedEntity)
				color = new(0, 0.75f, 0, 1);
			else if (entity == LevelEditorState.HighlightedEntity && Camera3d.Mode == CameraMode.None)
				color = new(1, 0.5f + timeAddition, 1, 1);
			else
				color = new(0.75f, 0, 0.75f, 1);

			if (entity.Shape is Aabb aabb)
			{
				Vector3 size = aabb.Max - aabb.Min;
				Vector3 center = (aabb.Max + aabb.Min) / 2;
				Gl.UniformMatrix4x4(modelUniform, Matrix4x4.CreateScale(size) * Matrix4x4.CreateTranslation(entity.Position + center));
				Gl.UniformVector4(colorUniform, color);
				Gl.BindVertexArray(_cubeVao);
				Gl.DrawArrays(PrimitiveType.Lines, 0, 24);
			}
			else if (entity.Shape is Sphere sphere)
			{
				Gl.UniformMatrix4x4(modelUniform, Matrix4x4.CreateScale(sphere.Radius) * Matrix4x4.CreateTranslation(entity.Position));
				Gl.UniformVector4(colorUniform, color);
				Gl.BindVertexArray(_sphereVao);
				Gl.DrawArrays(PrimitiveType.Lines, 0, (uint)_sphereVertices.Length);
			}
			else
			{
				Gl.UniformMatrix4x4(modelUniform, Matrix4x4.CreateScale(PointScale) * Matrix4x4.CreateTranslation(entity.Position));
				Gl.UniformVector4(colorUniform, color);
				Gl.BindVertexArray(_pointVao);
				Gl.DrawArrays(PrimitiveType.Lines, 0, (uint)_pointVertices.Length);
			}
		}
	}
}
