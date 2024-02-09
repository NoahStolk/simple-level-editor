using Detach.Parsers.Texture;
using Detach.Parsers.Texture.TgaFormat;
using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using SimpleLevelEditor.Ui;
using System.Runtime.InteropServices;

namespace SimpleLevelEditor;

public sealed class App
{
	private const double _maxMainDelta = 0.25;
	private const double _mainLoopLength = 1 / 300.0;

	private static App? _instance;

	private double _currentTime = Graphics.Glfw.GetTime();
	private double _frameTime;

	private int _currentSecond;
	private int _renders;

	public unsafe App(ImGuiController imGuiController)
	{
		ImGuiController = imGuiController;

		Graphics.Gl.ClearColor(0.3f, 0.3f, 0.3f, 0);

		Graphics.Glfw.SetKeyCallback(Graphics.Window, (_, keys, _, state, keyModifiers) =>
		{
			Input.KeyCallback(keys, state);
			imGuiController.PressKey(keys, state, keyModifiers);
		});

		TextureData texture = TgaParser.Parse(File.ReadAllBytes(Path.Combine("Resources", "Textures", "Icon.tga")));

		IntPtr iconPtr = Marshal.AllocHGlobal(texture.Width * texture.Height * 4);
		Marshal.Copy(texture.ColorData, 0, iconPtr, texture.Width * texture.Height * 4);
		Image image = new()
		{
			Width = texture.Width,
			Height = texture.Height,
			Pixels = (byte*)iconPtr,
		};
		Graphics.Glfw.SetWindowIcon(Graphics.Window, 1, &image);
	}

	public int Fps { get; private set; }
	public float FrameTime => (float)_frameTime;
	public ImGuiController ImGuiController { get; }

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
		ImGuiController.Destroy();
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
		ImGuiController.Update((float)_frameTime);

		Graphics.Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

		Shortcuts.Handle();

		// ImGui.DockSpaceOverViewport();

		MainWindow.Render();

		ImGuiController.Render();
	}
}
