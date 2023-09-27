using SimpleLevelEditor.Content.Containers;
using SimpleLevelEditor.Ui;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor;

public static class Nodes
{
	private static ImGuiController? _imGuiController;

	public static ImGuiController ImGuiController => _imGuiController ?? throw new InvalidOperationException("Custom nodes are not initialized.");

	public static bool IsInitialized { get; private set; }

	public static void Initialize()
	{
		if (IsInitialized)
			throw new InvalidOperationException("Custom nodes are already initialized.");

		ShaderContainer.Add("Line", File.ReadAllText(Path.Combine("Resources", "Shaders", "Line.vert")), File.ReadAllText(Path.Combine("Resources", "Shaders", "Line.frag")));
		ShaderContainer.Add("Mesh", File.ReadAllText(Path.Combine("Resources", "Shaders", "Mesh.vert")), File.ReadAllText(Path.Combine("Resources", "Shaders", "Mesh.frag")));
		ShaderContainer.Add("Ui", File.ReadAllText(Path.Combine("Resources", "Shaders", "Ui.vert")), File.ReadAllText(Path.Combine("Resources", "Shaders", "Ui.frag")));

		_imGuiController = new(Constants.WindowWidth, Constants.WindowHeight, static orthoProjection =>
		{
			uint uiShaderId = ShaderContainer.Shaders["Ui"];
			Graphics.Gl.UseProgram(uiShaderId);

			int texture = Graphics.Gl.GetUniformLocation(uiShaderId, "Texture");
			int projMtx = Graphics.Gl.GetUniformLocation(uiShaderId, "ProjMtx");

			ShaderUniformUtils.Set(texture, 0);
			ShaderUniformUtils.Set(projMtx, orthoProjection);
		});

		Style.Initialize();

		IsInitialized = true;
	}
}
