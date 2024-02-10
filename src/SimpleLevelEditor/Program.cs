using Detach.Parsers.Texture;
using Detach.Parsers.Texture.TgaFormat;
using ImGuiGlfw;
using ImGuiNET;
using SimpleLevelEditor;
using SimpleLevelEditor.Content;
using SimpleLevelEditor.User;
using SimpleLevelEditor.Utils;

Graphics.CreateWindow(new($"Simple Level Editor v{AssemblyUtils.VersionString}", Constants.WindowWidth, Constants.WindowHeight, false));
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

ImGuiIOPtr io = ImGui.GetIO();
unsafe
{
	io.NativePtr->IniFilename = null;
}

UserSettings.LoadImGuiIni();

ImGuiStylePtr style = ImGui.GetStyle();
style.WindowPadding = new(4, 4);
style.ItemSpacing = new(4, 4);

Graphics.OnChangeWindowSize = (w, h) =>
{
	Graphics.Gl.Viewport(0, 0, (uint)w, (uint)h);
	imGuiController.WindowResized(w, h);
};

App.Instance = new(imGuiController);
App.Instance.Run();
