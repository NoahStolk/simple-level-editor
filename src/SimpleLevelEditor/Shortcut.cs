using Silk.NET.GLFW;

namespace SimpleLevelEditor;

public sealed record Shortcut(Keys Key, bool Shift, bool Ctrl, Action Action);
