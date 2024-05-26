using Silk.NET.OpenGL;
using SimpleLevelEditor.Rendering.Scene;
using SimpleLevelEditor.State.Messages;
using static SimpleLevelEditor.Graphics;

namespace SimpleLevelEditor.Rendering;

public static class SceneFramebuffer
{
	private static Vector2 _cachedFramebufferSize;
	private static uint _framebufferId;

	public static uint FramebufferTextureId { get; private set; }

	public static unsafe void Initialize(Vector2 framebufferSize)
	{
		if (_cachedFramebufferSize == framebufferSize)
			return;

		if (_framebufferId != 0)
			Gl.DeleteFramebuffer(_framebufferId);

		if (FramebufferTextureId != 0)
			Gl.DeleteTexture(FramebufferTextureId);

		_framebufferId = Gl.GenFramebuffer();
		Gl.BindFramebuffer(FramebufferTarget.Framebuffer, _framebufferId);

		FramebufferTextureId = Gl.GenTexture();
		Gl.BindTexture(TextureTarget.Texture2D, FramebufferTextureId);
		Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, (uint)framebufferSize.X, (uint)framebufferSize.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, null);
		Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
		Gl.TexParameterI(TextureTarget.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
		Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, FramebufferTextureId, 0);

		uint rbo = Gl.GenRenderbuffer();
		Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);

		Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent24, (uint)framebufferSize.X, (uint)framebufferSize.Y);
		Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rbo);

		if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
			MessagesState.AddError("Framebuffer for scene is not complete.");

		Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		Gl.DeleteRenderbuffer(rbo);

		_cachedFramebufferSize = framebufferSize;
	}

	public static unsafe void RenderFramebuffer(Vector2 size)
	{
		Gl.BindFramebuffer(FramebufferTarget.Framebuffer, _framebufferId);

		// Keep track of the original viewport, so we can restore it later.
		Span<int> originalViewport = stackalloc int[4];
		Gl.GetInteger(GLEnum.Viewport, originalViewport);
		Gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);

		Gl.ClearColor(0.3f, 0.3f, 0.3f, 0);
		Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		Gl.Enable(EnableCap.DepthTest);
		Gl.Enable(EnableCap.Blend);
		Gl.Enable(EnableCap.CullFace);
		Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		SceneRenderer.RenderScene();

		Gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}
}
