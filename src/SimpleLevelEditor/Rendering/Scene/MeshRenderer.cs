using Silk.NET.OpenGL;
using SimpleLevelEditor.Content;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Formats.Types.EntityConfig;
using SimpleLevelEditor.Formats.Types.Level;
using SimpleLevelEditor.State;
using static SimpleLevelEditor.Graphics;

namespace SimpleLevelEditor.Rendering.Scene;

public static class MeshRenderer
{
	public static void Render()
	{
		ShaderCacheEntry meshShader = InternalContent.Shaders["Mesh"];
		Gl.UseProgram(meshShader.Id);

		Gl.UniformMatrix4x4(meshShader.GetUniformLocation("view"), Camera3d.ViewMatrix);
		Gl.UniformMatrix4x4(meshShader.GetUniformLocation("projection"), Camera3d.Projection);

		RenderEntitiesWithMeshShader(meshShader);

		if (LevelEditorState.ShouldRenderWorldObjects())
			RenderWorldObjects(meshShader);
	}

	private static unsafe void RenderWorldObjects(ShaderCacheEntry meshShader)
	{
		int modelUniform = meshShader.GetUniformLocation("model");
		for (int i = 0; i < LevelState.Level.WorldObjects.Length; i++)
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

	private static unsafe void RenderEntitiesWithMeshShader(ShaderCacheEntry meshShader)
	{
		int modelUniform = meshShader.GetUniformLocation("model");
		for (int i = 0; i < LevelState.Level.Entities.Length; i++)
		{
			Entity entity = LevelState.Level.Entities[i];
			if (!LevelEditorState.ShouldRenderEntity(entity))
				continue;

			EntityShape? entityShape = EntityConfigState.GetEntityShape(entity);
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
}
