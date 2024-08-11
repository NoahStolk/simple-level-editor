using System.Reflection;

namespace Editor.Utils;

public static class AssemblyUtils
{
	private static string? _versionString;

	public static string VersionString => _versionString ??= GetVersionString();

	private static string GetVersionString()
	{
		AssemblyInformationalVersionAttribute? attribute = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
		if (attribute == null)
			throw new InvalidOperationException("Could not get informational version attribute.");

		int index = attribute.InformationalVersion.IndexOf('+', StringComparison.OrdinalIgnoreCase);
		return index != -1 ? attribute.InformationalVersion[..index] : attribute.InformationalVersion;
	}
}
