namespace SimpleLevelEditor.Rendering;

public record MeshEntry(Mesh Mesh, Material Material, uint MeshVao, uint[] LineIndices, uint LineVao, Vector3 BoundingMin, Vector3 BoundingMax);
