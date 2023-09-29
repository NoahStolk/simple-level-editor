using ImGuiNET;
using SimpleLevelEditor;
using SimpleLevelEditor.Content;
using SimpleLevelEditor.Utils;

Graphics.CreateWindow(new("Simple Level Editor", Constants.WindowWidth, Constants.WindowHeight, false));
Graphics.SetWindowSizeLimits(1024, 768, 4096, 2160);

foreach (string filePath in Directory.GetFiles(Path.Combine("Resources", "Shaders")).DistinctBy(Path.GetFileNameWithoutExtension))
{
	string shaderName = Path.GetFileNameWithoutExtension(filePath);
	string vertexCode = File.ReadAllText(Path.Combine("Resources", "Shaders", $"{shaderName}.vert"));
	string fragmentCode = File.ReadAllText(Path.Combine("Resources", "Shaders", $"{shaderName}.frag"));
	ShaderContainer.Add(shaderName, vertexCode, fragmentCode);
}

ImGuiController imGuiController = new(Constants.WindowWidth, Constants.WindowHeight, static orthoProjection =>
{
	ShaderCacheEntry uiShader = ShaderContainer.Shaders["Ui"];
	Graphics.Gl.UseProgram(uiShader.Id);

	int texture = uiShader.GetUniformLocation("Texture");
	int projMtx = uiShader.GetUniformLocation("ProjMtx");

	ShaderUniformUtils.Set(texture, 0);
	ShaderUniformUtils.Set(projMtx, orthoProjection);
});

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
