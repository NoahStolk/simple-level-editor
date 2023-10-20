using Silk.NET.GLFW;
using SimpleLevelEditor.Maths;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Rendering;

public static class Camera3d
{
	public const MouseButton LookButton = MouseButton.Right;

	private const int _fieldOfView = 2;
	private static Vector2 _originalCursor = Input.GetMousePosition();
	private static bool _lookEnabled;

	private static float _yaw;
	private static float _pitch;

	private static float _zoom = 5;

	private static Vector3 _focusPointTarget;
	private static Vector3 _focusPoint;

	public static Quaternion Rotation { get; private set; } = Quaternion.Identity;
	public static Vector3 Position { get; private set; }

	public static Matrix4x4 Projection { get; private set; }
	public static Matrix4x4 ViewMatrix { get; private set; }
	public static float AspectRatio { get; set; }

	public static void SetFocusPoint(Vector3 focusPoint)
	{
		_focusPointTarget = focusPoint;
	}

	public static void Update(float dt, bool isFocused)
	{
		if (isFocused)
			HandleMouse();
		else
			StopLook();

		float scroll = Input.GetScroll();
		if (scroll != 0)
			_zoom = Math.Clamp(_zoom - scroll, 1, 30);

		_focusPoint = Vector3.Lerp(_focusPoint, _focusPointTarget, dt * 10);
		Position = _focusPoint + Vector3.Transform(new(0, 0, -_zoom), Rotation);

		Vector3 upDirection = Vector3.Transform(Vector3.UnitY, Rotation);
		Vector3 lookDirection = Vector3.Transform(Vector3.UnitZ, Rotation);
		ViewMatrix = Matrix4x4.CreateLookAt(Position, Position + lookDirection, upDirection);

		const float nearPlaneDistance = 0.05f;
		const float farPlaneDistance = 10000f;
		Projection = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 4 * _fieldOfView, AspectRatio, nearPlaneDistance, farPlaneDistance);
	}

	private static unsafe void HandleMouse()
	{
		const float lookSpeed = 20;

		Vector2 cursor = Input.GetMousePosition();

		if (!_lookEnabled && Input.IsButtonHeld(LookButton))
		{
			Graphics.Glfw.SetInputMode(Graphics.Window, CursorStateAttribute.Cursor, CursorModeValue.CursorHidden);
			_originalCursor = cursor;
			_lookEnabled = true;
		}
		else if (_lookEnabled && !Input.IsButtonHeld(LookButton))
		{
			StopLook();
		}

		Vector2 delta = cursor - _originalCursor;

		if (!_lookEnabled)
			return;

		_yaw -= lookSpeed * delta.X * 0.0001f;
		_pitch -= lookSpeed * delta.Y * 0.0001f;

		_pitch = Math.Clamp(_pitch, MathUtils.ToRadians(-89.9f), MathUtils.ToRadians(89.9f));
		Rotation = Quaternion.CreateFromYawPitchRoll(_yaw, -_pitch, 0);

		Graphics.Glfw.SetCursorPos(Graphics.Window, _originalCursor.X, _originalCursor.Y);
	}

	private static unsafe void StopLook()
	{
		Graphics.Glfw.SetInputMode(Graphics.Window, CursorStateAttribute.Cursor, CursorModeValue.CursorNormal);
		_lookEnabled = false;
	}

	public static Vector3 GetMouseWorldPosition(Vector2 normalizedMousePosition, Plane plane)
	{
		Vector3 nearSource = new(normalizedMousePosition.X, normalizedMousePosition.Y, 0f);
		Vector3 farSource = new(normalizedMousePosition.X, normalizedMousePosition.Y, 1f);
		Vector3 nearPoint = UnProject(nearSource, Projection, ViewMatrix, Matrix4x4.Identity);
		Vector3 farPoint = UnProject(farSource, Projection, ViewMatrix, Matrix4x4.Identity);

		// Create a ray from the near clip plane to the far clip plane.
		Vector3 direction = Vector3.Normalize(farPoint - nearPoint);
		Ray ray = new(nearPoint, direction);

		// Calculate distance of intersection point from ray.Position.
		float denominator = Vector3.Dot(plane.Normal, ray.Direction);
		float numerator = Vector3.Dot(plane.Normal, ray.Position) + plane.D;
		float t = -(numerator / denominator);

		// Calculate the picked position on the y = 0 plane.
		return nearPoint + direction * t;
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
		return new(x, framebufferSize.Y - y);
	}
}
