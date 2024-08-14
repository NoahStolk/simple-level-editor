using Silk.NET.OpenGL;
using System.Numerics;

namespace SimpleLevelEditorV2.Rendering.Scene;

public sealed class SceneRenderer
{
	private LineRenderer? _lineRenderer;
	private MeshRenderer? _meshRenderer;
	private SpriteRenderer? _spriteRenderer;

	public void RenderScene(
		GL gl,
		float gridCellFadeOutMinDistance,
		float gridCellFadeOutMaxDistance,
		Vector3? moveTargetPosition,
		float targetHeight,
		int gridCellInterval,
		Vector3? selectedPosition)
	{
		_lineRenderer ??= new LineRenderer(gl);
		_meshRenderer ??= new MeshRenderer(gl);
		_spriteRenderer ??= new SpriteRenderer(gl);

		_lineRenderer.Render(gridCellFadeOutMinDistance, gridCellFadeOutMaxDistance, moveTargetPosition, targetHeight, gridCellInterval, selectedPosition);
		_meshRenderer.Render();
		_spriteRenderer.Render();
	}
}
