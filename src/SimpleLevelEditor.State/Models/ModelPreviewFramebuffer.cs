using ImGuiNET;
using Silk.NET.OpenGL;
using SimpleLevelEditor.State.Extensions;
using SimpleLevelEditor.State.Messages;
using System.Numerics;

namespace SimpleLevelEditor.State.Models;

public sealed class ModelPreviewFramebuffer
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

	private unsafe void Rebuild(GL gl, Vector2 framebufferSize)
	{
		if (_cachedFramebufferSize == framebufferSize)
			return;

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

		if (gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
			MessagesState.AddError($"Model preview framebuffer for '{_model.AbsolutePath}' is not complete.");

		gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		gl.DeleteRenderbuffer(rbo);

		_cachedFramebufferSize = framebufferSize;

		const float nearPlaneDistance = 0.05f;
		const float farPlaneDistance = 100f;
		float aspectRatio = framebufferSize.X / framebufferSize.Y;
		_projection = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 4 * _fieldOfView, aspectRatio, nearPlaneDistance, farPlaneDistance);
	}

	public void Destroy(GL gl)
	{
		gl.DeleteFramebuffer(_framebufferId);
		gl.DeleteTexture(FramebufferTextureId);
	}

	public unsafe void Render(GL gl, Vector4 backgroundColor, Vector2 size)
	{
		Rebuild(gl, size);

		gl.BindFramebuffer(FramebufferTarget.Framebuffer, _framebufferId);

		// Keep track of the original viewport, so we can restore it later.
		Span<int> originalViewport = stackalloc int[4];
		gl.GetInteger(GLEnum.Viewport, originalViewport);
		gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);

		gl.ClearColor(backgroundColor.X, backgroundColor.Y, backgroundColor.Z, backgroundColor.W);
		gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		gl.Enable(EnableCap.DepthTest);
		gl.Enable(EnableCap.Blend);
		gl.Enable(EnableCap.CullFace);
		gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		RenderGeometry(gl);

		gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}

	private unsafe void RenderGeometry(GL gl)
	{
		_timer += ImGui.GetIO().DeltaTime;

		ShaderCacheEntry meshShader = InternalContent.Shaders["Mesh"];
		gl.UseProgram(meshShader.Id);

		Quaternion cameraRotation = Quaternion.CreateFromYawPitchRoll(_timer, 0.5f, 0);
		Vector3 cameraPosition = _origin + Vector3.Transform(new Vector3(0, 0, -_zoom), cameraRotation);
		Vector3 upDirection = Vector3.Transform(Vector3.UnitY, cameraRotation);
		Vector3 lookDirection = Vector3.Transform(Vector3.UnitZ, cameraRotation);
		Matrix4x4 viewMatrix = Matrix4x4.CreateLookAt(cameraPosition, cameraPosition + lookDirection, upDirection);

		gl.UniformMatrix4x4(meshShader.GetUniformLocation(gl, "view"), viewMatrix);
		gl.UniformMatrix4x4(meshShader.GetUniformLocation(gl, "projection"), _projection);
		gl.UniformMatrix4x4(meshShader.GetUniformLocation(gl, "model"), Matrix4x4.Identity);

		for (int i = 0; i < _model.Meshes.Count; i++)
		{
			Mesh mesh = _model.Meshes[i];

			Material? materialData = _model.GetMaterial(mesh.MaterialName);
			if (materialData == null)
				continue;

			uint textureId = TextureContainer.GetTexture(gl, materialData.DiffuseMap.TextureData);
			gl.BindTexture(TextureTarget.Texture2D, textureId);

			gl.BindVertexArray(mesh.MeshVao);
			fixed (uint* index = &mesh.Geometry.Indices[0])
				gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Geometry.Indices.Length, DrawElementsType.UnsignedInt, index);
		}
	}
}
