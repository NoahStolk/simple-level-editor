using Detach.Parsers.Model;

namespace SimpleLevelEditor.Rendering;

public record Model(Dictionary<string, MaterialsData> Materials, List<Mesh> Meshes);
