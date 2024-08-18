using Silk.NET.OpenGL;
using SimpleLevelEditorV2.Rendering.Internals;
using System.Numerics;

namespace SimpleLevelEditorV2.Rendering;

public sealed class SceneFramebuffer
{
	private readonly SceneRenderer _sceneRenderer = new();

	private Vector2 _cachedFramebufferSize;
	private uint _framebufferId;

	public uint FramebufferTextureId { get; private set; }

	public unsafe GLEnum Initialize(GL gl, Vector2 framebufferSize)
	{
		if (_cachedFramebufferSize == framebufferSize)
			return GLEnum.None;

		if (_framebufferId != 0)
			gl.DeleteFramebuffer(_framebufferId);

		if (FramebufferTextureId != 0)
			gl.DeleteTexture(FramebufferTextureId);

		_framebufferId = gl.GenFramebuffer();
		gl.BindFramebuffer(FramebufferTarget.Framebuffer, _framebufferId);

		FramebufferTextureId = gl.GenTexture();
		gl.BindTexture(TextureTarget.Texture2D, FramebufferTextureId);
		gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, (uint)framebufferSize.X, (uint)framebufferSize.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, null);
		gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
		gl.TexParameterI(TextureTarget.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
		gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, FramebufferTextureId, 0);

		uint rbo = gl.GenRenderbuffer();
		gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);

		gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent24, (uint)framebufferSize.X, (uint)framebufferSize.Y);
		gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rbo);

		GLEnum framebufferStatus = gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer);

		gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		gl.DeleteRenderbuffer(rbo);

		_cachedFramebufferSize = framebufferSize;

		return framebufferStatus;
	}

	public unsafe void RenderFramebuffer(GL gl, RenderData renderData)
	{
		gl.BindFramebuffer(FramebufferTarget.Framebuffer, _framebufferId);

		// Keep track of the original viewport, so we can restore it later.
		Span<int> originalViewport = stackalloc int[4];
		gl.GetInteger(GLEnum.Viewport, originalViewport);
		gl.Viewport(0, 0, (uint)renderData.Size.X, (uint)renderData.Size.Y);

		gl.ClearColor(0.3f, 0.3f, 0.3f, 0);
		gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		gl.Enable(EnableCap.DepthTest);
		gl.Enable(EnableCap.Blend);
		gl.Enable(EnableCap.CullFace);
		gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		_sceneRenderer.RenderScene(gl, renderData);

		gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}
}
