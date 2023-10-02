using Silk.NET.GLFW;
using SimpleLevelEditor.Extensions;

namespace SimpleLevelEditor;

public sealed record Shortcut(Keys Key, bool Ctrl, bool Shift, Action Action, string Description)
{
	// TODO: GetChar is not ideal for this.
	public string KeyDescription { get; } = $"{(Ctrl ? "CTRL+" : string.Empty)}{(Shift ? "SHIFT+" : string.Empty)}{char.ToUpper(Key.GetChar(false) ?? '?')}";
}
