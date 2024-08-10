using GameEntityConfig.Core;
using System.Reflection;

namespace GameEntityConfig.Editor.States;

public sealed class GameEntityConfigBuilderState
{
	public string Name = string.Empty;
	public List<string> ModelPaths = [];
	public List<string> TexturePaths = [];
	public bool EnableDefaultComponents;
	public List<TypeInfo> ComponentTypes = [];
	public List<EntityDescriptor> EntityDescriptors = [];
}
