using Serilog;
using Serilog.Core;
using SimpleLevelEditorV2.Utils;
using System.Globalization;

namespace SimpleLevelEditorV2.Logging;

public static class GlobalLogger
{
	private static readonly List<Message> _messages = [];

	private static readonly Logger _log = new LoggerConfiguration()
		.WriteTo.File($"simple-level-editor-{AssemblyUtils.VersionString}.log", formatProvider: CultureInfo.InvariantCulture, rollingInterval: RollingInterval.Infinite)
		.CreateLogger();

	public static IReadOnlyList<Message> Messages => _messages;

	public static void ClearMessages()
	{
		_messages.Clear();
	}

	public static void ClearMessagesOlderThan(TimeSpan timeSpan)
	{
		_messages.RemoveAll(m => DateTime.UtcNow - m.LastAppeared > timeSpan);
	}

	public static void LogInfo(string message)
	{
		_log.Information(message);
		AddMessage(message, MessageSeverity.Info);
	}

	public static void LogWarning(string message)
	{
		_log.Warning(message);
		AddMessage(message, MessageSeverity.Warning);
	}

	public static void LogError(string message)
	{
		_log.Error(message);
		AddMessage(message, MessageSeverity.Error);
	}

	public static void LogFatal(string message)
	{
		_log.Fatal(message);
		AddMessage(message, MessageSeverity.Fatal);
	}

	private static void AddMessage(string text, MessageSeverity severity)
	{
		Message? message = _messages.Find(m => m.Text == text && m.Severity == severity);
		if (message == null)
		{
			_messages.Add(new Message(text, severity));
		}
		else
		{
			message.LastAppeared = DateTime.UtcNow;
			message.Count++;
		}
	}
}
