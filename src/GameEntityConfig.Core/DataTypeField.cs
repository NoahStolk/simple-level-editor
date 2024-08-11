namespace GameEntityConfig.Core;

public sealed record DataTypeField
{
	public DataTypeField(string name, Primitive primitive)
	{
		Name = name;
		Primitive = primitive;
	}

	public string Name { get; }

	public Primitive Primitive { get; }
}
