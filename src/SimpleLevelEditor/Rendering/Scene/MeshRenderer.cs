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
			{
				ModelEntry? model = ModelContainer.GetEntityConfigModel(meshVisualization.MeshName);
				if (model != null)
					RenderModel(model, Matrix4x4.CreateTranslation(entity.Position));
			}
		}

		if (LevelEditorState.MoveTargetPosition.HasValue && LevelEditorState.SelectedEntity != null)
		{
			Entity selectedEntity = LevelEditorState.SelectedEntity;
			EntityShape? entityShape = EntityConfigState.GetEntityShape(selectedEntity);
			if (entityShape is EntityShape.Point { Visualization: PointEntityVisualization.Mesh meshVisualization })
			{
				ModelEntry? model = ModelContainer.GetEntityConfigModel(meshVisualization.MeshName);
				if (model != null)
					RenderModel(model, Matrix4x4.CreateTranslation(LevelEditorState.MoveTargetPosition.Value));
			}
		}
	}

	private void RenderWorldObjects()
	{
		for (int i = 0; i < LevelState.Level.WorldObjects.Length; i++)
		{
			WorldObject worldObject = LevelState.Level.WorldObjects[i];
			ModelEntry? model = ModelContainer.GetLevelModel(worldObject.Mesh);
			if (model != null)
				RenderModel(model, worldObject.GetModelMatrix());
		}

		if (LevelEditorState.MoveTargetPosition.HasValue && LevelEditorState.SelectedWorldObject != null)
		{
			WorldObject selectedWorldObject = LevelEditorState.SelectedWorldObject;
			ModelEntry? model = ModelContainer.GetLevelModel(selectedWorldObject.Mesh);
			if (model != null)
				RenderModel(model, selectedWorldObject.GetModelMatrix(LevelEditorState.MoveTargetPosition.Value));
		}
	}

	private unsafe void RenderModel(ModelEntry model, Matrix4x4 modelMatrix)
	{
		Gl.UniformMatrix4x4(_modelUniform, modelMatrix);

		for (int i = 0; i < model.MeshEntries.Count; i++)
		{
			MeshEntry meshEntry = model.MeshEntries[i];

			uint? textureId = TextureContainer.GetLevelTexture(meshEntry.Material.DiffuseTexturePath);
			if (textureId == null)
				continue;

			Gl.BindTexture(TextureTarget.Texture2D, textureId.Value);

			Gl.BindVertexArray(meshEntry.MeshVao);
			fixed (uint* index = &meshEntry.Mesh.Indices[0])
				Gl.DrawElements(PrimitiveType.Triangles, (uint)meshEntry.Mesh.Indices.Length, DrawElementsType.UnsignedInt, index);
		}
	}
}
