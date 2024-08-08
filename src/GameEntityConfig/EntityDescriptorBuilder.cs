using GameEntityConfig.Core;
using System.Numerics;

namespace GameEntityConfig;

public sealed class EntityDescriptorBuilder
{
	private string _name = string.Empty;
	private readonly List<FixedComponent> _fixedComponents = [];
	private readonly List<VaryingComponent> _varyingComponents = [];

	public EntityDescriptorBuilder WithName(string name)
	{
		_name = name;
		return this;
	}

	public EntityDescriptorBuilder WithFixedComponent<T>(T value)
	{
		AssertUniqueComponentType(typeof(T));

		_fixedComponents.Add(new FixedComponent<T>(value));
		return this;
	}

	public EntityDescriptorBuilder WithVaryingComponent<T>(T defaultValue)
	{
		AssertUniqueComponentType(typeof(T));

		_varyingComponents.Add(new VaryingComponent<T>(defaultValue));
		return this;
	}

	public EntityDescriptorBuilder WithVaryingComponent<T, TSlider>(T defaultValue, TSlider step, TSlider min, TSlider max)
		where TSlider : struct, INumber<TSlider>
	{
		AssertUniqueComponentType(typeof(T));

		_varyingComponents.Add(new VaryingComponent<T, TSlider>(defaultValue, new SliderConfiguration<TSlider>(step, min, max)));
		return this;
	}

	public EntityDescriptor Build()
	{
		return new EntityDescriptor(_name, _fixedComponents, _varyingComponents);
	}

	private void AssertUniqueComponentType(Type newType)
	{
		if (_fixedComponents.Any(fc => fc.GetType() == newType) || _varyingComponents.Any(vc => vc.GetType() == newType))
			throw new ArgumentException($"Fixed component of type '{newType.Name}' already exists.");
	}
}
