using SimpleLevelEditor.Rendering;

namespace SimpleLevelEditor.Utils;

public static class EntityMatrixUtils
{
	public static Matrix4x4 GetBillboardMatrix(Vector3 entityPosition)
	{
		// Negate the camera up vector because rendered textures are flipped vertically.
		return Matrix4x4.CreateBillboard(entityPosition, Camera3d.Position, -Camera3d.UpDirection, Camera3d.LookDirection);
	}
}
