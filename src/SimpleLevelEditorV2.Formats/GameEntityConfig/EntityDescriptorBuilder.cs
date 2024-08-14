using SimpleLevelEditorV2.Formats.GameEntityConfig.Model;

namespace SimpleLevelEditorV2.Formats.GameEntityConfig;

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

	public EntityDescriptorBuilder WithFixedComponent(DataType dataType, string value)
	{
		AssertUniqueComponentType(dataType);

		_fixedComponents.Add(new FixedComponent(dataType, value));
		return this;
	}

	public EntityDescriptorBuilder WithVaryingComponent(DataType dataType, string defaultValue)
	{
		AssertUniqueComponentType(dataType);

		_varyingComponents.Add(new VaryingComponent(dataType, defaultValue, null));
		return this;
	}

	public EntityDescriptorBuilder WithVaryingComponent(DataType dataType, string defaultValue, float step, float min, float max)
	{
		AssertUniqueComponentType(dataType);

		_varyingComponents.Add(new VaryingComponent(dataType, defaultValue, new SliderConfiguration(step, min, max)));
		return this;
	}

	public EntityDescriptor Build()
	{
		return new EntityDescriptor(_name, _fixedComponents, _varyingComponents);
	}

	private void AssertUniqueComponentType(DataType dataType)
	{
		if (_fixedComponents.Exists(fc => fc.DataType.Name == dataType.Name) || _varyingComponents.Exists(vc => vc.DataType.Name == dataType.Name))
			throw new ArgumentException($"Fixed component of type '{dataType.Name}' already exists.");
	}
}
