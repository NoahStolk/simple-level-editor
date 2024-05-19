namespace SimpleLevelEditor.Rendering.Scene;

public static class SceneRenderer
{
	private static readonly LineRenderer _lineRenderer = new();
	private static readonly MeshRenderer _meshRenderer = new();
	private static readonly SpriteRenderer _spriteRenderer = new();

	public static void RenderScene()
	{
		_lineRenderer.Render();
		_meshRenderer.Render();
		_spriteRenderer.Render();
	}
}
