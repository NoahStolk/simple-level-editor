using Silk.NET.GLFW;

namespace SimpleLevelEditor;

public static class Input
{
	private const int _maxKeys = 1024;
	private const int _maxMouseButtons = 8;

	private static readonly bool[] _keysPrevious = new bool[_maxKeys];
	private static readonly bool[] _keysCurrent = new bool[_maxKeys];

	private static readonly bool[] _mouseButtonsPrevious = new bool[_maxMouseButtons];
	private static readonly bool[] _mouseButtonsCurrent = new bool[_maxMouseButtons];

	public static bool IsKeyPressed(Keys key)
	{
		int k = (int)key;
		return _keysCurrent[k] && !_keysPrevious[k];
	}

	public static bool IsButtonPressed(MouseButton mouseButton)
	{
		int b = (int)mouseButton;
		return _mouseButtonsCurrent[b] && !_mouseButtonsPrevious[b];
	}
}
