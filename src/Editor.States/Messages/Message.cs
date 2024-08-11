namespace Editor.States.Messages;

public record Message(string Text, MessageSeverity Severity)
{
	public DateTime LastAppeared { get; set; } = DateTime.UtcNow;

	public int Count { get; set; } = 1;
}
