using System.Text.Json.Serialization;

namespace SimpleLevelEditorV2.Formats.Level.Model;

public sealed record LevelModel
{
	[JsonConstructor]
	public LevelModel(string entityConfigPath, IReadOnlyList<LevelEntity> levelEntities)
	{
		EntityConfigPath = entityConfigPath;
		LevelEntities = levelEntities;
	}

	public string EntityConfigPath { get; }

	public IReadOnlyList<LevelEntity> LevelEntities { get; }
}
