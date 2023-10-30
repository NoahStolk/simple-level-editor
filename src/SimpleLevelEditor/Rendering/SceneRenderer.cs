using Detach.Utils;
using Silk.NET.OpenGL;
using SimpleLevelEditor.Content;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;
using static SimpleLevelEditor.Graphics;

namespace SimpleLevelEditor.Rendering;

public static class SceneRenderer
{
	private static readonly uint _lineVao = VaoUtils.CreateLineVao(new[] { Vector3.Zero, Vector3.UnitZ });
	private static readonly uint _cubeVao = VaoUtils.CreateLineVao(new Vector3[]
	{
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
	});

	public static void RenderScene()
	{
		ShaderCacheEntry lineShader = InternalContent.Shaders["Line"];
		Gl.UseProgram(lineShader.Id);

		Gl.UniformMatrix4x4(lineShader.GetUniformLocation("view"), Camera3d.ViewMatrix);
		Gl.UniformMatrix4x4(lineShader.GetUniformLocation("projection"), Camera3d.Projection);

		RenderOrigin(lineShader);
		RenderGrid(lineShader);
		RenderBoundingBoxes(lineShader);

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
		Gl.DrawArrays(PrimitiveType.Lines, 0, 6);

		// Y axis
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 1.5f));
		Gl.UniformVector4(lineColorUniform, new Vector4(0, 1, 0, 1));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 6);

		// Z axis
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix);
		Gl.UniformVector4(lineColorUniform, new Vector4(0, 0, 1, 1));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 6);

		Gl.LineWidth(2);

		// X axis (negative)
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, -MathF.PI / 2));
		Gl.UniformVector4(lineColorUniform, new Vector4(1, 0, 0, 0.5f));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 6);

		// Y axis (negative)
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI / 2));
		Gl.UniformVector4(lineColorUniform, new Vector4(0, 1, 0, 0.5f));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 6);

		// Z axis (negative)
		Gl.UniformMatrix4x4(lineModelUniform, scaleMatrix * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI));
		Gl.UniformVector4(lineColorUniform, new Vector4(0, 0, 1, 0.5f));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 6);
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
				Gl.DrawArrays(PrimitiveType.Lines, 0, 6);
			}

			if (LevelEditorState.TargetHeight != 0 || i * LevelEditorState.GridCellSize + offset.Z != 0)
			{
				Gl.UniformMatrix4x4(lineModelUniform, scaleMat * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2) * Matrix4x4.CreateTranslation(new Vector3(min * LevelEditorState.GridCellSize, LevelEditorState.TargetHeight, i * LevelEditorState.GridCellSize) + offset));
				Gl.DrawArrays(PrimitiveType.Lines, 0, 6);
			}
		}
	}

	private static void RenderBoundingBoxes(ShaderCacheEntry lineShader)
	{
		Gl.BindVertexArray(_cubeVao);
		Gl.LineWidth(2);

		for (int i = 0; i < LevelState.Level.WorldObjects.Count; i++)
		{
			WorldObject worldObject = LevelState.Level.WorldObjects[i];

			if (worldObject != LevelEditorState.SelectedWorldObject && !LevelEditorState.RenderBoundingBoxes)
				continue;

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
				color = new(0.75f, 0, 0.75f, 1);

			RenderBoundingBox(lineShader, worldObject, color);
		}
	}

	private static void RenderBoundingBox(ShaderCacheEntry lineShader, WorldObject worldObject, Vector4 color)
	{
		MeshContainer.Entry? mesh = MeshContainer.GetMesh(worldObject.Mesh);
		if (mesh == null)
			return;

		int lineModelUniform = lineShader.GetUniformLocation("model");
		int lineColorUniform = lineShader.GetUniformLocation("color");

		Gl.UniformVector4(lineColorUniform, color);

		Vector3 bbScale = worldObject.Scale * (mesh.BoundingMax - mesh.BoundingMin);
		Vector3 bbOffset = (mesh.BoundingMax + mesh.BoundingMin) / 2;
		Matrix4x4 rotationMatrix = MathUtils.CreateRotationMatrixFromEulerAngles(MathUtils.ToRadians(worldObject.Rotation));

		Matrix4x4 modelMatrix = Matrix4x4.CreateScale(bbScale) * rotationMatrix * Matrix4x4.CreateTranslation(worldObject.Position + Vector3.Transform(bbOffset, rotationMatrix));
		Gl.UniformMatrix4x4(lineModelUniform, modelMatrix);
		Gl.DrawArrays(PrimitiveType.Lines, 0, 24);
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

			Gl.BindVertexArray(mesh.Vao);
			fixed (uint* index = &mesh.Mesh.Indices[0])
				Gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Mesh.Indices.Length, DrawElementsType.UnsignedInt, index);
		}
	}
}
