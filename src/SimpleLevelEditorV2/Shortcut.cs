using SimpleLevelEditorV2.Extensions;
using Silk.NET.GLFW;

namespace SimpleLevelEditorV2;

public sealed record Shortcut(string Id, Keys Key, bool Ctrl, bool Shift, string Description, Action Action)
{
	public string KeyDescription { get; } = $"{(Ctrl ? "CTRL+" : string.Empty)}{(Shift ? "SHIFT+" : string.Empty)}{Key.GetDisplayString(true)}";
}
