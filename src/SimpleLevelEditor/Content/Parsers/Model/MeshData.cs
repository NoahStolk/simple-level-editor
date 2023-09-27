namespace SimpleLevelEditor.Content.Parsers.Model;

public record MeshData(string MaterialName, IReadOnlyList<Face> Faces);
