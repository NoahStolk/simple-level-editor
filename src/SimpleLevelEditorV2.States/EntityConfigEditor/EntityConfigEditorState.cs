using SimpleLevelEditorV2.Formats.EntityConfig.Model;

namespace SimpleLevelEditorV2.States.EntityConfigEditor;

public sealed class EntityConfigEditorState
{
	public List<string> ModelPaths = [];
	public List<string> TexturePaths = [];
	public List<DataType> DataTypes = [];
	public List<EntityDescriptor> EntityDescriptors = [];

	public bool ShowShortcutsWindow;
}
