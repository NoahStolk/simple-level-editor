using Silk.NET.OpenGL;

namespace SimpleLevelEditorV2.Rendering.Scene;

public sealed class MeshRenderer
{
	private readonly GL _gl;
	private readonly ShaderCacheEntry _meshShader;
	private readonly int _modelUniform;

	public MeshRenderer(GL gl)
	{
		_gl = gl;
		_meshShader = InternalContentState.Shaders["Mesh"];
		_modelUniform = _meshShader.GetUniformLocation(_gl, "model");
	}

	public void Render()
	{
		_gl.UseProgram(_meshShader.Id);

		_gl.UniformMatrix4x4(_meshShader.GetUniformLocation(_gl, "view"), Camera3d.ViewMatrix);
		_gl.UniformMatrix4x4(_meshShader.GetUniformLocation(_gl, "projection"), Camera3d.Projection);

		// RenderMeshEntities();
		//
		// if (LevelEditorState.ShouldRenderWorldObjects)
		// 	RenderWorldObjects();
	}

	// private void RenderMeshEntities()
	// {
	// 	for (int i = 0; i < LevelState.Level.Entities.Count; i++)
	// 	{
	// 		Entity entity = LevelState.Level.Entities[i];
	// 		if (!LevelEditorState.ShouldRenderEntity(entity))
	// 			continue;
	//
	// 		EntityShapeDescriptor? entityShapeDescriptor = EntityConfigState.GetEntityShapeDescriptor(entity);
	// 		if (entityShapeDescriptor is EntityShapeDescriptor.Point { Visualization: PointEntityVisualization.Model modelVisualization })
	// 		{
	// 			Model? model = ModelContainer.EntityConfigContainer.GetModel(modelVisualization.ModelPath);
	// 			if (model != null)
	// 				RenderModel(model, Matrix4x4.CreateTranslation(entity.Position));
	// 		}
	// 	}
	//
	// 	if (LevelEditorState.MoveTargetPosition.HasValue && LevelEditorState.SelectedEntity != null)
	// 	{
	// 		Entity selectedEntity = LevelEditorState.SelectedEntity;
	// 		EntityShapeDescriptor? entityShapeDescriptor = EntityConfigState.GetEntityShapeDescriptor(selectedEntity);
	// 		if (entityShapeDescriptor is EntityShapeDescriptor.Point { Visualization: PointEntityVisualization.Model modelVisualization })
	// 		{
	// 			Model? model = ModelContainer.EntityConfigContainer.GetModel(modelVisualization.ModelPath);
	// 			if (model != null)
	// 				RenderModel(model, Matrix4x4.CreateTranslation(LevelEditorState.MoveTargetPosition.Value));
	// 		}
	// 	}
	// }

	// private unsafe void RenderModel(GL gl, Model model, Matrix4x4 modelMatrix)
	// {
	// 	gl.UniformMatrix4x4(_modelUniform, modelMatrix);
	//
	// 	for (int i = 0; i < model.Meshes.Count; i++)
	// 	{
	// 		Mesh mesh = model.Meshes[i];
	//
	// 		Material? materialData = model.GetMaterial(mesh.MaterialName);
	// 		if (materialData == null)
	// 			continue;
	//
	// 		uint textureId = TextureContainer.GetTexture(gl, materialData.DiffuseMap.TextureData);
	// 		gl.BindTexture(TextureTarget.Texture2D, textureId);
	//
	// 		gl.BindVertexArray(mesh.MeshVao);
	// 		fixed (uint* index = &mesh.Geometry.Indices[0])
	// 			gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Geometry.Indices.Length, DrawElementsType.UnsignedInt, index);
	// 	}
	// }
}
