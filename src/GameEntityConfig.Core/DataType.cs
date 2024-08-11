namespace GameEntityConfig.Core;

public sealed record DataType
{
	public DataType(string name, IReadOnlyList<DataTypeField> fields)
	{
		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Data type name cannot be null or whitespace.", nameof(name));

		HashSet<string> fieldNames = [];
		foreach (string fieldName in fields.Select(f => f.Name))
		{
			if (!fieldNames.Add(fieldName))
				throw new ArgumentException($"Field name '{fieldName}' is not unique.", nameof(fields));
		}

		Name = name;
		Fields = fields;
	}

	public string Name { get; }

	public IReadOnlyList<DataTypeField> Fields { get; }

	public static DataType DiffuseColor { get; } = new("DiffuseColor", new[]
	{
		new DataTypeField("R", new Primitive.U8()),
		new DataTypeField("G", new Primitive.U8()),
		new DataTypeField("B", new Primitive.U8()),
		new DataTypeField("A", new Primitive.U8()),
	});

	public static DataType Position { get; } = new("Position", new[]
	{
		new DataTypeField("X", new Primitive.F32()),
		new DataTypeField("Y", new Primitive.F32()),
		new DataTypeField("Z", new Primitive.F32()),
	});

	public static DataType Rotation { get; } = new("Rotation", new[]
	{
		new DataTypeField("X", new Primitive.F32()),
		new DataTypeField("Y", new Primitive.F32()),
		new DataTypeField("Z", new Primitive.F32()),
	});

	public static DataType Scale { get; } = new("Scale", new[]
	{
		new DataTypeField("X", new Primitive.F32()),
		new DataTypeField("Y", new Primitive.F32()),
		new DataTypeField("Z", new Primitive.F32()),
	});

	public static DataType Model { get; } = new("Model", new[]
	{
		new DataTypeField("ModelPath", new Primitive.Str()),
	});

	public static DataType Billboard { get; } = new("Billboard", new[]
	{
		new DataTypeField("TexturePath", new Primitive.Str()),
	});

	public static DataType Wireframe { get; } = new("Wireframe", new[]
	{
		new DataTypeField("Thickness", new Primitive.F32()),
		new DataTypeField("Shape", new Primitive.Str()),
	});

	public static IReadOnlyList<DataType> DefaultDataTypes { get; } = new[]
	{
		DiffuseColor,
		Position,
		Rotation,
		Scale,
		Model,
		Billboard,
		Wireframe,
	};
}
