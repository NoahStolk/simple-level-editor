using GameEntityConfig.Core;
using GameEntityConfig.Core.Components;
using System.Reflection;

namespace GameEntityConfig;

public sealed class GameEntityConfigBuilder
{
	private readonly List<string> _modelPaths = [];
	private readonly List<string> _texturePaths = [];
	private readonly List<TypeInfo> _componentTypes = [];
	private readonly List<EntityDescriptor> _entityDescriptors = [];

	public string Name { get; private set; } = string.Empty;

	public IReadOnlyList<string> ModelPaths => _modelPaths;
	public IReadOnlyList<string> TexturePaths => _texturePaths;
	public IReadOnlyList<TypeInfo> ComponentTypes => _componentTypes;
	public IReadOnlyList<EntityDescriptor> EntityDescriptors => _entityDescriptors;

	public GameEntityConfigBuilder WithName(string name)
	{
		Name = name;
		return this;
	}

	public GameEntityConfigBuilder WithModelPath(string modelPath)
	{
		if (_modelPaths.Contains(modelPath))
			throw new ArgumentException($"Model path '{modelPath}' already exists.");

		_modelPaths.Add(modelPath);
		return this;
	}

	public GameEntityConfigBuilder WithTexturePath(string texturePath)
	{
		if (_texturePaths.Contains(texturePath))
			throw new ArgumentException($"Texture path '{texturePath}' already exists.");

		_texturePaths.Add(texturePath);
		return this;
	}

	public GameEntityConfigBuilder WithDefaultComponentTypes()
	{
		return WithComponentType<DiffuseColor>()
			.WithComponentType<Position>()
			.WithComponentType<Rotation>()
			.WithComponentType<Scale>()
			.WithComponentType<Shape>()
			.WithComponentType<Visualizer>();
	}

	public GameEntityConfigBuilder WithComponentType<T>()
	{
		return WithComponentType(typeof(T).GetTypeInfo());
	}

	public GameEntityConfigBuilder WithComponentType(TypeInfo type)
	{
		if (_componentTypes.Contains(type))
			throw new ArgumentException($"Component type '{type.Name}' already exists.");

		_componentTypes.Add(type);
		return this;
	}

	public GameEntityConfigBuilder WithEntityDescriptor(EntityDescriptor entityDescriptor)
	{
		if (_entityDescriptors.Any(ed => ed.Name == entityDescriptor.Name))
			throw new ArgumentException($"Entity descriptor '{entityDescriptor.Name}' already exists.");

		if (entityDescriptor.FixedComponents.Any(fc => !_componentTypes.Contains(fc.GetType())))
			throw new ArgumentException($"Entity descriptor '{entityDescriptor.Name}' contains fixed components with unknown types.");

		if (entityDescriptor.VaryingComponents.Any(vc => !_componentTypes.Contains(vc.GetType())))
			throw new ArgumentException($"Entity descriptor '{entityDescriptor.Name}' contains varying components with unknown types.");

		_entityDescriptors.Add(entityDescriptor);
		return this;
	}

	public Core.GameEntityConfig Build()
	{
		return new Core.GameEntityConfig(Name, _modelPaths, _texturePaths, _componentTypes, _entityDescriptors);
	}
}
