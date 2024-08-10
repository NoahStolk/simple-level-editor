using System.Reflection;

namespace GameEntityConfig.Core;

public sealed record GameEntityConfig
{
	internal GameEntityConfig(string name, IReadOnlyList<string> modelPaths, IReadOnlyList<string> texturePaths, IReadOnlyList<TypeInfo> componentTypes, IReadOnlyList<EntityDescriptor> entityDescriptors)
	{
		Name = name;
		ModelPaths = modelPaths;
		TexturePaths = texturePaths;
		ComponentTypes = componentTypes;
		EntityDescriptors = entityDescriptors;
	}

	public string Name { get; }

	public IReadOnlyList<string> ModelPaths { get; }

	public IReadOnlyList<string> TexturePaths { get; }

	public IReadOnlyList<TypeInfo> ComponentTypes { get; }

	public IReadOnlyList<EntityDescriptor> EntityDescriptors { get; }
}
