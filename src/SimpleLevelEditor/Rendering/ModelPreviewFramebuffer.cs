using ImGuiNET;
using Silk.NET.OpenGL;
using SimpleLevelEditor.Content;
using SimpleLevelEditor.Extensions;
using SimpleLevelEditor.State.Messages;
using static SimpleLevelEditor.Graphics;

namespace SimpleLevelEditor.Rendering;

public class ModelPreviewFramebuffer
{
	private const int _fieldOfView = 2;

	private readonly Model _model;
	private readonly float _zoom;
	private readonly Vector3 _origin;

	private Vector2 _cachedFramebufferSize;
	private Matrix4x4 _projection;
	private float _timer;
	private uint _framebufferId;

	public ModelPreviewFramebuffer(Model model)
	{
		_model = model;

		_zoom = model.BoundingSphereRadius * 2f / MathF.Tan(_fieldOfView / 2f);
		_origin = model.BoundingSphereOrigin;
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
			MessagesState.AddError($"Model preview framebuffer for '{_model.AbsolutePath}' is not complete.");

		Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		Gl.DeleteRenderbuffer(rbo);

		_cachedFramebufferSize = framebufferSize;

		const float nearPlaneDistance = 0.05f;
		const float farPlaneDistance = 100f;
		float aspectRatio = framebufferSize.X / framebufferSize.Y;
		_projection = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 4 * _fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
	}

	public void Destroy()
	{
		Gl.DeleteFramebuffer(_framebufferId);
		Gl.DeleteTexture(FramebufferTextureId);
	}

	public unsafe void Render(Vector4 backgroundColor, Vector2 size)
	{
		Rebuild(size);

		Gl.BindFramebuffer(FramebufferTarget.Framebuffer, _framebufferId);

		// Keep track of the original viewport, so we can restore it later.
		Span<int> originalViewport = stackalloc int[4];
		Gl.GetInteger(GLEnum.Viewport, originalViewport);
		Gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);

		Gl.ClearColor(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, backgroundColor.W);
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

		ShaderCacheEntry meshShader = InternalContent.Shaders["Mesh"];
		Gl.UseProgram(meshShader.Id);

		Quaternion cameraRotation = Quaternion.CreateFromYawPitchRoll(_timer, 0.5f, 0);
		Vector3 cameraPosition = _origin + Vector3.Transform(new Vector3(0, 0, -_zoom), cameraRotation);
		Vector3 upDirection = Vector3.Transform(Vector3.UnitY, cameraRotation);
		Vector3 lookDirection = Vector3.Transform(Vector3.UnitZ, cameraRotation);
		Matrix4x4 viewMatrix = Matrix4x4.CreateLookAt(cameraPosition, cameraPosition + lookDirection, upDirection);

		Gl.UniformMatrix4x4(meshShader.GetUniformLocation("view"), viewMatrix);
		Gl.UniformMatrix4x4(meshShader.GetUniformLocation("projection"), _projection);
		Gl.UniformMatrix4x4(meshShader.GetUniformLocation("model"), Matrix4x4.Identity);

		for (int i = 0; i < _model.Meshes.Count; i++)
		{
			Mesh mesh = _model.Meshes[i];

			Material? materialData = _model.GetMaterial(mesh.MaterialName);
			if (materialData == null)
				continue;

			uint textureId = TextureContainer.GetTexture(materialData.DiffuseMap.TextureData);
			Gl.BindTexture(TextureTarget.Texture2D, textureId);

			Gl.BindVertexArray(mesh.MeshVao);
			fixed (uint* index = &mesh.Geometry.Indices[0])
				Gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Geometry.Indices.Length, DrawElementsType.UnsignedInt, index);
		}
	}
}
