using Detach.Parsers.Texture;
using Detach.Parsers.Texture.TgaFormat;
using ImGuiGlfw;
using ImGuiNET;
using SimpleLevelEditor;
using SimpleLevelEditor.Rendering.Content;
using SimpleLevelEditor.User;
using SimpleLevelEditor.Utils;

AppDomain.CurrentDomain.UnhandledException += (_, args) => LogUtils.Log.Fatal(args.ExceptionObject.ToString() ?? "<NO ERROR>");

UserSettings.LoadSettings();

Graphics.CreateWindow($"Simple Level Editor v{AssemblyUtils.VersionString}", Constants.WindowWidth, Constants.WindowHeight, UserSettings.Settings.StartMaximized);
Graphics.SetWindowSizeLimits(1024, 768, 4096, 2160);

foreach (string filePath in Directory.GetFiles(Path.Combine("Resources", "Shaders")).DistinctBy(Path.GetFileNameWithoutExtension))
{
	string shaderName = Path.GetFileNameWithoutExtension(filePath);
	string vertexCode = File.ReadAllText(Path.Combine("Resources", "Shaders", $"{shaderName}.vert"));
	string fragmentCode = File.ReadAllText(Path.Combine("Resources", "Shaders", $"{shaderName}.frag"));
	InternalContent.AddShader(shaderName, vertexCode, fragmentCode);
}

foreach (string filePath in Directory.GetFiles(Path.Combine("Resources", "Textures")).DistinctBy(Path.GetFileNameWithoutExtension))
{
	string textureName = Path.GetFileNameWithoutExtension(filePath);
	TextureData texture = TgaParser.Parse(File.ReadAllBytes(filePath));
	InternalContent.AddTexture(textureName, texture);
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
