using Dunet;

namespace GameEntityConfig.Core.Components;

[Union]
public partial record Visualizer
{
	public sealed partial record Model(string ModelPath);
	public sealed partial record Billboard(string TexturePath);
	public sealed partial record Wireframe(float Thickness, Shape Shape);
}
