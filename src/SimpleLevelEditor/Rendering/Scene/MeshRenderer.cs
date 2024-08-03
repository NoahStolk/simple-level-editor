using Silk.NET.OpenGL;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Formats.Types.EntityConfig;
using SimpleLevelEditor.Formats.Types.Level;
using SimpleLevelEditor.State;
using SimpleLevelEditor.State.Extensions;
using SimpleLevelEditor.State.Level;
using SimpleLevelEditor.State.Models;
using static SimpleLevelEditor.Graphics;

namespace SimpleLevelEditor.Rendering.Scene;

public sealed class MeshRenderer
{
	private readonly ShaderCacheEntry _meshShader;
	private readonly int _modelUniform;

	public MeshRenderer()
	{
		_meshShader = InternalContent.Shaders["Mesh"];
		_modelUniform = _meshShader.GetUniformLocation(Gl, "model");
	}

	public void Render()
	{
		Gl.UseProgram(_meshShader.Id);

		Gl.UniformMatrix4x4(_meshShader.GetUniformLocation(Gl, "view"), Camera3d.ViewMatrix);
		Gl.UniformMatrix4x4(_meshShader.GetUniformLocation(Gl, "projection"), Camera3d.Projection);

		RenderMeshEntities();

		if (LevelEditorState.ShouldRenderWorldObjects)
			RenderWorldObjects();
	}

	private void RenderMeshEntities()
	{
		for (int i = 0; i < LevelState.Level.Entities.Length; i++)
		{
			Entity entity = LevelState.Level.Entities[i];
			if (!LevelEditorState.ShouldRenderEntity(entity))
				continue;

			EntityShapeDescriptor? entityShapeDescriptor = EntityConfigState.GetEntityShapeDescriptor(entity);
			if (entityShapeDescriptor is EntityShapeDescriptor.Point { Visualization: PointEntityVisualization.Model modelVisualization })
			{
				Model? model = ModelContainer.EntityConfigContainer.GetModel(modelVisualization.ModelPath);
				if (model != null)
					RenderModel(model, Matrix4x4.CreateTranslation(entity.Position));
			}
		}

		if (LevelEditorState.MoveTargetPosition.HasValue && LevelEditorState.SelectedEntity != null)
		{
			Entity selectedEntity = LevelEditorState.SelectedEntity;
			EntityShapeDescriptor? entityShapeDescriptor = EntityConfigState.GetEntityShapeDescriptor(selectedEntity);
			if (entityShapeDescriptor is EntityShapeDescriptor.Point { Visualization: PointEntityVisualization.Model modelVisualization })
			{
				Model? model = ModelContainer.EntityConfigContainer.GetModel(modelVisualization.ModelPath);
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
			Model? model = ModelContainer.LevelContainer.GetModel(worldObject.ModelPath);
			if (model != null)
				RenderModel(model, worldObject.GetModelMatrix());
		}

		if (LevelEditorState.MoveTargetPosition.HasValue && LevelEditorState.SelectedWorldObject != null)
		{
			WorldObject selectedWorldObject = LevelEditorState.SelectedWorldObject;
			Model? model = ModelContainer.LevelContainer.GetModel(selectedWorldObject.ModelPath);
			if (model != null)
				RenderModel(model, selectedWorldObject.GetModelMatrix(LevelEditorState.MoveTargetPosition.Value));
		}
	}

	private unsafe void RenderModel(Model model, Matrix4x4 modelMatrix)
	{
		Gl.UniformMatrix4x4(_modelUniform, modelMatrix);

		for (int i = 0; i < model.Meshes.Count; i++)
		{
			Mesh mesh = model.Meshes[i];

			Material? materialData = model.GetMaterial(mesh.MaterialName);
			if (materialData == null)
				continue;

			uint textureId = TextureContainer.GetTexture(Gl, materialData.DiffuseMap.TextureData);
			Gl.BindTexture(TextureTarget.Texture2D, textureId);

			Gl.BindVertexArray(mesh.MeshVao);
			fixed (uint* index = &mesh.Geometry.Indices[0])
				Gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Geometry.Indices.Length, DrawElementsType.UnsignedInt, index);
		}
	}
}
