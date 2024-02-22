using Serilog;
using Serilog.Core;
using System.Globalization;

namespace SimpleLevelEditor.Utils;

public static class LogUtils
{
	public static Logger Log { get; } = new LoggerConfiguration()
		.WriteTo.File($"simple-level-editor-{AssemblyUtils.VersionString}.log", formatProvider: CultureInfo.InvariantCulture, rollingInterval: RollingInterval.Infinite)
		.CreateLogger();
}
