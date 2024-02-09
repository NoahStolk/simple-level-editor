using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using SimpleLevelEditor.Extensions;
using System.Runtime.InteropServices;

namespace SimpleLevelEditor;

public sealed class ImGuiController
{
	private static readonly IReadOnlyList<Keys> _allKeys = (Keys[])Enum.GetValues(typeof(Keys));

	private readonly Dictionary<Keys, bool> _keysDown = [];
	private readonly List<char> _charsPressed = [];

	private readonly uint _vbo;
	private readonly uint _ebo;
	private uint _vao;

	private int _windowWidth;
	private int _windowHeight;
	private readonly Action<Matrix4x4> _useShader;

	private readonly IntPtr _context;

	public ImGuiController(int windowWidth, int windowHeight, Action<Matrix4x4> useShader)
	{
		_windowWidth = windowWidth;
		_windowHeight = windowHeight;
		_useShader = useShader;

		_context = ImGui.CreateContext();
		ImGui.SetCurrentContext(_context);
		ImGui.StyleColorsDark();

		ImGuiIOPtr io = ImGui.GetIO();
		io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;
		// io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;

		_vbo = Graphics.Gl.GenBuffer();
		_ebo = Graphics.Gl.GenBuffer();

		RecreateFontDeviceTexture();
	}

	#region Initialization

	private static unsafe void RecreateFontDeviceTexture()
	{
		// Build texture atlas
		ImGuiIOPtr io = ImGui.GetIO();

		// Load as RGBA 32-bit (75% of the memory is wasted, but default font is so small) because it is more likely to be compatible with user's existing shaders.
		// If your ImTextureId represent a higher-level concept than just a GL texture id, consider calling GetTexDataAsAlpha8() instead to save on GPU memory.
		io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);

		byte[] data = new byte[width * height * bytesPerPixel];
		Marshal.Copy(pixels, data, 0, data.Length);
		uint textureId = Graphics.Gl.GenTexture();

		Graphics.Gl.BindTexture(TextureTarget.Texture2D, textureId);

		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
		Graphics.Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

