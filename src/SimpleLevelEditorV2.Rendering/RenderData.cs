using System.Numerics;

namespace SimpleLevelEditorV2.Rendering;

public readonly record struct RenderData(
	Vector2 Size,
	float GridCellFadeOutMinDistance,
	float GridCellFadeOutMaxDistance,
	Vector3? MoveTargetPosition,
	float TargetHeight,
	int GridCellInterval,
	Vector3? SelectedPosition,
	Matrix4x4 View,
	Matrix4x4 Projection,
	Vector3 CameraPosition,
	Vector3 FocusPointTarget);
