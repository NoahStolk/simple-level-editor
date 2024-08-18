using Detach.Parsers.Texture;
using Silk.NET.OpenGL;

namespace SimpleLevelEditorV2.Rendering.Internals;

internal sealed class SpriteRenderer
{
	private readonly GL _gl;

	private readonly uint _planeVao;
	private readonly uint[] _planeIndices = [0, 1, 2, 2, 1, 3];

	private readonly ShaderCacheEntry _spriteShader;
	private readonly int _modelUniform;

	private readonly Dictionary<string, TextureData> _billboardSpriteTextures = new();

	public SpriteRenderer(GL gl)
	{
		_gl = gl;
		_spriteShader = InternalContent.Shaders["Sprite"];
		_modelUniform = _spriteShader.GetUniformLocation(_gl, "model");

		_planeVao = VaoUtils.CreatePlaneVao(_gl, [
			-0.5f, -0.5f, 0, 0, 0,
			-0.5f, 0.5f, 0, 0, 1,
			0.5f, -0.5f, 0, 1, 0,
			0.5f, 0.5f, 0, 1, 1,
		]);
	}

	public void Render(RenderData renderData)
	{
		_gl.UseProgram(_spriteShader.Id);

		_gl.UniformMatrix4x4(_spriteShader.GetUniformLocation(_gl, "view"), renderData.View);
		_gl.UniformMatrix4x4(_spriteShader.GetUniformLocation(_gl, "projection"), renderData.Projection);

		RenderSpriteEntities();
	}

	private void RenderSpriteEntities()
	{
		// for (int i = 0; i < LevelState.Level.Entities.Count; i++)
		// {
		// 	Entity entity = LevelState.Level.Entities[i];
		// 	if (!LevelEditorState.ShouldRenderEntity(entity))
		// 		continue;
		//
		// 	RenderSpriteEntity(entity, entity.Position);
		// }
		//
		// if (LevelEditorState.MoveTargetPosition.HasValue && LevelEditorState.SelectedEntity != null)
		// 	RenderSpriteEntity(LevelEditorState.SelectedEntity, LevelEditorState.MoveTargetPosition.Value);
	}

	// private unsafe void RenderSpriteEntity(Entity entity, Vector3 entityPosition)
	// {
	// 	EntityShapeDescriptor? entityShapeDescriptor = EntityConfigState.GetEntityShapeDescriptor(entity);
	// 	if (entityShapeDescriptor is not EntityShapeDescriptor.Point { Visualization: PointEntityVisualization.BillboardSprite billboardSprite })
	// 		return;
	//
	// 	if (LevelState.Level.EntityConfigPath == null)
	// 		return;
	//
	// 	// TODO: Move path handling and reading texture files to a separate class.
	// 	string? levelDirectory = Path.GetDirectoryName(LevelState.LevelFilePath);
	// 	if (levelDirectory == null)
	// 		return;
	//
	// 	string absolutePathToEntityConfig = Path.Combine(levelDirectory, LevelState.Level.EntityConfigPath);
	// 	string? entityConfigDirectory = Path.GetDirectoryName(absolutePathToEntityConfig);
	// 	if (entityConfigDirectory == null)
	// 		return;
	//
	// 	string absolutePathToSpriteTexture = Path.Combine(entityConfigDirectory, billboardSprite.TexturePath);
	// 	if (!_billboardSpriteTextures.TryGetValue(absolutePathToSpriteTexture, out TextureData? textureData))
	// 	{
	// 		textureData = TextureParser.Parse(absolutePathToSpriteTexture);
	// 		if (textureData == null)
	// 			return;
	//
	// 		_billboardSpriteTextures.Add(absolutePathToSpriteTexture, textureData);
	// 	}
	//
	// 	uint textureId = TextureContainer.GetTexture(_gl, textureData);
	// 	_gl.BindTexture(TextureTarget.Texture2D, textureId);
	//
	// 	// Note; keep Z scale at 1 to avoid rendering glitches.
	// 	_gl.UniformMatrix4x4(_modelUniform, Matrix4x4.CreateScale(new Vector3(billboardSprite.Size, billboardSprite.Size, 1)) * EntityMatrixUtils.GetBillboardMatrix(entityPosition));
	//
	// 	_gl.BindVertexArray(_planeVao);
	// 	fixed (uint* indexPtr = &_planeIndices[0])
	// 		_gl.DrawElements(PrimitiveType.Triangles, (uint)_planeIndices.Length, DrawElementsType.UnsignedInt, indexPtr);
	// }
}
