namespace SimpleLevelEditor.State;

public static class DebugState
{
	private static readonly Dictionary<string, int> _warnings = new();

	public static IReadOnlyDictionary<string, int> Warnings => _warnings;

	public static void ClearWarnings()
	{
		_warnings.Clear();
	}

	public static void AddWarning(string warning)
	{
		if (_warnings.TryGetValue(warning, out int count))
			_warnings[warning] = count + 1;
		else
			_warnings.Add(warning, 1);
	}
}
