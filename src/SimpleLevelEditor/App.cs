using Silk.NET.OpenGL;
using SimpleLevelEditor.Ui;

namespace SimpleLevelEditor;

public sealed class App
{
	private const double _maxMainDelta = 0.25;
	private const double _mainLoopLength = 1 / 300.0;

	private static App? _instance;

	private readonly ImGuiController _imGuiController;

	private double _currentTime = Graphics.Glfw.GetTime();
	private double _frameTime;

	private int _currentSecond;
	private int _renders;

	public unsafe App(ImGuiController imGuiController)
	{
		_imGuiController = imGuiController;

		Graphics.Gl.ClearColor(0.3f, 0.3f, 0.3f, 0);

		Graphics.Glfw.SetKeyCallback(Graphics.Window, (_, keys, _, state, _) =>
		{
			Input.KeyCallback(keys, state);
			imGuiController.PressKey(keys, state);
		});
	}

	public int Fps { get; private set; }
	public float FrameTime => (float)_frameTime;

	public static App Instance
	{
		get => _instance ?? throw new InvalidOperationException("App is not initialized.");
		set
		{
			if (_instance != null)
				throw new InvalidOperationException("App is already initialized.");

			_instance = value;
		}
	}

	public unsafe void Run()
	{
		while (!Graphics.Glfw.WindowShouldClose(Graphics.Window))
		{
			double expectedNextFrame = Graphics.Glfw.GetTime() + _mainLoopLength;
			Main();

			while (Graphics.Glfw.GetTime() < expectedNextFrame)
				Thread.Yield();
		}

		AssetFileWatcher.Destroy();
		_imGuiController.Destroy();
		Graphics.Glfw.Terminate();
	}

	private unsafe void Main()
	{
		double mainStartTime = Graphics.Glfw.GetTime();
		if (_currentSecond != (int)mainStartTime)
		{
			Fps = _renders;
			_renders = 0;
			_currentSecond = (int)mainStartTime;
		}

		_frameTime = mainStartTime - _currentTime;
		if (_frameTime > _maxMainDelta)
			_frameTime = _maxMainDelta;

		_currentTime = mainStartTime;

		Graphics.Glfw.PollEvents();

		Render();
		_renders++;

		Input.PostUpdate();

		Graphics.Glfw.SwapBuffers(Graphics.Window);
	}

	private void Render()
	{
		_imGuiController.Update((float)_frameTime);

		Graphics.Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

		Shortcuts.Handle();

		MainWindow.Render();

		_imGuiController.Render();
	}
}
