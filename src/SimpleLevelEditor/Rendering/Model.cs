namespace SimpleLevelEditor.Rendering;

public record Model(string AbsolutePath, Dictionary<string, MaterialLibrary> MaterialLibraries, List<Mesh> Meshes)
{
	public Material? GetMaterial(string materialName)
	{
		foreach (MaterialLibrary kvp in MaterialLibraries.Values)
		{
			for (int i = 0; i < kvp.Materials.Count; i++)
			{
				Material materialData = kvp.Materials[i];
				if (materialData.Name == materialName)
					return materialData;
			}
		}

		return null;
	}
}