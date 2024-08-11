using System.Text.Json.Serialization;

namespace GameEntityConfig.Core;

public sealed record GameEntityConfig
{
	[JsonConstructor]
	internal GameEntityConfig(IReadOnlyList<string> modelPaths, IReadOnlyList<string> texturePaths, IReadOnlyList<DataType> dataTypes, IReadOnlyList<EntityDescriptor> entityDescriptors)
	{
		ModelPaths = modelPaths;
		TexturePaths = texturePaths;
		DataTypes = dataTypes;
		EntityDescriptors = entityDescriptors;
	}

	public IReadOnlyList<string> ModelPaths { get; }

	public IReadOnlyList<string> TexturePaths { get; }

	public IReadOnlyList<DataType> DataTypes { get; }

	public IReadOnlyList<EntityDescriptor> EntityDescriptors { get; }
}
