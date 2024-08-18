using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using System.Text;
using ErrorCode = Silk.NET.GLFW.ErrorCode;
using Monitor = Silk.NET.GLFW.Monitor;

namespace SimpleLevelEditorV2;

public static class Graphics
{
	private static bool _windowIsCreated;

	private static bool _windowIsActive = true;
	private static Glfw? _glfw;
	private static GL? _gl;

	public static Glfw Glfw => _glfw ?? throw new InvalidOperationException("GLFW is not initialized.");
	public static GL Gl => _gl ?? throw new InvalidOperationException("OpenGL is not initialized.");

	public static Action<bool>? OnChangeWindowIsActive { get; set; }
	public static Action<int, int>? OnChangeWindowSize { get; set; }

	public static unsafe WindowHandle* Window { get; private set; }

	public static int Width { get; private set; }
	public static int Height { get; private set; }
	public static bool WindowIsActive
	{
		get => _windowIsActive;
		private set
		{
			_windowIsActive = value;
			OnChangeWindowIsActive?.Invoke(_windowIsActive);
		}
	}

	public static unsafe void CreateWindow(string title, int width, int height, bool startMaximized)
	{
		if (_windowIsCreated)
			throw new InvalidOperationException("Window is already created. Cannot create window again.");

		Width = width;
		Height = height;

		_glfw = Glfw.GetApi();
		_glfw.Init();
		CheckGlfwError(_glfw);

		_glfw.WindowHint(WindowHintInt.ContextVersionMajor, 3);
		_glfw.WindowHint(WindowHintInt.ContextVersionMinor, 3);
		_glfw.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);

		_glfw.WindowHint(WindowHintBool.Focused, true);
		_glfw.WindowHint(WindowHintBool.Resizable, true);
		CheckGlfwError(_glfw);

		Window = _glfw.CreateWindow(width, height, title, null, null);
		CheckGlfwError(_glfw);
		if (Window == (WindowHandle*)0)
			throw new InvalidOperationException("Could not create window.");

		_glfw.SetFramebufferSizeCallback(Window, (_, w, h) => SetWindowSize(w, h));
		_glfw.SetWindowFocusCallback(Window, (_, focusing) => WindowIsActive = focusing);
		_glfw.SetCursorPosCallback(Window, (_, x, y) => Input.GlfwInput.CursorPosCallback(x, y));
		_glfw.SetScrollCallback(Window, (_, _, y) => Input.GlfwInput.MouseWheelCallback(y));
		_glfw.SetMouseButtonCallback(Window, (_, button, state, _) => Input.GlfwInput.MouseButtonCallback(button, state));
		_glfw.SetKeyCallback(Window, (_, keys, _, state, _) => Input.GlfwInput.KeyCallback(keys, state));
		_glfw.SetCharCallback(Window, (_, codepoint) => Input.GlfwInput.CharCallback(codepoint));

		if (startMaximized)
		{
			_glfw.MaximizeWindow(Window);
		}
		else
		{
			Monitor* primaryMonitor = _glfw.GetPrimaryMonitor();
			int primaryMonitorWidth, primaryMonitorHeight;
			if (primaryMonitor != null)
				_glfw.GetMonitorWorkarea(primaryMonitor, out _, out _, out primaryMonitorWidth, out primaryMonitorHeight);
			else
				(primaryMonitorWidth, primaryMonitorHeight) = (1024, 768);

			_glfw.SetWindowPos(Window, (primaryMonitorWidth - width) / 2, (primaryMonitorHeight - height) / 2);
		}

		_glfw.MakeContextCurrent(Window);
		_gl = GL.GetApi(_glfw.GetProcAddress);

		SetWindowSize(width, height);

		_glfw.SwapInterval(0); // Turns VSync off.

		_windowIsCreated = true;
	}

	public static unsafe void SetWindowSizeLimits(int minWidth, int minHeight, int maxWidth, int maxHeight)
	{
		Glfw.SetWindowSizeLimits(Window, minWidth, minHeight, maxWidth, maxHeight);
	}

	private static void SetWindowSize(int width, int height)
	{
		Width = width;
		Height = height;
		OnChangeWindowSize?.Invoke(width, height);
	}

	private static unsafe void CheckGlfwError(Glfw glfw)
	{
		ErrorCode errorCode = glfw.GetError(out byte* c);
		if (errorCode == ErrorCode.NoError || c == (byte*)0)
			return;

		StringBuilder errorBuilder = new();
		while (*c != 0x00)
			errorBuilder.Append((char)*c++);

		throw new InvalidOperationException($"GLFW {errorCode}: {errorBuilder}");
	}
}
