using Silk.NET.OpenGL;
using System.Numerics;

namespace SimpleLevelEditorV2.Rendering.Internals;

internal sealed class SceneRenderer
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
		Vector3? selectedPosition,
		Matrix4x4 view,
		Matrix4x4 projection,
		Vector3 cameraPosition,
		Vector3 focusPointTarget)
	{
		_lineRenderer ??= new LineRenderer(gl);
		_meshRenderer ??= new MeshRenderer(gl);
		_spriteRenderer ??= new SpriteRenderer(gl);

		_lineRenderer.Render(gridCellFadeOutMinDistance, gridCellFadeOutMaxDistance, moveTargetPosition, targetHeight, gridCellInterval, selectedPosition, view, projection, cameraPosition, focusPointTarget);
		_meshRenderer.Render(view, projection);
		_spriteRenderer.Render(view, projection);
	}
}
