using Silk.NET.GLFW;

namespace SimpleLevelEditor;

public static class Input
{
	private const int _maxKeys = 1024;
	private const int _maxMouseButtons = 8;
	private const int _maxJoystickButtons = 16;

	#region States

	private static readonly bool[] _keysPrevious = new bool[_maxKeys];
	private static readonly bool[] _keysCurrent = new bool[_maxKeys];

	private static readonly bool[] _mouseButtonsPrevious = new bool[_maxMouseButtons];
	private static readonly bool[] _mouseButtonsCurrent = new bool[_maxMouseButtons];

	private static double _mouseWheel;

	private static readonly bool[] _joystickButtonsPrevious = new bool[_maxJoystickButtons];
	private static readonly bool[] _joystickButtonsCurrent = new bool[_maxJoystickButtons];

	public static int LastConnectedJoystick { get; private set; }

	public static Vector2 LeftStick { get; private set; }
	public static Vector2 RightStick { get; private set; }
	public static float LeftTrigger { get; private set; }
	public static float RightTrigger { get; private set; }

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

	public static void JoystickCallback(int joystick, ConnectedState state)
	{
		if (state == ConnectedState.Connected)
			LastConnectedJoystick = joystick;
	}

	#endregion Callbacks

	#region Keyboard

	public static bool IsKeyHeld(Keys key) => _keysCurrent[(int)key];

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
		=> _mouseButtonsCurrent[(int)mouseButton];

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
		=> _mouseWheel = delta;

	public static int GetScroll()
		=> _mouseWheel > 0 ? 1 : _mouseWheel < 0 ? -1 : 0;

	public static unsafe Vector2 GetMousePosition()
	{
		Graphics.Glfw.GetCursorPos(Graphics.Window, out double x, out double y);
		return new((float)x, (float)y);
	}

	#endregion Mouse

	internal static unsafe void PreUpdate()
	{
		float* axes = Graphics.Glfw.GetJoystickAxes(LastConnectedJoystick, out int axesCount);
		if (axes == null)
		{
			LeftStick = Vector2.Zero;
			RightStick = Vector2.Zero;
			LeftTrigger = 0;
			RightTrigger = 0;
		}
		else
		{
			if (axesCount >= 2)
				LeftStick = new(axes[0], axes[1]);

			if (axesCount >= 4)
				RightStick = new(axes[2], axes[3]);

			if (axesCount >= 6)
			{
				LeftTrigger = axes[4];
				RightTrigger = axes[5];
			}
		}

		byte* buttons = Graphics.Glfw.GetJoystickButtons(LastConnectedJoystick, out int buttonsCount);
		if (buttons == null)
		{
			for (int i = 0; i < _maxJoystickButtons; i++)
				_joystickButtonsCurrent[i] = false;
		}
		else
		{
			for (int i = 0; i < _maxJoystickButtons; i++)
				_joystickButtonsCurrent[i] = i < buttonsCount && buttons[i] == 1;
		}
	}

	internal static void PostUpdate()
	{
		for (int i = 0; i < _maxKeys; i++)
			_keysPrevious[i] = _keysCurrent[i];
		for (int i = 0; i < _maxMouseButtons; i++)
			_mouseButtonsPrevious[i] = _mouseButtonsCurrent[i];
		for (int i = 0; i < _maxJoystickButtons; i++)
			_joystickButtonsPrevious[i] = _joystickButtonsCurrent[i];

		_mouseWheel = 0;
	}
}
