using Detach.Parsers.Texture;
using Detach.Parsers.Texture.TgaFormat;
using ImGuiGlfw;
using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using SimpleLevelEditorV2.States.App;
using SimpleLevelEditorV2.States.GameEntityConfigBuilder;
using SimpleLevelEditorV2.States.LevelEditor;
using SimpleLevelEditorV2.Ui.GameEntityConfigBuilder;
using SimpleLevelEditorV2.Ui.LevelEditor;
using SimpleLevelEditorV2.Ui.Main;
using SimpleLevelEditorV2.User;
using System.Runtime.InteropServices;

namespace SimpleLevelEditorV2;

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

	private readonly MainWindow _mainWindow = new();

	private readonly GameEntityConfigBuilderMainMenuBar _gameEntityConfigBuilderMainMenuBar = new();
	private readonly GameEntityConfigModelsWindow _gameEntityConfigModelsWindow = new();
	private readonly GameEntityConfigTexturesWindow _gameEntityConfigTexturesWindow = new();
	private readonly GameEntityConfigDataTypesWindow _gameEntityConfigDataTypesWindow = new();
	private readonly GameEntityConfigEntityDescriptorsWindow _gameEntityConfigEntityDescriptorsWindow = new();

	private readonly LevelEditorMainMenuBar _levelEditorMainMenuBar = new();
	private readonly LevelEditorWindow _levelEditorWindow = new();

	private readonly AppState _appState = new();
	private readonly GameEntityConfigBuilderState _gameEntityConfigBuilderState = new();
	private readonly LevelEditorState _levelEditorState = new();

	public unsafe App(ImGuiController imGuiController)
	{
		_imGuiController = imGuiController;

		TextureData texture = TgaParser.Parse(File.ReadAllBytes(Path.Combine(Constants.ContentDirectoryName, "Textures", "Icon.tga")));

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

		Graphics.Glfw.SwapBuffers(Graphics.Window);
	}

	private void Render()
	{
		_imGuiController.Update((float)_frameTime);

		Graphics.Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

		Shortcuts.Handle();

		ImGui.DockSpaceOverViewport(null, ImGuiDockNodeFlags.PassthruCentralNode);

		RenderUi();

		ImGuiIOPtr io = ImGui.GetIO();
		if (io.WantSaveIniSettings)
			UserSettings.SaveImGuiIni(io);

		_imGuiController.Render();

		Input.GlfwInput.PostRender();
	}

	private void RenderUi()
	{
		switch (_appState.CurrentView)
		{
			case AppView.Main:
				_mainWindow.Render(_appState);
				break;
			case AppView.GameEntityConfigEditor:
				_gameEntityConfigBuilderMainMenuBar.Render(_appState, _gameEntityConfigBuilderState);

				Vector2 initialWindowSize = new(640, 480);
				_gameEntityConfigModelsWindow.Render(_gameEntityConfigBuilderState, initialWindowSize);
				_gameEntityConfigTexturesWindow.Render(_gameEntityConfigBuilderState, initialWindowSize);
				_gameEntityConfigDataTypesWindow.Render(_gameEntityConfigBuilderState, initialWindowSize);
				_gameEntityConfigEntityDescriptorsWindow.Render(_gameEntityConfigBuilderState, initialWindowSize);
				break;
			case AppView.LevelEditor:
				_levelEditorMainMenuBar.Render(_appState, _levelEditorState);
				_levelEditorWindow.Render(_appState, _levelEditorState);
				break;
		}
	}
}
