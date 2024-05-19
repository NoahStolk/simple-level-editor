namespace SimpleLevelEditor.Rendering.Scene;

public static class SceneRenderer
{
	private static readonly LineRenderer _lineRenderer = new();

	public static void RenderScene()
	{
		_lineRenderer.Render();
		MeshRenderer.Render();
		SpriteRenderer.Render();
	}
}
