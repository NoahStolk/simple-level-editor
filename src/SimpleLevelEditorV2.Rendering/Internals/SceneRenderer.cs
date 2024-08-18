using Silk.NET.OpenGL;

namespace SimpleLevelEditorV2.Rendering.Internals;

internal sealed class SceneRenderer
{
	private LineRenderer? _lineRenderer;
	private MeshRenderer? _meshRenderer;
	private SpriteRenderer? _spriteRenderer;

	public void RenderScene(GL gl, RenderData renderData)
	{
		_lineRenderer ??= new LineRenderer(gl);
		_meshRenderer ??= new MeshRenderer(gl);
		_spriteRenderer ??= new SpriteRenderer(gl);

		_lineRenderer.Render(renderData);
		_meshRenderer.Render(renderData);
		_spriteRenderer.Render(renderData);
	}
}
