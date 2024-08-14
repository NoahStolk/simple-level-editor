using System.Text.Json.Serialization;

namespace SimpleLevelEditorV2.Formats.GameEntityConfig.Model;

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