		fixed (byte* b = data)
			Graphics.Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, (uint)width, (uint)height, 0, GLEnum.Rgba, PixelType.UnsignedByte, b);

		io.Fonts.SetTexID((IntPtr)textureId);
	}

	#endregion Initialization

	public void Destroy()
	{
		Graphics.Gl.DeleteBuffer(_vbo);
		Graphics.Gl.DeleteBuffer(_ebo);
		Graphics.Gl.DeleteVertexArray(_vao);

		ImGui.DestroyContext(_context);
	}

	public void WindowResized(int width, int height)
	{
		_windowWidth = width;
		_windowHeight = height;
	}

	public void Render()
	{
		ImGui.Render();
		RenderImDrawData(ImGui.GetDrawData());
	}

	public void Update(float deltaSeconds)
	{
		ImGuiIOPtr io = ImGui.GetIO();
		io.DisplaySize = new(_windowWidth, _windowHeight);
		io.DisplayFramebufferScale = Vector2.One;
		io.DeltaTime = deltaSeconds;

		UpdateImGuiInput();

		ImGui.NewFrame();

		io.KeyCtrl = IsKeyDown(Keys.ControlLeft) || IsKeyDown(Keys.ControlRight);
		io.KeyAlt = IsKeyDown(Keys.AltLeft) || IsKeyDown(Keys.AltRight);
		io.KeyShift = IsKeyDown(Keys.ShiftLeft) || IsKeyDown(Keys.ShiftRight);
		io.KeySuper = IsKeyDown(Keys.SuperLeft) || IsKeyDown(Keys.SuperRight);
	}

	#region Input

	private void UpdateImGuiInput()
	{
		ImGuiIOPtr io = ImGui.GetIO();

		io.MousePos = Input.GetMousePosition();
		io.MouseWheel = Input.GetScroll();

		io.MouseDown[0] = Input.IsButtonHeld(MouseButton.Left);
		io.MouseDown[1] = Input.IsButtonHeld(MouseButton.Right);
		io.MouseDown[2] = Input.IsButtonHeld(MouseButton.Middle);

		for (int i = 0; i < _allKeys.Count; i++)
		{
			Keys key = _allKeys[i];
			int keyValue = (int)key;
			if (keyValue < 0)
				continue;

			io.AddKeyEvent(key.GetImGuiKey(), IsKeyDown(key));
		}

		for (int i = 0; i < _charsPressed.Count; i++)
			io.AddInputCharacter(_charsPressed[i]);

		_charsPressed.Clear();
	}

	public void PressKey(Keys keys, InputAction state, KeyModifiers keyModifiers)
	{
		if (state is InputAction.Press or InputAction.Repeat)
		{
			PressKey(keys);

			bool shift = keyModifiers.HasFlag(KeyModifiers.Shift);
			char? c = keys.GetChar(shift);
			if (c.HasValue)
				_charsPressed.Add(c.Value);
		}
		else
		{
			ReleaseKey(keys);
		}
	}

	private void PressKey(Keys key)
	{
		_keysDown[key] = true;
	}

	private void ReleaseKey(Keys key)
	{
		_keysDown[key] = false;
	}

	public bool IsKeyDown(Keys key)
	{
		return _keysDown.TryGetValue(key, out bool isDown) && isDown;
	}

	#endregion Input

	#region Rendering

	private unsafe void SetUpRenderState(ImDrawDataPtr drawDataPtr)
	{
		// Setup render state: alpha-blending enabled, no face culling, no depth testing, scissor enabled, polygon fill
		Graphics.Gl.Enable(GLEnum.Blend);
		Graphics.Gl.BlendEquation(GLEnum.FuncAdd);
		Graphics.Gl.BlendFuncSeparate(GLEnum.SrcAlpha, GLEnum.OneMinusSrcAlpha, GLEnum.One, GLEnum.OneMinusSrcAlpha);
		Graphics.Gl.Disable(GLEnum.CullFace);
		Graphics.Gl.Disable(GLEnum.DepthTest);
		Graphics.Gl.Disable(GLEnum.StencilTest);
		Graphics.Gl.Enable(GLEnum.ScissorTest);
		Graphics.Gl.Disable(GLEnum.PrimitiveRestart);
		Graphics.Gl.PolygonMode(GLEnum.FrontAndBack, GLEnum.Fill);

		Matrix4x4 orthographicProjection = Matrix4x4.CreateOrthographicOffCenter(
			left: drawDataPtr.DisplayPos.X,
			right: drawDataPtr.DisplayPos.X + drawDataPtr.DisplaySize.X,
			bottom: drawDataPtr.DisplayPos.Y + drawDataPtr.DisplaySize.Y,
			top: drawDataPtr.DisplayPos.Y,
			zNearPlane: -1,
			zFarPlane: 1);

		_useShader.Invoke(orthographicProjection);

		Graphics.Gl.BindSampler(0, 0);

		// Setup desired GL state
		// Recreate the VAO every time (this is to easily allow multiple GL contexts to be rendered to. VAO are not shared among GL contexts)
		// The renderer would actually work without any VAO bound, but then our VertexAttrib calls would overwrite the default one currently bound.
		_vao = Graphics.Gl.GenVertexArray();
		Graphics.Gl.BindVertexArray(_vao);

		// Bind vertex/index buffers and setup attributes for ImDrawVert
		Graphics.Gl.BindBuffer(GLEnum.ArrayBuffer, _vbo);
		Graphics.Gl.BindBuffer(GLEnum.ElementArrayBuffer, _ebo);

		Graphics.Gl.EnableVertexAttribArray(0);
		Graphics.Gl.EnableVertexAttribArray(1);
		Graphics.Gl.EnableVertexAttribArray(2);

		Graphics.Gl.VertexAttribPointer(0, 2, GLEnum.Float, false, (uint)sizeof(ImDrawVert), (void*)0);
		Graphics.Gl.VertexAttribPointer(1, 2, GLEnum.Float, false, (uint)sizeof(ImDrawVert), (void*)8);
		Graphics.Gl.VertexAttribPointer(2, 4, GLEnum.UnsignedByte, true, (uint)sizeof(ImDrawVert), (void*)16);
	}

	private unsafe void RenderImDrawData(ImDrawDataPtr drawDataPtr)
	{
		int framebufferWidth = (int)(drawDataPtr.DisplaySize.X * drawDataPtr.FramebufferScale.X);
		int framebufferHeight = (int)(drawDataPtr.DisplaySize.Y * drawDataPtr.FramebufferScale.Y);
		if (framebufferWidth <= 0 || framebufferHeight <= 0)
			return;

		SetUpRenderState(drawDataPtr);

		// Will project scissor/clipping rectangles into framebuffer space
		Vector2 clipOff = drawDataPtr.DisplayPos; // (0,0) unless using multi-viewports
		Vector2 clipScale = drawDataPtr.FramebufferScale; // (1,1) unless using retina display which are often (2,2)

		// Render command lists
		for (int n = 0; n < drawDataPtr.CmdListsCount; n++)
		{
			ImDrawListPtr cmdListPtr = drawDataPtr.CmdLists[n];

			// Upload vertex/index buffers
			Graphics.Gl.BufferData(GLEnum.ArrayBuffer, (nuint)(cmdListPtr.VtxBuffer.Size * sizeof(ImDrawVert)), (void*)cmdListPtr.VtxBuffer.Data, GLEnum.StreamDraw);
			Graphics.Gl.BufferData(GLEnum.ElementArrayBuffer, (nuint)(cmdListPtr.IdxBuffer.Size * sizeof(ushort)), (void*)cmdListPtr.IdxBuffer.Data, GLEnum.StreamDraw);

			for (int cmdI = 0; cmdI < cmdListPtr.CmdBuffer.Size; cmdI++)
			{
				ImDrawCmdPtr cmdPtr = cmdListPtr.CmdBuffer[cmdI];
				if (cmdPtr.UserCallback != IntPtr.Zero)
					throw new NotImplementedException();

				Vector4 clipRect;
				clipRect.X = (cmdPtr.ClipRect.X - clipOff.X) * clipScale.X;
				clipRect.Y = (cmdPtr.ClipRect.Y - clipOff.Y) * clipScale.Y;
				clipRect.Z = (cmdPtr.ClipRect.Z - clipOff.X) * clipScale.X;
				clipRect.W = (cmdPtr.ClipRect.W - clipOff.Y) * clipScale.Y;

				if (clipRect.X >= framebufferWidth || clipRect.Y >= framebufferHeight || clipRect.Z < 0.0f || clipRect.W < 0.0f)
					continue;

				// Apply scissor/clipping rectangle
				Graphics.Gl.Scissor((int)clipRect.X, (int)(framebufferHeight - clipRect.W), (uint)(clipRect.Z - clipRect.X), (uint)(clipRect.W - clipRect.Y));

				// Bind texture, Draw
				Graphics.Gl.BindTexture(GLEnum.Texture2D, (uint)cmdPtr.TextureId);
				Graphics.Gl.DrawElementsBaseVertex(GLEnum.Triangles, cmdPtr.ElemCount, GLEnum.UnsignedShort, (void*)(cmdPtr.IdxOffset * sizeof(ushort)), (int)cmdPtr.VtxOffset);
			}
		}

		// Destroy the temporary VAO
		Graphics.Gl.DeleteVertexArray(_vao);
		_vao = 0;

		// Restore scissors
		Graphics.Gl.Disable(EnableCap.ScissorTest);
	}

	#endregion Rendering
}
