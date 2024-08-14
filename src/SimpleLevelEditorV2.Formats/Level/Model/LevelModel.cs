using System.Text.Json.Serialization;

namespace SimpleLevelEditorV2.Formats.Level.Model;

public sealed record LevelModel
{
	[JsonConstructor]
	public LevelModel(string gameEntityConfigPath, IReadOnlyList<LevelEntity> levelEntities)
	{
		GameEntityConfigPath = gameEntityConfigPath;
		LevelEntities = levelEntities;
	}

	public string GameEntityConfigPath { get; }

	public IReadOnlyList<LevelEntity> LevelEntities { get; }
}
