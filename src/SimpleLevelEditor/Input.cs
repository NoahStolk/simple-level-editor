using Silk.NET.GLFW;

namespace SimpleLevelEditor;

public static class Input
{
	private const int _maxKeys = 1024;
	private const int _maxMouseButtons = 8;

	#region States

	private static readonly bool[] _keysPrevious = new bool[_maxKeys];
	private static readonly bool[] _keysCurrent = new bool[_maxKeys];

	private static readonly bool[] _mouseButtonsPrevious = new bool[_maxMouseButtons];
	private static readonly bool[] _mouseButtonsCurrent = new bool[_maxMouseButtons];

	private static double _mouseWheel;

	#endregion States

	#region Callbacks

	public static void KeyCallback(Keys key, InputAction inputState)
	{
		if (key >= 0 && (int)key < _maxKeys && inputState is InputAction.Press or InputAction.Release)
		{
			_keysCurrent[(int)key] = inputState == InputAction.Press;
		}
	}

	public static void ButtonCallback(MouseButton mouseButton, InputAction inputState)
	{
		if (mouseButton >= 0 && (int)mouseButton < _maxMouseButtons && inputState is InputAction.Press or InputAction.Release)
		{
			_mouseButtonsCurrent[(int)mouseButton] = inputState == InputAction.Press;
		}
	}

	#endregion Callbacks

	#region Keyboard

	public static bool IsKeyHeld(Keys key)
	{
		return _keysCurrent[(int)key];
	}

	public static bool IsKeyPressed(Keys key)
	{
		int k = (int)key;
		return _keysCurrent[k] && !_keysPrevious[k];
	}

	public static bool IsKeyReleased(Keys key)
	{
		int k = (int)key;
		return !_keysCurrent[k] && _keysPrevious[k];
	}

	#endregion Keyboard

	#region Mouse

	public static bool IsButtonHeld(MouseButton mouseButton)
	{
		return _mouseButtonsCurrent[(int)mouseButton];
	}

	public static bool IsButtonPressed(MouseButton mouseButton)
	{
		int b = (int)mouseButton;
		return _mouseButtonsCurrent[b] && !_mouseButtonsPrevious[b];
	}

	public static bool IsButtonReleased(MouseButton mouseButton)
	{
		int b = (int)mouseButton;
		return !_mouseButtonsCurrent[b] && _mouseButtonsPrevious[b];
	}

	public static void MouseWheelCallback(double delta)
	{
		_mouseWheel = delta;
	}

	public static int GetScroll()
	{
		return _mouseWheel > 0 ? 1 : _mouseWheel < 0 ? -1 : 0;
	}

	public static unsafe Vector2 GetMousePosition()
	{
		Graphics.Glfw.GetCursorPos(Graphics.Window, out double x, out double y);
		return new((float)x, (float)y);
	}

	#endregion Mouse

	internal static void PostUpdate()
	{
		for (int i = 0; i < _maxKeys; i++)
			_keysPrevious[i] = _keysCurrent[i];
		for (int i = 0; i < _maxMouseButtons; i++)
			_mouseButtonsPrevious[i] = _mouseButtonsCurrent[i];

		_mouseWheel = 0;
	}
}
