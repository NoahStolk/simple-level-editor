using System.Text.Json.Serialization;

namespace Level.Core;

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
