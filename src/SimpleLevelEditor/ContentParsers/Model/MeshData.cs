namespace SimpleLevelEditor.ContentParsers.Model;

public record MeshData(string MaterialName, IReadOnlyList<Face> Faces);
