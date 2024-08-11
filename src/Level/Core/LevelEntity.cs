using System.Text.Json.Serialization;

namespace Level.Core;

public sealed record LevelEntity
{
	[JsonConstructor]
	public LevelEntity(string entityDescriptorName, Dictionary<string, string> data)
	{
		EntityDescriptorName = entityDescriptorName;
		Data = data;
	}

	public string EntityDescriptorName { get; }

	public Dictionary<string, string> Data { get; }
}
