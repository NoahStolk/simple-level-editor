using Silk.NET.OpenGL;
using SimpleLevelEditor.Ui;

namespace SimpleLevelEditor;

public sealed class Game
{
	private const float _maxMainDelta = 0.25f;

	private double _updateStartTime;

	private double _currentTime = Graphics.Glfw.GetTime();
	private double _accumulator;

	private double _updateRate;
	private double _mainLoopRate;

	private double _updateLength;
	private double _mainLoopLength;

	private int _currentSecond;
	private int _updates;
	private int _renders;

	public unsafe Game()
	{
		// Must call setters here.
		UpdateRate = 60;
		MainLoopRate = 300;

		Graphics.Gl.ClearColor(0.3f, 0.3f, 0.3f, 0);

		Graphics.Glfw.SetKeyCallback(Graphics.Window, (_, keys, _, state, _) =>
		{
			// We overwrite the key callback, so we need to call this method again.
			Input.KeyCallback(keys, state);
			Nodes.ImGuiController.PressKey(keys, state);
		});
	}

	public double UpdateRate
	{
		get => _updateRate;
		set
		{
			_updateRate = value;
			_updateLength = 1 / _updateRate;
		}
	}

	public double MainLoopRate
	{
		get => _mainLoopRate;
		set
		{
			_mainLoopRate = value;
			_mainLoopLength = 1 / _mainLoopRate;
		}
	}

	/// <summary>
	/// Represents the delta time in seconds.
	/// </summary>
	public float Dt { get; private set; }

	/// <summary>
	/// Represents the total elapsed time in seconds.
	/// </summary>
	public float Tt { get; private set; }

	public double FrameTime { get; private set; }

	public float SubFrame { get; private set; }

	public int Tps { get; private set; }
	public int Fps { get; private set; }

	public void Render()
	{
		Nodes.ImGuiController.Update((float)FrameTime);

		Graphics.Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

		MainWindow.Render();

		Nodes.ImGuiController.Render();
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

		Nodes.ImGuiController.Destroy();
		Graphics.Glfw.Terminate();
	}

	private unsafe void Main()
	{
		double mainStartTime = Graphics.Glfw.GetTime();

		if (_currentSecond != (int)mainStartTime)
		{
			Tps = _updates;
			Fps = _renders;
			_updates = 0;
			_renders = 0;
			_currentSecond = (int)mainStartTime;
		}

		FrameTime = mainStartTime - _currentTime;
		if (FrameTime > _maxMainDelta)
			FrameTime = _maxMainDelta;

		_currentTime = mainStartTime;

		_accumulator += FrameTime;

		Dt = (float)_updateLength;

		Graphics.Glfw.PollEvents();

		while (_accumulator >= _updateLength)
		{
			_updateStartTime = Graphics.Glfw.GetTime();

			_updates++;

			Tt += Dt;
			_accumulator -= _updateLength;
		}

		SubFrame = (float)((Graphics.Glfw.GetTime() - _updateStartTime) * UpdateRate);

		_renders++;
		Render();

		Graphics.Glfw.SwapBuffers(Graphics.Window);
	}
}
