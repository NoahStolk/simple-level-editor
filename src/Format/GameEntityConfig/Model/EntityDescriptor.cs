using System.Text.Json.Serialization;

namespace Format.GameEntityConfig.Model;

public sealed record EntityDescriptor
{
	[JsonConstructor]
	internal EntityDescriptor(string name, IReadOnlyList<FixedComponent> fixedComponents, IReadOnlyList<VaryingComponent> varyingComponents)
	{
		Name = name;
		FixedComponents = fixedComponents;
		VaryingComponents = varyingComponents;
	}

	public string Name { get; }

	public IReadOnlyList<FixedComponent> FixedComponents { get; }

	public IReadOnlyList<VaryingComponent> VaryingComponents { get; }
}
