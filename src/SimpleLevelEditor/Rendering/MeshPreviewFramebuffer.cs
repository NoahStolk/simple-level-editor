using ImGuiNET;
using Silk.NET.OpenGL;
using SimpleLevelEditor.Content;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.State;
using static SimpleLevelEditor.Graphics;

namespace SimpleLevelEditor.Rendering;

public class MeshPreviewFramebuffer
{
	private readonly Mesh _mesh;
	private readonly float _zoom;
	private readonly Vector3 _origin;
	private Vector2 _cachedFramebufferSize;
	private Matrix4x4 _projection;
	private float _timer;

	private uint _framebufferId;

	public MeshPreviewFramebuffer(Mesh mesh)
	{
		_mesh = mesh;

		Vector3 distance = mesh.BoundingMax - mesh.BoundingMin;
		_zoom = distance.Length();

		_origin = (mesh.BoundingMin + mesh.BoundingMax) / 2;
	}

	public uint FramebufferTextureId { get; private set; }

	private unsafe void Rebuild(Vector2 framebufferSize)
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
			DebugState.AddWarning("Framebuffer is not complete");

		Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		Gl.DeleteRenderbuffer(rbo);

		_cachedFramebufferSize = framebufferSize;

		const int fieldOfView = 2;
		const float nearPlaneDistance = 0.05f;
		const float farPlaneDistance = 100f;
		float aspectRatio = framebufferSize.X / framebufferSize.Y;
		_projection = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 4 * fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
	}

	public void Destroy()
	{
		Gl.DeleteFramebuffer(_framebufferId);
		Gl.DeleteTexture(FramebufferTextureId);
	}

	public unsafe void Render(Vector2 size)
	{
		Rebuild(size);

		Gl.BindFramebuffer(FramebufferTarget.Framebuffer, _framebufferId);

		// Keep track of the original viewport, so we can restore it later.
		Span<int> originalViewport = stackalloc int[4];
		Gl.GetInteger(GLEnum.Viewport, originalViewport);
		Gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);

		Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		Gl.Enable(EnableCap.DepthTest);
		Gl.Enable(EnableCap.Blend);
		Gl.Enable(EnableCap.CullFace);
		Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		RenderGeometry();

		Gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}

	private unsafe void RenderGeometry()
	{
		_timer += ImGui.GetIO().DeltaTime;

		ShaderCacheEntry lineShader = InternalContent.Shaders["Line"];
		Gl.UseProgram(lineShader.Id);

		Quaternion cameraRotation = Quaternion.CreateFromYawPitchRoll(_timer, 0.5f, 0);
		Vector3 cameraPosition = _origin + Vector3.Transform(new Vector3(0, 0, -_zoom), cameraRotation);
		Vector3 upDirection = Vector3.Transform(Vector3.UnitY, cameraRotation);
		Vector3 lookDirection = Vector3.Transform(Vector3.UnitZ, cameraRotation);
		Matrix4x4 viewMatrix = Matrix4x4.CreateLookAt(cameraPosition, cameraPosition + lookDirection, upDirection);

		Gl.UniformMatrix4x4(lineShader.GetUniformLocation("view"), viewMatrix);
		Gl.UniformMatrix4x4(lineShader.GetUniformLocation("projection"), _projection);

		Gl.Uniform4(lineShader.GetUniformLocation("color"), new Vector4(1, 0, 0, 1));
		Gl.UniformMatrix4x4(lineShader.GetUniformLocation("model"), Matrix4x4.Identity);

		Gl.BindVertexArray(_mesh.LineVao);
		fixed (uint* index = &_mesh.LineIndices[0])
			Gl.DrawElements(PrimitiveType.Lines, (uint)_mesh.LineIndices.Length, DrawElementsType.UnsignedInt, index);
	}
}
