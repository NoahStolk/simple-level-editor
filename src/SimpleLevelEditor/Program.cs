using SimpleLevelEditor;

Graphics.OnChangeWindowSize = (w, h) =>
{
	if (!Nodes.IsInitialized)
		return;

	Graphics.Gl.Viewport(0, 0, (uint)w, (uint)h);
	Nodes.ImGuiController.WindowResized(w, h);
};

Graphics.CreateWindow(new("Simple Level Editor", Constants.WindowWidth, Constants.WindowHeight, false));
Graphics.SetWindowSizeLimits(1024, 768, 4096, 2160);

Game game = new();

Nodes.Initialize();

game.Run();
