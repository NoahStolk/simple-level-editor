using SimpleLevelEditor.Model.Enums;
using SimpleLevelEditor.State;

namespace SimpleLevelEditor.Utils;

public static class EnumUtils
{
	public static IReadOnlyList<WorldObjectValues> WorldObjectValuesArray { get; } = Enum.GetValues<WorldObjectValues>();
	public static IReadOnlyDictionary<WorldObjectValues, string> WorldObjectValuesNames { get; } = WorldObjectValuesArray.ToDictionary(wov => wov, wov => wov.ToString());

	public static IReadOnlyList<LevelEditorMode> LevelEditorModeArray { get; } = Enum.GetValues<LevelEditorMode>();
	public static IReadOnlyDictionary<LevelEditorMode, string> LevelEditorModeNames { get; } = LevelEditorModeArray.ToDictionary(lem => lem, lem => lem.ToString());
	public static IReadOnlyDictionary<LevelEditorMode, string> LevelEditorModeShortcuts { get; } = new Dictionary<LevelEditorMode, string>
	{
		[LevelEditorMode.AddWorldObjects] = Shortcuts.AddWorldObjectsMode,
		[LevelEditorMode.EditWorldObjects] = Shortcuts.EditWorldObjectsMode,
	};
}
