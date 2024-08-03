using System.Numerics;

namespace SimpleLevelEditor.State.States.Models;

public record struct PositionTextureNormal(Vector3 Position, Vector2 Texture, Vector3 Normal);
