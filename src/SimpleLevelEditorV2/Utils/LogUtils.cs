using Serilog;
using Serilog.Core;
using System.Globalization;

namespace SimpleLevelEditorV2.Utils;

public static class LogUtils
{
	public static Logger Log { get; } = new LoggerConfiguration()
		.WriteTo.File($"simple-level-editor-{AssemblyUtils.VersionString}.log", formatProvider: CultureInfo.InvariantCulture, rollingInterval: RollingInterval.Infinite)
		.CreateLogger();
}
