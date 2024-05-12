using Detach.Utils;
using SimpleLevelEditor.Formats.Types.Level;

namespace SimpleLevelEditor.Extensions;

public static class WorldObjectExtensions
{
	public static Matrix4x4 GetModelMatrix(this WorldObject worldObject)
	{
		return Matrix4x4.CreateScale(worldObject.Scale) * MathUtils.CreateRotationMatrixFromEulerAngles(MathUtils.ToRadians(worldObject.Rotation)) * Matrix4x4.CreateTranslation(worldObject.Position);
	}
}
