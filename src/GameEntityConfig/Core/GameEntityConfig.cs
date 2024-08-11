using System.Text.Json.Serialization;

namespace GameEntityConfig.Core;

public sealed record GameEntityConfig
{
	[JsonConstructor]
	internal GameEntityConfig(string name, IReadOnlyList<string> modelPaths, IReadOnlyList<string> texturePaths, IReadOnlyList<DataType> dataTypes, IReadOnlyList<EntityDescriptor> entityDescriptors)
	{
		Name = name;
		ModelPaths = modelPaths;
		TexturePaths = texturePaths;
		DataTypes = dataTypes;
		EntityDescriptors = entityDescriptors;
	}

	public string Name { get; }

	public IReadOnlyList<string> ModelPaths { get; }

	public IReadOnlyList<string> TexturePaths { get; }

	public IReadOnlyList<DataType> DataTypes { get; }

	public IReadOnlyList<EntityDescriptor> EntityDescriptors { get; }
}
