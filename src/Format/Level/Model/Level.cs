using System.Text.Json.Serialization;

namespace Format.Level.Model;

public sealed record Level
{
	[JsonConstructor]
	public Level(string gameEntityConfigPath, IReadOnlyList<LevelEntity> levelEntities)
	{
		GameEntityConfigPath = gameEntityConfigPath;
		LevelEntities = levelEntities;
	}

	public string GameEntityConfigPath { get; }

	public IReadOnlyList<LevelEntity> LevelEntities { get; }
}
