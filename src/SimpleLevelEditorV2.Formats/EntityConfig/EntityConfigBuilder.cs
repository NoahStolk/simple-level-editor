using SimpleLevelEditorV2.Formats.EntityConfig.Model;

namespace SimpleLevelEditorV2.Formats.EntityConfig;

public sealed class EntityConfigBuilder
{
	private readonly List<string> _modelPaths = [];
	private readonly List<string> _texturePaths = [];
	private readonly List<DataType> _dataTypes = [];
	private readonly List<EntityDescriptor> _entityDescriptors = [];

	public EntityConfigBuilder WithModelPath(string modelPath)
	{
		if (_modelPaths.Contains(modelPath))
			throw new ArgumentException($"Model path '{modelPath}' already exists.");

		_modelPaths.Add(modelPath);
		return this;
	}

	public EntityConfigBuilder WithTexturePath(string texturePath)
	{
		if (_texturePaths.Contains(texturePath))
			throw new ArgumentException($"Texture path '{texturePath}' already exists.");

		_texturePaths.Add(texturePath);
		return this;
	}

	public EntityConfigBuilder WithDefaultDataTypes()
	{
		return WithDataType(DataType.DiffuseColor)
			.WithDataType(DataType.Position)
			.WithDataType(DataType.Rotation)
			.WithDataType(DataType.Scale)
			.WithDataType(DataType.Model)
			.WithDataType(DataType.Billboard)
			.WithDataType(DataType.Wireframe);
	}

	public EntityConfigBuilder WithDataType(DataType dataType)
	{
		if (_dataTypes.Contains(dataType))
			throw new ArgumentException($"Component type '{dataType}' already exists.");

		_dataTypes.Add(dataType);
		return this;
	}

	public EntityConfigBuilder WithEntityDescriptor(EntityDescriptor entityDescriptor)
	{
		if (_entityDescriptors.Exists(ed => ed.Name == entityDescriptor.Name))
			throw new ArgumentException($"Entity descriptor '{entityDescriptor.Name}' already exists.");

		foreach (FixedComponent fixedComponent in entityDescriptor.FixedComponents)
			ValidateDataType(fixedComponent.DataType);

		foreach (VaryingComponent varyingComponent in entityDescriptor.VaryingComponents)
			ValidateDataType(varyingComponent.DataType);

		_entityDescriptors.Add(entityDescriptor);
		return this;

		void ValidateDataType(DataType dataType)
		{
			if (!_dataTypes.Exists(dt => dt.Name == dataType.Name))
				throw new ArgumentException($"Entity descriptor '{entityDescriptor.Name}' contains component of type '{dataType.Name}' which is not registered.");
		}
	}

	public EntityConfigModel Build()
	{
		return new EntityConfigModel(_modelPaths, _texturePaths, _dataTypes, _entityDescriptors);
	}
}
