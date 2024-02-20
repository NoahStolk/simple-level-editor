namespace SimpleLevelEditor.Formats.EntityConfig.Model;

public record EntityConfigData
{
	public required int Version;

	public required List<EntityDescriptor> Entities;

	public static EntityConfigData CreateDefault()
	{
		return new()
		{
			Version = 1,
			Entities = [],
		};
	}
}
