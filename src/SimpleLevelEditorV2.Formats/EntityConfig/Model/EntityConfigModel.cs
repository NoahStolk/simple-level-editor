using System.Text.Json.Serialization;

namespace SimpleLevelEditorV2.Formats.EntityConfig.Model;

public sealed record EntityConfigModel
{
	[JsonConstructor]
	internal EntityConfigModel(IReadOnlyList<string> modelPaths, IReadOnlyList<string> texturePaths, IReadOnlyList<DataType> dataTypes, IReadOnlyList<EntityDescriptor> entityDescriptors)
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
