using Silk.NET.OpenGL;
using SimpleLevelEditor.Content;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Formats.Types.EntityConfig;
using SimpleLevelEditor.Formats.Types.Level;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;
using static SimpleLevelEditor.Graphics;

namespace SimpleLevelEditor.Rendering.Scene;

public sealed class SpriteRenderer
{
	private static readonly uint _planeVao = VaoUtils.CreatePlaneVao([
		-0.5f, -0.5f, 0, 0, 0,
		-0.5f, 0.5f, 0, 0, 1,
		0.5f, -0.5f, 0, 1, 0,
		0.5f, 0.5f, 0, 1, 1,
	]);
	private static readonly uint[] _planeIndices = [0, 1, 2, 2, 1, 3];

	private readonly ShaderCacheEntry _spriteShader;
	private readonly int _modelUniform;

	public SpriteRenderer()
	{
		_spriteShader = InternalContent.Shaders["Sprite"];
		_modelUniform = _spriteShader.GetUniformLocation("model");
	}

	public void Render()
	{
		Gl.UseProgram(_spriteShader.Id);

		Gl.UniformMatrix4x4(_spriteShader.GetUniformLocation("view"), Camera3d.ViewMatrix);
		Gl.UniformMatrix4x4(_spriteShader.GetUniformLocation("projection"), Camera3d.Projection);

		RenderSpriteEntities();
	}

	private void RenderSpriteEntities()
	{
		for (int i = 0; i < LevelState.Level.Entities.Length; i++)
		{
			Entity entity = LevelState.Level.Entities[i];
			if (!LevelEditorState.ShouldRenderEntity(entity))
				continue;

			RenderSpriteEntity(entity, entity.Position);
		}

		if (LevelEditorState.MoveTargetPosition.HasValue && LevelEditorState.SelectedEntity != null)
			RenderSpriteEntity(LevelEditorState.SelectedEntity, LevelEditorState.MoveTargetPosition.Value);
	}

	private unsafe void RenderSpriteEntity(Entity entity, Vector3 entityPosition)
	{
		EntityShape? entityShape = EntityConfigState.GetEntityShape(entity);
		if (entityShape is not EntityShape.Point { Visualization: PointEntityVisualization.BillboardSprite billboardSprite })
			return;

		uint? textureId = TextureContainer.GetTexture(billboardSprite.TextureName);
		if (textureId == null)
			return;

		Gl.UniformMatrix4x4(_modelUniform, Matrix4x4.CreateScale(new Vector3(billboardSprite.Size, billboardSprite.Size, 1)) * EntityMatrixUtils.GetBillboardMatrix(entityPosition));

		Gl.BindTexture(TextureTarget.Texture2D, textureId.Value);

		Gl.BindVertexArray(_planeVao);
		fixed (uint* indexPtr = &_planeIndices[0])
			Gl.DrawElements(PrimitiveType.Triangles, (uint)_planeIndices.Length, DrawElementsType.UnsignedInt, indexPtr);
	}
}
