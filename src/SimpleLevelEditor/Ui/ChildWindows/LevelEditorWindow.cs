using ImGuiNET;
using Silk.NET.GLFW;
using Silk.NET.OpenGL;
using SimpleLevelEditor.Content;
using SimpleLevelEditor.Maths;
using SimpleLevelEditor.Model;
using SimpleLevelEditor.Model.Enums;
using SimpleLevelEditor.Rendering;
using SimpleLevelEditor.State;
using SimpleLevelEditor.Utils;
using static SimpleLevelEditor.Graphics;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class LevelEditorWindow
{
	private static readonly float[] _snapPoints = { 0, 0.125f, 0.25f, 0.5f, 1, 2, 4, 8 };
	private static readonly uint _lineVao = VaoUtils.CreateLineVao(new[] { Vector3.Zero, Vector3.UnitZ });
	private static readonly uint _cubeVao = VaoUtils.CreateLineVao(new Vector3[]
	{
		new(-0.5f, -0.5f, -0.5f),
		new(-0.5f, -0.5f, 0.5f),
		new(-0.5f, 0.5f, -0.5f),
		new(-0.5f, 0.5f, 0.5f),
		new(0.5f, -0.5f, -0.5f),
		new(0.5f, -0.5f, 0.5f),
		new(0.5f, 0.5f, -0.5f),
		new(0.5f, 0.5f, 0.5f),

		new(-0.5f, -0.5f, -0.5f),
		new(-0.5f, 0.5f, -0.5f),
		new(-0.5f, -0.5f, 0.5f),
		new(-0.5f, 0.5f, 0.5f),
		new(0.5f, -0.5f, -0.5f),
		new(0.5f, 0.5f, -0.5f),
		new(0.5f, -0.5f, 0.5f),
		new(0.5f, 0.5f, 0.5f),

		new(-0.5f, -0.5f, -0.5f),
		new(0.5f, -0.5f, -0.5f),
		new(-0.5f, -0.5f, 0.5f),
		new(0.5f, -0.5f, 0.5f),
		new(-0.5f, 0.5f, -0.5f),
		new(0.5f, 0.5f, -0.5f),
		new(-0.5f, 0.5f, 0.5f),
		new(0.5f, 0.5f, 0.5f),
	});

	private static Vector2 _cachedSize;
	private static uint _textureHandle;
	private static uint _framebuffer;

	private static float _gridSnap = 1;
	private static Vector3 _targetPosition;
	private static Ray _ray;

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
			DebugState.AddWarning("Framebuffer is not complete");

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
			RenderFramebuffer(framebufferSize);

			CalculateTargetPosition(cursorScreenPos, framebufferSize);

			ImDrawListPtr drawList = ImGui.GetWindowDrawList();
			drawList.AddImage((IntPtr)_textureHandle, cursorScreenPos, cursorScreenPos + framebufferSize, Vector2.UnitY, Vector2.UnitX);

			Vector2 cursorPosition = ImGui.GetCursorPos();

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

			if (isFocused && ObjectCreatorState.SelectedMeshName != null && Input.IsButtonPressed(MouseButton.Left) && !LevelState.Level.WorldObjects.Exists(wo => wo.Position == _targetPosition))
			{
				WorldObject worldObject = new()
				{
					Position = _targetPosition,
					Mesh = ObjectCreatorState.SelectedMeshName,
					Texture = string.Empty,
					Scale = new(1),
					Rotation = new(0),
					BoundingMesh = string.Empty,
					Values = WorldObjectValues.None,
				};
				LevelState.Level.WorldObjects.Add(worldObject);
			}
		}

		ImGui.EndChild(); // End Level Editor
	}

	private static void CalculateTargetPosition(Vector2 origin, Vector2 size)
	{
		Vector2 mousePosition = Input.GetMousePosition() - origin;
		Vector2 normalizedMousePosition = new Vector2(mousePosition.X / size.X - 0.5f, -(mousePosition.Y / size.Y - 0.5f)) * 2;
		Vector3 point = Camera3d.GetMouseWorldPosition(normalizedMousePosition, new(Vector3.UnitY, 0f));

		if (_gridSnap > 0)
		{
			point.X = MathF.Round(point.X / _gridSnap) * _gridSnap;
			point.Y = MathF.Round(point.Y / _gridSnap) * _gridSnap;
			point.Z = MathF.Round(point.Z / _gridSnap) * _gridSnap;
		}

		_targetPosition = point;
		_ray = Camera3d.GetMouseRay(normalizedMousePosition);
	}

	private static unsafe void RenderFramebuffer(Vector2 size)
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

		RenderScene();

		Gl.Viewport(originalViewport[0], originalViewport[1], (uint)originalViewport[2], (uint)originalViewport[3]);
		Gl.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
	}

	private static void RenderScene()
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
		RenderSelection(meshShader);
	}

	private static void RenderLines(ShaderCacheEntry lineShader)
	{
		int viewUniform = lineShader.GetUniformLocation("view");
		int projectionUniform = lineShader.GetUniformLocation("projection");
		int modelUniform = lineShader.GetUniformLocation("model");
		int colorUniform = lineShader.GetUniformLocation("color");

		ShaderUniformUtils.Set(viewUniform, Camera3d.ViewMatrix);
		ShaderUniformUtils.Set(projectionUniform, Camera3d.Projection);
		Gl.BindVertexArray(_lineVao);
		Gl.LineWidth(4);

		// X axis
		ShaderUniformUtils.Set(modelUniform, Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2));
		ShaderUniformUtils.Set(colorUniform, new Vector4(1, 0, 0, 1));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 6);

		// Y axis
		ShaderUniformUtils.Set(modelUniform, Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, MathF.PI * 1.5f));
		ShaderUniformUtils.Set(colorUniform, new Vector4(0, 1, 0, 1));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 6);

		// Z axis
		ShaderUniformUtils.Set(modelUniform, Matrix4x4.Identity);
		ShaderUniformUtils.Set(colorUniform, new Vector4(0, 0, 1, 1));
		Gl.DrawArrays(PrimitiveType.Lines, 0, 6);

		// Grid
		ShaderUniformUtils.Set(colorUniform, new Vector4(0.8f, 0.8f, 0.8f, 1));
		Gl.LineWidth(1);

		const int min = -10;
		const int max = 10;
		const float height = 0;
		Vector3 scale = new(1, 1, max - min);
		Matrix4x4 scaleMat = Matrix4x4.CreateScale(scale);
		for (int i = min; i <= max; i++)
		{
			ShaderUniformUtils.Set(modelUniform, scaleMat * Matrix4x4.CreateTranslation(i, height, min));
			Gl.DrawArrays(PrimitiveType.Lines, 0, 6);

			ShaderUniformUtils.Set(modelUniform, scaleMat * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, MathF.PI / 2) * Matrix4x4.CreateTranslation(min, height, i));
			Gl.DrawArrays(PrimitiveType.Lines, 0, 6);
		}

		// Bounding boxes
		Gl.BindVertexArray(_cubeVao);
		ShaderUniformUtils.Set(colorUniform, new Vector4(1, 0, 0.8f, 1));
		for (int i = 0; i < LevelState.Level.WorldObjects.Count; i++)
		{
			WorldObject worldObject = LevelState.Level.WorldObjects[i];
			MeshContainer.Entry? mesh = MeshContainer.GetMesh(worldObject.Mesh);
			if (mesh == null)
				continue;

			Vector3 bbScale = worldObject.Scale * (mesh.BoundingMax - mesh.BoundingMin);
			float maxScale = Math.Max(bbScale.X, Math.Max(bbScale.Y, bbScale.Z));
			Sphere sphere = new(worldObject.Position, maxScale);
			Vector3? intersection = _ray.Intersects(sphere);
			if (intersection.HasValue)
				ShaderUniformUtils.Set(colorUniform, new Vector4(1, 1, 0, 1));
			else
				ShaderUniformUtils.Set(colorUniform, new Vector4(1, 0, 0.8f, 1));

			Vector3 bbOffset = (mesh.BoundingMax + mesh.BoundingMin) / 2;

			Matrix4x4 modelMatrix = Matrix4x4.CreateScale(bbScale) * MathUtils.CreateRotationMatrixFromEulerAngles(worldObject.Rotation) * Matrix4x4.CreateTranslation(worldObject.Position + bbOffset);
			ShaderUniformUtils.Set(modelUniform, modelMatrix);
			Gl.DrawArrays(PrimitiveType.Lines, 0, 24);
		}
	}

	private static unsafe void RenderWorldObjects(ShaderCacheEntry meshShader)
	{
		int modelUniform = meshShader.GetUniformLocation("model");
		for (int i = 0; i < LevelState.Level.WorldObjects.Count; i++)
		{
			WorldObject worldObject = LevelState.Level.WorldObjects[i];
			ShaderUniformUtils.Set(modelUniform, Matrix4x4.CreateScale(worldObject.Scale) * MathUtils.CreateRotationMatrixFromEulerAngles(worldObject.Rotation) * Matrix4x4.CreateTranslation(worldObject.Position));

			MeshContainer.Entry? mesh = MeshContainer.GetMesh(worldObject.Mesh);
			if (mesh == null)
				continue;

			uint? textureId = TextureContainer.GetTexture(worldObject.Texture);
			if (textureId == null)
				continue;

			Gl.BindTexture(TextureTarget.Texture2D, textureId.Value);

			Gl.BindVertexArray(mesh.Vao);
			fixed (uint* index = &mesh.Mesh.Indices[0])
				Gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Mesh.Indices.Length, DrawElementsType.UnsignedInt, index);
		}
	}

	private static unsafe void RenderSelection(ShaderCacheEntry meshShader)
	{
		if (ObjectCreatorState.SelectedMeshName == null)
			return;

		MeshContainer.Entry? mesh = MeshContainer.GetMesh(ObjectCreatorState.SelectedMeshName);
		if (mesh == null)
			return;

		int modelUniform = meshShader.GetUniformLocation("model");
		ShaderUniformUtils.Set(modelUniform, Matrix4x4.CreateTranslation(_targetPosition));
		Gl.BindVertexArray(mesh.Vao);
		fixed (uint* i = &mesh.Mesh.Indices[0])
			Gl.DrawElements(PrimitiveType.Triangles, (uint)mesh.Mesh.Indices.Length, DrawElementsType.UnsignedInt, i);
	}
}
