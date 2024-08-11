namespace Editor.States.Messages;

public static class MessagesState
{
	private static readonly List<Message> _messages = [];

	public static IReadOnlyList<Message> Messages => _messages;

	public static void ClearMessages()
	{
		_messages.Clear();
	}

	public static void ClearMessagesOlderThan(TimeSpan timeSpan)
	{
		_messages.RemoveAll(m => DateTime.UtcNow - m.LastAppeared > timeSpan);
	}

	public static void AddInfo(string message)
	{
		AddMessage(message, MessageSeverity.Info);
	}

	public static void AddWarning(string message)
	{
		AddMessage(message, MessageSeverity.Warning);
	}

	public static void AddError(string message)
	{
		AddMessage(message, MessageSeverity.Error);
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
