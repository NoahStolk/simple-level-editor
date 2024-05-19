using Silk.NET.OpenGL;
using SimpleLevelEditor.Content;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Formats.Types.EntityConfig;
using SimpleLevelEditor.Formats.Types.Level;
using SimpleLevelEditor.State;
using static SimpleLevelEditor.Graphics;

namespace SimpleLevelEditor.Rendering.Scene;

public sealed class MeshRenderer
{
	private readonly ShaderCacheEntry _meshShader;
	private readonly int _modelUniform;

	public MeshRenderer()
	{
		_meshShader = InternalContent.Shaders["Mesh"];
		_modelUniform = _meshShader.GetUniformLocation("model");
	}

	public void Render()
	{
		Gl.UseProgram(_meshShader.Id);

		Gl.UniformMatrix4x4(_meshShader.GetUniformLocation("view"), Camera3d.ViewMatrix);
		Gl.UniformMatrix4x4(_meshShader.GetUniformLocation("projection"), Camera3d.Projection);

		RenderMeshEntities();

		if (LevelEditorState.ShouldRenderWorldObjects())
			RenderWorldObjects();
	}

	private void RenderMeshEntities()
	{
		for (int i = 0; i < LevelState.Level.Entities.Length; i++)
		{
			Entity entity = LevelState.Level.Entities[i];
			if (!LevelEditorState.ShouldRenderEntity(entity))
				continue;

			EntityShape? entityShape = EntityConfigState.GetEntityShape(entity);
			if (entityShape is EntityShape.Point { Visualization: PointEntityVisualization.Mesh meshVisualization })
				RenderMesh(meshVisualization.MeshName, meshVisualization.TextureName, Matrix4x4.CreateTranslation(entity.Position));
		}

		if (LevelEditorState.MoveTargetPosition.HasValue && LevelEditorState.SelectedEntity != null)
		{
			Entity selectedEntity = LevelEditorState.SelectedEntity;
			EntityShape? entityShape = EntityConfigState.GetEntityShape(selectedEntity);
			if (entityShape is EntityShape.Point { Visualization: PointEntityVisualization.Mesh meshVisualization })
				RenderMesh(meshVisualization.MeshName, meshVisualization.TextureName, Matrix4x4.CreateTranslation(LevelEditorState.MoveTargetPosition.Value));
		}
	}

	private void RenderWorldObjects()
	{
		for (int i = 0; i < LevelState.Level.WorldObjects.Length; i++)
		{
			WorldObject worldObject = LevelState.Level.WorldObjects[i];
			RenderMesh(worldObject.Mesh, worldObject.Texture, worldObject.GetModelMatrix());
		}

		if (LevelEditorState.MoveTargetPosition.HasValue && LevelEditorState.SelectedWorldObject != null)
		{
			WorldObject selectedWorldObject = LevelEditorState.SelectedWorldObject;
			RenderMesh(selectedWorldObject.Mesh, selectedWorldObject.Texture, selectedWorldObject.GetModelMatrix(LevelEditorState.MoveTargetPosition.Value));
		}
	}

	private unsafe void RenderMesh(string meshName, string textureName, Matrix4x4 modelMatrix)
	{
		MeshEntry? mesh = MeshContainer.GetMesh(meshName);
		if (mesh == null)
			return;

		uint? textureId = TextureContainer.GetTexture(textureName);
		if (textureId == null)
			return;

		Gl.UniformMatrix4x4(_modelUniform, modelMatrix);

		Gl.BindTexture(TextureTarget.Texture2D, textureId.Value);

		Gl.BindVertexArray(mesh.MeshVao);
		fixed (uint* index = &mesh.Mesh.Indices[0])
			Gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Mesh.Indices.Length, DrawElementsType.UnsignedInt, index);
	}
}
