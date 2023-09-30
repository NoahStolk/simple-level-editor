using SimpleLevelEditor.Model.Enums;

namespace SimpleLevelEditor.Utils;

public static class EnumUtils
{
	public static IReadOnlyList<WorldObjectValues> WorldObjectValuesArray { get; } = Enum.GetValues<WorldObjectValues>();
	public static IReadOnlyDictionary<WorldObjectValues, string> WorldObjectValuesNames { get; } = WorldObjectValuesArray.ToDictionary(wov => wov, wov => wov.ToString());
}
