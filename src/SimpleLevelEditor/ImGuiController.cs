using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using SimpleLevelEditor.Extensions;
using System.Runtime.InteropServices;

namespace SimpleLevelEditor;

public sealed class ImGuiController
{
	private static readonly IReadOnlyList<Keys> _allKeys = (Keys[])Enum.GetValues(typeof(Keys));
	private static readonly IEnumerable<Keys> _controlKeys = new List<Keys>
	{
		Keys.Left,
		Keys.Right,
		Keys.Up,
		Keys.Down,
		Keys.Home,
		Keys.End,
		Keys.PageUp,
		Keys.PageDown,
		Keys.Delete,
		Keys.Backspace,
		Keys.Enter,
		Keys.Escape,
		Keys.Tab,
	};

	private readonly List<char> _pressedChars = new();
	private readonly List<Keys> _pressedControlKeys = new();

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

		_vbo = Graphics.Gl.GenBuffer();
		_ebo = Graphics.Gl.GenBuffer();

		RecreateFontDeviceTexture();
		SetKeyMappings();
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

	private static void SetKeyMappings()
	{
		ImGuiIOPtr io = ImGui.GetIO();
		io.KeyRepeatRate = 1f / 30f;
		io.KeyRepeatDelay = 0.05f;
		io.KeyMap[(int)ImGuiKey.Tab] = (int)Keys.Tab;
		io.KeyMap[(int)ImGuiKey.LeftArrow] = (int)Keys.Left;
		io.KeyMap[(int)ImGuiKey.RightArrow] = (int)Keys.Right;
		io.KeyMap[(int)ImGuiKey.UpArrow] = (int)Keys.Up;
		io.KeyMap[(int)ImGuiKey.DownArrow] = (int)Keys.Down;
		io.KeyMap[(int)ImGuiKey.PageUp] = (int)Keys.PageUp;
		io.KeyMap[(int)ImGuiKey.PageDown] = (int)Keys.PageDown;
		io.KeyMap[(int)ImGuiKey.Home] = (int)Keys.Home;
		io.KeyMap[(int)ImGuiKey.End] = (int)Keys.End;
		io.KeyMap[(int)ImGuiKey.Delete] = (int)Keys.Delete;
		io.KeyMap[(int)ImGuiKey.Backspace] = (int)Keys.Backspace;
		io.KeyMap[(int)ImGuiKey.Enter] = (int)Keys.Enter;
		io.KeyMap[(int)ImGuiKey.KeypadEnter] = (int)Keys.KeypadEnter;
		io.KeyMap[(int)ImGuiKey.Escape] = (int)Keys.Escape;

		for (int i = 0; i < 26; i++)
			io.KeyMap[(int)ImGuiKey.A + i] = (int)Keys.A + i;

		for (int i = 0; i < 10; i++)
		{
			io.KeyMap[(int)ImGuiKey._0 + i] = (int)Keys.Number0 + i;
			io.KeyMap[(int)ImGuiKey.Keypad0 + i] = (int)Keys.Keypad0 + i;
		}
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
		UpdateMouse(io);
		UpdateKeyboard(io);

		ImGui.NewFrame();
	}

	private void UpdateMouse(ImGuiIOPtr io)
	{
		io.MousePos = Input.GetMousePosition();
		io.MouseWheel = Input.GetScroll();

		io.MouseDown[0] = Input.IsButtonHeld(MouseButton.Left);
		io.MouseDown[1] = Input.IsButtonHeld(MouseButton.Right);
		io.MouseDown[2] = Input.IsButtonHeld(MouseButton.Middle);
	}

	private void UpdateKeyboard(ImGuiIOPtr io)
	{
		for (int i = 0; i < _allKeys.Count; i++)
		{
			Keys key = _allKeys[i];
			int keyValue = (int)key;
			if (keyValue < 0)
				continue;

			if (_controlKeys.Contains(key))
				io.KeysDown[keyValue] = _pressedControlKeys.Contains(key);
			else
				io.KeysDown[keyValue] = Input.IsKeyHeld(key);
		}

		_pressedControlKeys.Clear();

		for (int i = 0; i < _pressedChars.Count; i++)
			io.AddInputCharacter(_pressedChars[i]);

		_pressedChars.Clear();

		io.KeyCtrl = Input.IsKeyHeld(Keys.ControlLeft) || Input.IsKeyHeld(Keys.ControlRight);
		io.KeyAlt = Input.IsKeyHeld(Keys.AltLeft) || Input.IsKeyHeld(Keys.AltRight);
		io.KeyShift = Input.IsKeyHeld(Keys.ShiftLeft) || Input.IsKeyHeld(Keys.ShiftRight);
		io.KeySuper = Input.IsKeyHeld(Keys.SuperLeft) || Input.IsKeyHeld(Keys.SuperRight);
	}

	public void PressKey(Keys keys, InputAction state)
	{
		if (state is not (InputAction.Press or InputAction.Repeat))
			return;

		if (_controlKeys.Contains(keys))
		{
			_pressedControlKeys.Add(keys);
		}
		else
		{
			ImGuiIOPtr io = ImGui.GetIO();
			bool shift = io.KeyShift;
			char? c = keys.GetChar(shift);

			if (c.HasValue)
				_pressedChars.Add(c.Value);
		}
	}

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

		Matrix4x4 orthoProjection = Matrix4x4.CreateOrthographicOffCenter(
			left: drawDataPtr.DisplayPos.X,
			right: drawDataPtr.DisplayPos.X + drawDataPtr.DisplaySize.X,
			bottom: drawDataPtr.DisplayPos.Y + drawDataPtr.DisplaySize.Y,
			top: drawDataPtr.DisplayPos.Y,
			zNearPlane: -1,
			zFarPlane: 1);

		_useShader.Invoke(orthoProjection);

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
