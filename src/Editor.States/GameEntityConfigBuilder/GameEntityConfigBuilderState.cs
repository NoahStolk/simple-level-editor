using SimpleLevelEditorV2.Formats.GameEntityConfig.Model;

namespace Editor.States.GameEntityConfigBuilder;

public sealed class GameEntityConfigBuilderState
{
	public List<string> ModelPaths = [];
	public List<string> TexturePaths = [];
	public List<DataType> DataTypes = [];
	public List<EntityDescriptor> EntityDescriptors = [];
}
