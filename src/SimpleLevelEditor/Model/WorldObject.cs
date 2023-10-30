using Detach.Utils;
using SimpleLevelEditor.Model.Enums;

namespace SimpleLevelEditor.Model;

public record WorldObject
{
	/// <summary>
	/// The Id is only used to keep track of the object in the editor.
	/// </summary>
	public required int Id;
	public required string Mesh;
	public required string Texture;
	public required string BoundingMesh;
	public required Vector3 Scale;
	public required Vector3 Rotation;
	public required Vector3 Position;
	public required WorldObjectValues Values;

	public WorldObject DeepCopy()
	{
		return new()
		{
			Id = Id,
			Mesh = Mesh,
			Position = Position,
			Scale = Scale,
			Rotation = Rotation,
			Texture = Texture,
			BoundingMesh = BoundingMesh,
			Values = Values,
		};
	}

	public Matrix4x4 GetModelMatrix()
	{
		return Matrix4x4.CreateScale(Scale) * MathUtils.CreateRotationMatrixFromEulerAngles(Rotation) * Matrix4x4.CreateTranslation(Position);
	}
}
