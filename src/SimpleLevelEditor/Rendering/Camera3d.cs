using Silk.NET.GLFW;
using SimpleLevelEditor.Extensions;

namespace SimpleLevelEditor.Rendering;

public static class Camera3d
{
	public const MouseButton LookButton = MouseButton.Right;
	private const MouseButton _panButton = MouseButton.Middle;

	private const int _fieldOfView = 2;
	private static Vector2 _originalCursor = Input.GlfwInput.CursorPosition;

	private static float _yaw = MathF.PI * 0.25f;
	private static float _pitch = -0.5f;

	public static float Zoom = 5;

	private static Vector3 _focusPoint;

	public static Quaternion Rotation { get; private set; } = Quaternion.CreateFromYawPitchRoll(_yaw, -_pitch, 0);
	public static Vector3 Position { get; private set; }

	public static Matrix4x4 Projection { get; private set; }
	public static Matrix4x4 ViewMatrix { get; private set; }
	public static float AspectRatio { get; set; }
	public static CameraMode Mode { get; private set; }
	public static Vector3 FocusPointTarget { get; private set; }

	public static Vector3 UpDirection => Vector3.Transform(Vector3.UnitY, Rotation);
	public static Vector3 LookDirection => Vector3.Transform(Vector3.UnitZ, Rotation);

	public static void SetFocusPoint(Vector3 focusPoint)
	{
		FocusPointTarget = focusPoint;
	}

	private static void SetFocusPointHard(Vector3 focusPoint)
	{
		FocusPointTarget = focusPoint;
		_focusPoint = focusPoint;
	}

	public static void Update(float dt, bool isFocused)
	{
		if (isFocused)
		{
			HandleMouse();

			float scroll = Input.GlfwInput.MouseWheelY;
			if (!scroll.IsZero() && !Input.GlfwInput.IsKeyDown(Keys.ControlLeft) && !Input.GlfwInput.IsKeyDown(Keys.ControlRight))
				Zoom = Math.Max(Zoom - scroll, 1);

			if (!Input.GlfwInput.IsKeyDown(Keys.ControlLeft) && !Input.GlfwInput.IsKeyDown(Keys.ControlRight))
			{
				const float speed = 15;
				if (Input.GlfwInput.IsKeyDown(Keys.W))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(0, 0, speed), Rotation) * App.Instance.FrameTime);
				if (Input.GlfwInput.IsKeyDown(Keys.S))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(0, 0, -speed), Rotation) * App.Instance.FrameTime);
				if (Input.GlfwInput.IsKeyDown(Keys.A))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(speed, 0, 0), Rotation) * App.Instance.FrameTime);
				if (Input.GlfwInput.IsKeyDown(Keys.D))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(-speed, 0, 0), Rotation) * App.Instance.FrameTime);
				if (Input.GlfwInput.IsKeyDown(Keys.Space))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(0, speed, 0), Rotation) * App.Instance.FrameTime);
				if (Input.GlfwInput.IsKeyDown(Keys.ShiftLeft))
					SetFocusPointHard(FocusPointTarget + Vector3.Transform(new Vector3(0, -speed, 0), Rotation) * App.Instance.FrameTime);
			}
		}
		else
		{
			ResetCameraMode();
		}

		_focusPoint = Vector3.Lerp(_focusPoint, FocusPointTarget, dt * 10);
		Position = _focusPoint + Vector3.Transform(new Vector3(0, 0, -Zoom), Rotation);

		ViewMatrix = Matrix4x4.CreateLookAt(Position, Position + LookDirection, UpDirection);

		const float nearPlaneDistance = 0.05f;
		const float farPlaneDistance = 10000f;
		Projection = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 4 * _fieldOfView, AspectRatio, nearPlaneDistance, farPlaneDistance);
	}

	private static unsafe void HandleMouse()
	{
		Vector2 cursor = Input.GlfwInput.CursorPosition;

		if (Mode == CameraMode.None && (Input.GlfwInput.IsMouseButtonDown(LookButton) || Input.GlfwInput.IsMouseButtonDown(_panButton)))
		{
			Graphics.Glfw.SetInputMode(Graphics.Window, CursorStateAttribute.Cursor, CursorModeValue.CursorHidden);
			_originalCursor = cursor;
			Mode = Input.GlfwInput.IsMouseButtonDown(LookButton) ? CameraMode.Look : CameraMode.Pan;
		}
		else if (Mode != CameraMode.None && !Input.GlfwInput.IsMouseButtonDown(LookButton) && !Input.GlfwInput.IsMouseButtonDown(_panButton))
		{
			ResetCameraMode();
		}

		if (Mode == CameraMode.None)
			return;

		Vector2 delta = cursor - _originalCursor;
		if (Mode == CameraMode.Look)
		{
			const float lookSpeed = 20;
			_yaw -= lookSpeed * delta.X * 0.0001f;
			_pitch -= lookSpeed * delta.Y * 0.0001f;
			Rotation = Quaternion.CreateFromYawPitchRoll(_yaw, -_pitch, 0);

			Graphics.Glfw.SetCursorPos(Graphics.Window, _originalCursor.X, _originalCursor.Y);
		}
		else if (Mode == CameraMode.Pan)
		{
			float multiplier = 0.0005f * Zoom;
			SetFocusPointHard(FocusPointTarget - Vector3.Transform(new Vector3(-delta.X * multiplier, -delta.Y * multiplier, 0), Rotation));

			Graphics.Glfw.SetCursorPos(Graphics.Window, _originalCursor.X, _originalCursor.Y);
		}
	}

	private static unsafe void ResetCameraMode()
	{
		Graphics.Glfw.SetInputMode(Graphics.Window, CursorStateAttribute.Cursor, CursorModeValue.CursorNormal);
		Mode = CameraMode.None;
	}

	public static Vector3 GetMouseWorldPosition(Vector2 normalizedMousePosition, Plane plane)
	{
		Vector3 nearSource = new(normalizedMousePosition.X, normalizedMousePosition.Y, 0f);
		Vector3 farSource = new(normalizedMousePosition.X, normalizedMousePosition.Y, 1f);
		Vector3 nearPoint = UnProject(nearSource, Projection, ViewMatrix, Matrix4x4.Identity);
		Vector3 farPoint = UnProject(farSource, Projection, ViewMatrix, Matrix4x4.Identity);

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

	public static Vector2 GetScreenPositionFrom3dPoint(Vector3 position, Vector2 framebufferSize)
	{
		Vector3 clipSpace = Vector3.Transform(position, ViewMatrix * Projection);
		return ToScreenSpace(clipSpace, framebufferSize);
	}

	private static Vector2 ToScreenSpace(Vector3 position, Vector2 framebufferSize)
	{
		float x = framebufferSize.X * 0.5f + position.X * framebufferSize.X * 0.5f / position.Z;
		float y = framebufferSize.Y * 0.5f + position.Y * framebufferSize.Y * 0.5f / position.Z;
		return new Vector2(x, framebufferSize.Y - y);
	}
}
