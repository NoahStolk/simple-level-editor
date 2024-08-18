using ImGuiGlfw;
using Silk.NET.GLFW;
using System.Numerics;

namespace SimpleLevelEditorV2.States.LevelEditor;

public sealed class CameraState
{
	public const MouseButton LookButton = MouseButton.Right;
	private const MouseButton _panButton = MouseButton.Middle;
	private const int _fieldOfView = 2;

	private Vector2? _originalCursor;

	public float Yaw { get; private set; } = MathF.PI * 0.25f;
	public float Pitch { get; private set; } = -0.5f;
	public float Zoom { get; private set; } = 5;

	public Vector3 Position { get; private set; }

	public Vector3 FocusPoint { get; private set; }
	public Vector3 FocusPointTarget { get; private set; }

	public CameraMode Mode { get; private set; }

	private Quaternion Rotation => Quaternion.CreateFromYawPitchRoll(Yaw, -Pitch, 0);
	public Vector3 UpDirection => Vector3.Transform(Vector3.UnitY, Rotation);
	public Vector3 LookDirection => Vector3.Transform(Vector3.UnitZ, Rotation);

	public Matrix4x4 ProjectionMatrix { get; private set; }
	public Matrix4x4 ViewMatrix { get; private set; }
	public float AspectRatio { get; set; }

	public void SetFocusPoint(Vector3 focusPoint)
	{
		FocusPointTarget = focusPoint;
	}

	private void SetFocusPointHard(Vector3 focusPoint)
	{
		FocusPointTarget = focusPoint;
		FocusPoint = focusPoint;
	}

