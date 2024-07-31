using SimpleLevelEditor.Rendering.Vertices;

namespace SimpleLevelEditor.Rendering;

public record Geometry(PositionTextureNormal[] Vertices, uint[] Indices);
