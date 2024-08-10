using GameEntityConfig.Core.Components;
using System.Reflection;

namespace GameEntityConfig.Editor.Utils;

public static class ComponentUtils
{
	private static readonly List<TypeInfo> _defaultComponents =
	[
		typeof(DiffuseColor).GetTypeInfo(),
		typeof(Position).GetTypeInfo(),
		typeof(Rotation).GetTypeInfo(),
		typeof(Scale).GetTypeInfo(),
		typeof(Shape).GetTypeInfo(),
		typeof(Visualizer).GetTypeInfo(),
	];

	public static IReadOnlyList<TypeInfo> DefaultComponents => _defaultComponents;
}
