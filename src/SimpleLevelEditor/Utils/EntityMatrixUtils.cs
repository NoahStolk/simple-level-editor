using SimpleLevelEditor.Formats.Level.Model;
using SimpleLevelEditor.Formats.Types.Level;
using SimpleLevelEditor.Rendering;

namespace SimpleLevelEditor.Utils;

public static class EntityMatrixUtils
{
	public static Matrix4x4 GetBillboardMatrix(Entity entity)
	{
		// Negate the camera up vector because rendered textures are flipped vertically.
		return Matrix4x4.CreateBillboard(entity.Position, Camera3d.Position, -Camera3d.UpDirection, Camera3d.LookDirection);
	}
}
