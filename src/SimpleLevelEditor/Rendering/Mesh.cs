namespace SimpleLevelEditor.Rendering;

public record Mesh(Geometry Geometry, Material Material, uint MeshVao, uint[] LineIndices, uint LineVao, Vector3 BoundingMin, Vector3 BoundingMax);
