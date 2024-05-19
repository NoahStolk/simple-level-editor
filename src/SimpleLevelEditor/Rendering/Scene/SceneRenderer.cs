namespace SimpleLevelEditor.Rendering.Scene;

public static class SceneRenderer
{
	public static void RenderScene()
	{
		LineRenderer.Render();
		MeshRenderer.Render();
		SpriteRenderer.Render();
	}
}