	public unsafe void Update(float frameTime, GlfwInput input, Glfw glfw, WindowHandle* window, float dt, bool isFocused)
	{
		if (isFocused)
		{
			HandleMouse(input, glfw, window);

			float scroll = input.MouseWheelY;
			if (!scroll.IsZero() && !input.IsKeyDown(Keys.ControlLeft) && !input.IsKeyDown(Keys.ControlRight))
				Zoom = Math.Max(Zoom - scroll, 1);

			if (!input.IsKeyDown(Keys.ControlLeft) && !input.IsKeyDown(Keys.ControlRight))
			{
				const float speed = 15;
				if (input.IsKeyDown(Keys.W))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(0, 0, speed), Rotation) * frameTime);
				if (input.IsKeyDown(Keys.S))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(0, 0, -speed), Rotation) * frameTime);
				if (input.IsKeyDown(Keys.A))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(speed, 0, 0), Rotation) * frameTime);
				if (input.IsKeyDown(Keys.D))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(-speed, 0, 0), Rotation) * frameTime);
				if (input.IsKeyDown(Keys.Space))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(0, speed, 0), Rotation) * frameTime);
				if (input.IsKeyDown(Keys.ShiftLeft))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(0, -speed, 0), Rotation) * frameTime);
			}
		}
		else
		{
			ResetCameraMode(glfw, window);
		}

		FocusPoint = Vector3.Lerp(FocusPoint, FocusPointTarget, dt * 10);
		Position = FocusPoint + Vector3.Transform(new Vector3(0, 0, -Zoom), Rotation);

		ViewMatrix = Matrix4x4.CreateLookAt(Position, Position + LookDirection, UpDirection);

		const float nearPlaneDistance = 0.05f;
		const float farPlaneDistance = 10000f;
		ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 4 * _fieldOfView, AspectRatio, nearPlaneDistance, farPlaneDistance);
	}

	private unsafe void HandleMouse(GlfwInput input, Glfw glfw, WindowHandle* window)
	{
		_originalCursor ??= input.CursorPosition;

		Vector2 cursor = input.CursorPosition;

		if (Mode == CameraMode.None && (input.IsMouseButtonDown(LookButton) || input.IsMouseButtonDown(_panButton)))
		{
			glfw.SetInputMode(window, CursorStateAttribute.Cursor, CursorModeValue.CursorHidden);
			_originalCursor = cursor;
			Mode = input.IsMouseButtonDown(LookButton) ? CameraMode.Look : CameraMode.Pan;
		}
		else if (Mode != CameraMode.None && !input.IsMouseButtonDown(LookButton) && !input.IsMouseButtonDown(_panButton))
		{
			ResetCameraMode(glfw, window);
		}

		if (Mode == CameraMode.None)
			return;

		Vector2 delta = cursor - _originalCursor.Value;
		if (Mode == CameraMode.Look)
		{
			const float lookSpeed = 20;
			Yaw -= lookSpeed * delta.X * 0.0001f;
			Pitch -= lookSpeed * delta.Y * 0.0001f;

			glfw.SetCursorPos(window, _originalCursor.Value.X, _originalCursor.Value.Y);
		}
		else if (Mode == CameraMode.Pan)
		{
			float multiplier = 0.0005f * Zoom;
			SetFocusPointHard(FocusPointTarget - Vector3.Transform(new Vector3(-delta.X * multiplier, -delta.Y * multiplier, 0), Rotation));

			glfw.SetCursorPos(window, _originalCursor.Value.X, _originalCursor.Value.Y);
		}
	}

	private unsafe void ResetCameraMode(Glfw glfw, WindowHandle* window)
	{
		glfw.SetInputMode(window, CursorStateAttribute.Cursor, CursorModeValue.CursorNormal);
		Mode = CameraMode.None;
	}

	public Vector3 GetMouseWorldPosition(Vector2 normalizedMousePosition, Plane plane)
	{
		Vector3 nearSource = new(normalizedMousePosition.X, normalizedMousePosition.Y, 0f);
		Vector3 farSource = new(normalizedMousePosition.X, normalizedMousePosition.Y, 1f);
		Vector3 nearPoint = UnProject(nearSource, ProjectionMatrix, ViewMatrix, Matrix4x4.Identity);
		Vector3 farPoint = UnProject(farSource, ProjectionMatrix, ViewMatrix, Matrix4x4.Identity);

		// Create a ray from the near clip plane to the far clip plane.
		Vector3 rayDirection = Vector3.Normalize(farPoint - nearPoint);

		// Calculate distance of intersection point from ray.Position.
		float denominator = Vector3.Dot(plane.Normal, rayDirection);
		float numerator = Vector3.Dot(plane.Normal, nearPoint) + plane.D;
		float t = -(numerator / denominator);

		// Calculate the picked position on the y = 0 plane.
		return nearPoint + rayDirection * t;
	}

	private static Vector3 UnProject(Vector3 source, Matrix4x4 projection, Matrix4x4 view, Matrix4x4 world)
	{
		Matrix4x4.Invert(Matrix4x4.Multiply(Matrix4x4.Multiply(world, view), projection), out Matrix4x4 matrix);
		Vector3 vector = Vector3.Transform(source, matrix);
		float a = source.X * matrix.M14 + source.Y * matrix.M24 + source.Z * matrix.M34 + matrix.M44;
		if (WithinEpsilon(a, 1f))
			return vector;

		return vector / a;

		static bool WithinEpsilon(float a, float b)
		{
			float num = a - b;
			return num is >= -float.Epsilon and <= float.Epsilon;
		}
	}

	public Vector2 GetScreenPositionFrom3dPoint(Vector3 position, Vector2 framebufferSize)
	{
		Vector3 clipSpace = Vector3.Transform(position, ViewMatrix * ProjectionMatrix);
		return ToScreenSpace(clipSpace, framebufferSize);
	}

	private static Vector2 ToScreenSpace(Vector3 position, Vector2 framebufferSize)
	{
		float x = framebufferSize.X * 0.5f + position.X * framebufferSize.X * 0.5f / position.Z;
		float y = framebufferSize.Y * 0.5f + position.Y * framebufferSize.Y * 0.5f / position.Z;
		return new Vector2(x, framebufferSize.Y - y);
	}
}
