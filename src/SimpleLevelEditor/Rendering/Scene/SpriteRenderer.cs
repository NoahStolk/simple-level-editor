using Silk.NET.OpenGL;
using SimpleLevelEditor.Content;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.Formats.Types.EntityConfig;
using SimpleLevelEditor.Formats.Types.Level;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;
using static SimpleLevelEditor.Graphics;

namespace SimpleLevelEditor.Rendering.Scene;

public static class SpriteRenderer
{
	private static readonly uint _planeVao = VaoUtils.CreatePlaneVao([
		-0.5f, -0.5f, 0, 0, 0,
		-0.5f, 0.5f, 0, 0, 1,
		0.5f, -0.5f, 0, 1, 0,
		0.5f, 0.5f, 0, 1, 1,
	]);
	private static readonly uint[] _planeIndices = [0, 1, 2, 2, 1, 3];

	public static void Render()
	{
		ShaderCacheEntry spriteShader = InternalContent.Shaders["Sprite"];
		Gl.UseProgram(spriteShader.Id);

		Gl.UniformMatrix4x4(spriteShader.GetUniformLocation("view"), Camera3d.ViewMatrix);
		Gl.UniformMatrix4x4(spriteShader.GetUniformLocation("projection"), Camera3d.Projection);

		RenderEntitiesWithSpriteShader(spriteShader);
	}

	private static unsafe void RenderEntitiesWithSpriteShader(ShaderCacheEntry spriteShader)
	{
		int modelUniform = spriteShader.GetUniformLocation("model");
		for (int i = 0; i < LevelState.Level.Entities.Length; i++)
		{
			Entity entity = LevelState.Level.Entities[i];
			if (!LevelEditorState.ShouldRenderEntity(entity))
				continue;

			EntityShape? entityShape = EntityConfigState.GetEntityShape(entity);
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
}
