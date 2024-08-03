using System.Numerics;

namespace SimpleLevelEditor.State.Models;

public record Model(string AbsolutePath, Dictionary<string, MaterialLibrary> MaterialLibraries, List<Mesh> Meshes, Vector3 BoundingSphereOrigin, float BoundingSphereRadius)
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
