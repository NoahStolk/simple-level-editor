using ImGuiNET;
using Silk.NET.OpenGL;
using SimpleLevelEditor.Content;
using SimpleLevelEditor.Content.Data;
using SimpleLevelEditor.Formats.Level3d;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;
using static SimpleLevelEditor.Graphics;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class LevelEditorWindow
{
	private static readonly float[] _snapPoints = { 0, 0.125f, 0.25f, 0.5f, 1, 2, 4, 8 };
	private static readonly uint _lineVao;

	private static Vector2 _cachedSize;
	private static uint _textureHandle;
	private static uint _framebuffer;

	private static float _gridSnap = 1;

#pragma warning disable S3963
	static unsafe LevelEditorWindow()
#pragma warning restore S3963
	{
		_lineVao = Gl.GenVertexArray();
		Gl.BindVertexArray(_lineVao);

		uint vbo = Gl.GenBuffer();
		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, vbo);

		float[] vertices =
		{
			0, 0, 0,
			0, 0, 1,
		};
		GlUtils.BufferData(BufferTargetARB.ArrayBuffer, vertices, BufferUsageARB.StaticDraw);

		Gl.EnableVertexAttribArray(0);
		Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, (uint)sizeof(Vector3), (void*)0);

		Gl.BindVertexArray(0);
		Gl.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
		Gl.DeleteBuffer(vbo);
	}

	private static unsafe void Initialize(Vector2 size)
	{
		if (_cachedSize == size)
			return;

		if (_framebuffer != 0)
			Gl.DeleteFramebuffer(_framebuffer);

		if (_textureHandle != 0)
			Gl.DeleteTexture(_textureHandle);

		_framebuffer = Gl.GenFramebuffer();
		Gl.BindFramebuffer(FramebufferTarget.Framebuffer, _framebuffer);

		_textureHandle = Gl.GenTexture();
		Gl.BindTexture(TextureTarget.Texture2D, _textureHandle);
		Gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgb, (uint)size.X, (uint)size.Y, 0, PixelFormat.Rgb, PixelType.UnsignedByte, null);
		Gl.TexParameterI(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
		Gl.TexParameterI(TextureTarget.Texture2D, GLEnum.TextureMagFilter, (int)GLEnum.Linear);
		Gl.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, _textureHandle, 0);

		uint rbo = Gl.GenRenderbuffer();
		Gl.BindRenderbuffer(RenderbufferTarget.Renderbuffer, rbo);

		Gl.RenderbufferStorage(RenderbufferTarget.Renderbuffer, InternalFormat.DepthComponent24, (uint)size.X, (uint)size.Y);
		Gl.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, rbo);

		if (Gl.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != GLEnum.FramebufferComplete)
			DebugWindow.Warnings.Add("Framebuffer is not complete.");

		Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
		Gl.DeleteRenderbuffer(rbo);

		_cachedSize = size;
	}

	public static void Render(Vector2 size)
	{
		Vector2 framebufferSize = size - new Vector2(0, 32);

		Initialize(framebufferSize);
		if (ImGui.BeginChild("Level Editor", size, true))
		{
			ImGui.SeparatorText("Level Editor");

			Camera3d.AspectRatio = framebufferSize.X / framebufferSize.Y;
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			RenderFramebuffer(cursorScreenPos, framebufferSize);

			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			drawList.AddImage((IntPtr)_textureHandle, cursorScreenPos, cursorScreenPos + framebufferSize, Vector2.UnitY, Vector2.UnitX);

			Vector2 cursorPosition = ImGui.GetCursorPos();
			if (ImGui.Button("Set test level"))
				LevelState.SetTestLevel();

			ImGui.Text("Snap");
			for (int i = 0; i < _snapPoints.Length; i++)
			{
				if (i != 0)
					ImGui.SameLine();

				if (ImGui.RadioButton(Inline.Span(_snapPoints[i]), Math.Abs(_gridSnap - _snapPoints[i]) < 0.001f))
					_gridSnap = _snapPoints[i];
			}

			ImGui.Text(Inline.Span(Camera3d.Position, "0.00"));

			ImGui.SetCursorPos(cursorPosition);
			ImGui.InvisibleButton("3d_view", framebufferSize);
			bool isFocused = ImGui.IsItemHovered();
			Camera3d.Update(ImGui.GetIO().DeltaTime, isFocused);
		}

		ImGui.EndChild(); // End Level Editor
	}

	private static unsafe void RenderFramebuffer(Vector2 origin, Vector2 size)
	{
		Gl.BindFramebuffer(FramebufferTarget.Framebuffer, _framebuffer);

		// Keep track of the original viewport so we can restore it later.
		Span<int> originalViewport = stackalloc int[4];
		Gl.GetInteger(GLEnum.Viewport, originalViewport);
		Gl.Viewport(0, 0, (uint)size.X, (uint)size.Y);

		Gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

		Gl.Enable(EnableCap.DepthTest);
		Gl.Enable(EnableCap.Blend);
		Gl.Enable(EnableCap.CullFace);
		Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

		RenderScene(origin, size);

		Gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}

	private static void RenderScene(Vector2 origin, Vector2 size)
	{
		ShaderCacheEntry lineShader = ShaderContainer.Shaders["Line"];
		Gl.UseProgram(lineShader.Id);

		RenderLines(lineShader);

		ShaderCacheEntry meshShader = ShaderContainer.Shaders["Mesh"];
		Gl.UseProgram(meshShader.Id);

		int viewUniform = meshShader.GetUniformLocation("view");
		int projectionUniform = meshShader.GetUniformLocation("projection");

		ShaderUniformUtils.Set(viewUniform, Camera3d.ViewMatrix);
		ShaderUniformUtils.Set(projectionUniform, Camera3d.Projection);

		RenderWorldObjects(meshShader);
		RenderSelection(meshShader, origin, size);
	}

	private static void RenderLines(ShaderCacheEntry lineShader)
	{
		int view = lineShader.GetUniformLocation("view");
		int projection = lineShader.GetUniformLocation("projection");
		int model = lineShader.GetUniformLocation("model");
		int color = lineShader.GetUniformLocation("color");

		ShaderUniformUtils.Set(view, Camera3d.ViewMatrix);
		ShaderUniformUtils.Set(projection, Camera3d.Projection);
		Gl.BindVertexArray(_lineVao);

		// X axis
		ShaderUniformUtils.Set(model, Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2));
		ShaderUniformUtils.Set(color, new Vector4(1, 0, 0, 1));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 6);

		// Y axis
		ShaderUniformUtils.Set(model, Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 1.5f));
		ShaderUniformUtils.Set(color, new Vector4(0, 1, 0, 1));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 6);

		// Z axis
		ShaderUniformUtils.Set(model, Matrix4x4.Identity);
		ShaderUniformUtils.Set(color, new Vector4(0, 0, 1, 1));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 6);

		// Grid
		ShaderUniformUtils.Set(color, new Vector4(0.8f, 0.8f, 0.8f, 1));

		const int min = -10;
		const int max = 10;
		const float height = 0.01f;
		Vector3 scale = new(1, 1, max - min);
		Matrix4x4 scaleMat = Matrix4x4.CreateScale(scale);
		for (int i = min; i <= max; i++)
		{
			ShaderUniformUtils.Set(model, scaleMat * Matrix4x4.CreateTranslation(i, height, min));
			Gl.DrawArrays(PrimitiveType.Lines, 0, 6);

			ShaderUniformUtils.Set(model, scaleMat * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2) * Matrix4x4.CreateTranslation(min, height, i));
			Gl.DrawArrays(PrimitiveType.Lines, 0, 6);
		}
	}

	private static unsafe void RenderWorldObjects(ShaderCacheEntry meshShader)
	{
		int modelUniform = meshShader.GetUniformLocation("model");
		for (int i = 0; i < LevelState.Level.WorldObjects.Count; i++)
		{
			WorldObject worldObject = LevelState.Level.WorldObjects[i];
			ShaderUniformUtils.Set(modelUniform, Matrix4x4.CreateScale(worldObject.Scale) * Matrix4x4.CreateFromYawPitchRoll(worldObject.Rotation.X, worldObject.Rotation.Y, worldObject.Rotation.Z) * Matrix4x4.CreateTranslation(worldObject.Position));

			(Mesh Mesh, uint Vao)? mesh = GetMesh(worldObject.MeshId);
			if (mesh == null)
				continue;

			uint? textureId = GetTexture(worldObject.TextureId);
			if (textureId == null)
				continue;

			Gl.BindTexture(TextureTarget.Texture2D, textureId.Value);

			Gl.BindVertexArray(mesh.Value.Vao);
			fixed (uint* index = &mesh.Value.Mesh.Indices[0])
				Gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Value.Mesh.Indices.Length, DrawElementsType.UnsignedInt, index);
		}
	}

	private static unsafe void RenderSelection(ShaderCacheEntry meshShader, Vector2 origin, Vector2 size)
	{
		if (ObjectCreatorState.SelectedMeshName == null)
			return;

		int index = LevelState.Level.Meshes.IndexOf(ObjectCreatorState.SelectedMeshName);
		(Mesh Mesh, uint Vao)? mesh = GetMesh(index);
		if (mesh == null)
			return;

		Vector2 mousePosition = Input.GetMousePosition() - origin;
		Vector2 normalizedMousePosition = new Vector2(mousePosition.X / size.X - 0.5f, -(mousePosition.Y / size.Y - 0.5f)) * 2;
		Vector3 point = Camera3d.GetMouseWorldPosition(normalizedMousePosition, new(Vector3.UnitY, 0f));

		if (_gridSnap > 0)
		{
			point.X = MathF.Round(point.X / _gridSnap) * _gridSnap;
			point.Y = MathF.Round(point.Y / _gridSnap) * _gridSnap;
			point.Z = MathF.Round(point.Z / _gridSnap) * _gridSnap;
		}

		int modelUniform = meshShader.GetUniformLocation("model");
		ShaderUniformUtils.Set(modelUniform, Matrix4x4.CreateTranslation(point));
		Gl.BindVertexArray(mesh.Value.Vao);
		fixed (uint* i = &mesh.Value.Mesh.Indices[0])
			Gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Value.Mesh.Indices.Length, DrawElementsType.UnsignedInt, i);
	}

	private static (Mesh Mesh, uint Vao)? GetMesh(int meshId)
	{
		string? meshName = meshId >= 0 && LevelState.Level.Meshes.Count > meshId ? LevelState.Level.Meshes[meshId] : null;
		if (meshName == null)
		{
			DebugWindow.Warnings.Add("Cannot find mesh name.");
			return null;
		}

		(Mesh Mesh, uint Vao)? mesh = MeshContainer.GetMesh(meshName);
		if (!mesh.HasValue)
			DebugWindow.Warnings.Add("Cannot find mesh.");

		return mesh;
	}

	private static uint? GetTexture(int textureId)
	{
		string? textureName = LevelState.Level.Textures.Count > textureId ? LevelState.Level.Textures[textureId] : null;
		if (textureName == null)
		{
			DebugWindow.Warnings.Add("Cannot find texture name.");
			return null;
		}

		uint? glTextureId = TextureContainer.GetTexture(textureName);
		if (!glTextureId.HasValue)
			DebugWindow.Warnings.Add("Cannot find texture.");

		return glTextureId;
	}
}
