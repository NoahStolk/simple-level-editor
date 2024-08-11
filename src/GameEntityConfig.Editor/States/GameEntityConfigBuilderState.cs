using Format.GameEntityConfig.Model;

namespace GameEntityConfig.Editor.States;

public sealed class GameEntityConfigBuilderState
{
	public List<string> ModelPaths = [];
	public List<string> TexturePaths = [];
	public List<DataType> DataTypes = [];
	public List<EntityDescriptor> EntityDescriptors = [];
}
