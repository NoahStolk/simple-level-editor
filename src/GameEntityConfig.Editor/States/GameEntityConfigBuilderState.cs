using GameEntityConfig.Core;

namespace GameEntityConfig.Editor.States;

public sealed class GameEntityConfigBuilderState
{
	public string Name = string.Empty;
	public List<string> ModelPaths = [];
	public List<string> TexturePaths = [];
	public bool EnableDefaultComponents;
	public List<DataType> DataTypes = [];
	public List<EntityDescriptor> EntityDescriptors = [];
}
