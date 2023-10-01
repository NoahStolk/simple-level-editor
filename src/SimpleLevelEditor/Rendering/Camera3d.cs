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

	private static Vector3 _axisAlignedSpeed;
	private static float _yaw;
	private static float _pitch;

	private static Quaternion _rotation = Quaternion.Identity;
	public static Vector3 Position { get; private set; } = new(-3, 3, -3);

	public static Matrix4x4 Projection { get; private set; }
	public static Matrix4x4 ViewMatrix { get; private set; }
	public static float AspectRatio { get; set; }

	public static void Update(float dt, bool isFocused)
	{
		HandleKeys(dt);

		if (isFocused)
			HandleMouse();
		else
			StopLook();

		const float moveSpeed = 10;

		Matrix4x4 rotationMatrix = Matrix4x4.CreateFromQuaternion(_rotation);
		Vector3 transformed = RotateVector(_axisAlignedSpeed, rotationMatrix) + new Vector3(0, _axisAlignedSpeed.Y, 0);
		Position += transformed * moveSpeed * dt;

		Vector3 upDirection = Vector3.Transform(Vector3.UnitY, _rotation);
		Vector3 lookDirection = Vector3.Transform(Vector3.UnitZ, _rotation);
		ViewMatrix = Matrix4x4.CreateLookAt(Position, Position + lookDirection, upDirection);

		const float nearPlaneDistance = 0.05f;
		const float farPlaneDistance = 10000f;
		Projection = Matrix4x4.CreatePerspectiveFieldOfView(MathF.PI / 4 * _fieldOfView, AspectRatio, nearPlaneDistance, farPlaneDistance);

		static Vector3 RotateVector(Vector3 vector, Matrix4x4 rotationMatrix)
		{
			Vector3 right = new(rotationMatrix.M11, rotationMatrix.M12, rotationMatrix.M13);
			Vector3 forward = -Vector3.Cross(Vector3.UnitY, right);
			return right * vector.X + forward * vector.Z;
		}
	}

	private static void HandleKeys(float dt)
	{
		// Prevent moving camera when CTRL is pressed for shortcuts.
		bool ctrl = Input.IsKeyHeld(Keys.ControlLeft) || Input.IsKeyHeld(Keys.ControlRight);

		const float acceleration = 20;
		const float friction = 20;
		const Keys forwardInput = Keys.W;
		const Keys leftInput = Keys.A;
		const Keys backwardInput = Keys.S;
		const Keys rightInput = Keys.D;
		const Keys upInput = Keys.Space;
		const Keys downInput = Keys.ShiftLeft;
		bool forwardHold = !ctrl && Input.IsKeyHeld(forwardInput);
		bool leftHold = !ctrl && Input.IsKeyHeld(leftInput);
		bool backwardHold = !ctrl && Input.IsKeyHeld(backwardInput);
		bool rightHold = !ctrl && Input.IsKeyHeld(rightInput);
		bool upHold = !ctrl && Input.IsKeyHeld(upInput);
		bool downHold = !ctrl && Input.IsKeyHeld(downInput);

		float accelerationDt = acceleration * dt;
		float frictionDt = friction * dt;

		if (leftHold)
			_axisAlignedSpeed.X += accelerationDt;
		if (rightHold)
			_axisAlignedSpeed.X -= accelerationDt;

		if (upHold)
			_axisAlignedSpeed.Y += accelerationDt;
		if (downHold)
			_axisAlignedSpeed.Y -= accelerationDt;

		if (forwardHold)
			_axisAlignedSpeed.Z += accelerationDt;
		if (backwardHold)
			_axisAlignedSpeed.Z -= accelerationDt;

		if (!leftHold && !rightHold)
			_axisAlignedSpeed.X -= _axisAlignedSpeed.X * frictionDt;

		if (!upHold && !downHold)
			_axisAlignedSpeed.Y -= _axisAlignedSpeed.Y * frictionDt;

		if (!forwardHold && !backwardHold)
			_axisAlignedSpeed.Z -= _axisAlignedSpeed.Z * frictionDt;

		_axisAlignedSpeed.X = Math.Clamp(_axisAlignedSpeed.X, -1, 1);
		_axisAlignedSpeed.Y = Math.Clamp(_axisAlignedSpeed.Y, -1, 1);
		_axisAlignedSpeed.Z = Math.Clamp(_axisAlignedSpeed.Z, -1, 1);
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
		_rotation = Quaternion.CreateFromYawPitchRoll(_yaw, -_pitch, 0);

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
}
