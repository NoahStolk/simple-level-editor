using System.Text.Json.Serialization;

namespace Format.GameEntityConfig.Model;

public sealed record DataTypeField
{
	[JsonConstructor]
	public DataTypeField(string name, Primitive primitive)
	{
		Name = name;
		Primitive = primitive;
	}

	public string Name { get; }

	public Primitive Primitive { get; }
}
