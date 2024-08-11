using Detach.Parsers.Texture;
using Detach.Parsers.Texture.TgaFormat;
using Editor;
using Editor.States.InternalContent;
using Editor.User;
using Editor.Utils;
using ImGuiGlfw;
using ImGuiNET;

AppDomain.CurrentDomain.UnhandledException += (_, args) => LogUtils.Log.Fatal(args.ExceptionObject.ToString() ?? "<NO ERROR>");

UserSettings.LoadSettings();

Graphics.CreateWindow($"Simple Level Editor v{AssemblyUtils.VersionString}", Constants.WindowWidth, Constants.WindowHeight, UserSettings.Settings.StartMaximized);
Graphics.SetWindowSizeLimits(1024, 768, 4096, 2160);

foreach (string filePath in Directory.GetFiles(Path.Combine(Constants.ContentDirectoryName, "Shaders")).DistinctBy(Path.GetFileNameWithoutExtension))
{
	string shaderName = Path.GetFileNameWithoutExtension(filePath);
	string vertexCode = File.ReadAllText(Path.Combine(Constants.ContentDirectoryName, "Shaders", $"{shaderName}.vert"));
	string fragmentCode = File.ReadAllText(Path.Combine(Constants.ContentDirectoryName, "Shaders", $"{shaderName}.frag"));
	InternalContentState.AddShader(Graphics.Gl, shaderName, vertexCode, fragmentCode);
}

foreach (string filePath in Directory.GetFiles(Path.Combine(Constants.ContentDirectoryName, "Textures")).DistinctBy(Path.GetFileNameWithoutExtension))
{
	string textureName = Path.GetFileNameWithoutExtension(filePath);
	TextureData texture = TgaParser.Parse(File.ReadAllBytes(filePath));
	InternalContentState.AddTexture(Graphics.Gl, textureName, texture);
}

ImGuiController imGuiController = new(Graphics.Gl, Input.GlfwInput, Constants.WindowWidth, Constants.WindowHeight);
imGuiController.CreateDefaultFont();

ImGuiIOPtr io = ImGui.GetIO();
unsafe
{
	io.NativePtr->IniFilename = null;
}

UserSettings.LoadImGuiIni();

ImGuiStylePtr style = ImGui.GetStyle();
style.WindowPadding = new Vector2(4, 4);
style.ItemSpacing = new Vector2(4, 4);

Graphics.OnChangeWindowSize = (w, h) =>
{
	Graphics.Gl.Viewport(0, 0, (uint)w, (uint)h);
	imGuiController.WindowResized(w, h);
};

unsafe
{
	// Always invoke this in case of window size change via StartMaximized.
	Graphics.Glfw.GetWindowSize(Graphics.Window, out int width, out int height);
	Graphics.OnChangeWindowSize.Invoke(width, height);
}

App.Instance = new App(imGuiController);
App.Instance.Run();
