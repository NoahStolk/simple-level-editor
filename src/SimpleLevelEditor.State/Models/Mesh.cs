using System.Numerics;

namespace SimpleLevelEditor.State.Models;

public record Mesh(Geometry Geometry, string MaterialName, uint MeshVao, uint[] LineIndices, uint LineVao, Vector3 BoundingMin, Vector3 BoundingMax);
