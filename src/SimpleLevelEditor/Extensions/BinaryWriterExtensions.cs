namespace SimpleLevelEditor.Extensions;

public static class BinaryWriterExtensions
{
	public static void WriteAsHalfPrecision(this BinaryWriter binaryWriter, Vector2 vector)
	{
		binaryWriter.Write((Half)vector.X);
		binaryWriter.Write((Half)vector.Y);
	}

	public static void WriteAsHalfPrecision(this BinaryWriter binaryWriter, Vector3 vector)
	{
		binaryWriter.Write((Half)vector.X);
		binaryWriter.Write((Half)vector.Y);
		binaryWriter.Write((Half)vector.Z);
	}

	public static void Write(this BinaryWriter binaryWriter, Vector2 vector)
	{
		binaryWriter.Write(vector.X);
		binaryWriter.Write(vector.Y);
	}

	public static void Write(this BinaryWriter binaryWriter, Vector3 vector)
	{
		binaryWriter.Write(vector.X);
		binaryWriter.Write(vector.Y);
		binaryWriter.Write(vector.Z);
	}

	public static void Write(this BinaryWriter binaryWriter, Vector4 vector)
	{
		binaryWriter.Write(vector.X);
		binaryWriter.Write(vector.Y);
		binaryWriter.Write(vector.Z);
		binaryWriter.Write(vector.W);
	}

	public static void Write(this BinaryWriter binaryWriter, Plane plane)
	{
		binaryWriter.Write(plane.Normal);
		binaryWriter.Write(plane.D);
	}

	public static void Write(this BinaryWriter binaryWriter, Quaternion quaternion)
	{
		binaryWriter.Write(quaternion.X);
		binaryWriter.Write(quaternion.Y);
		binaryWriter.Write(quaternion.Z);
		binaryWriter.Write(quaternion.W);
	}

	public static void Write(this BinaryWriter binaryWriter, Matrix4x4 matrix)
	{
		binaryWriter.Write(matrix.M11);
		binaryWriter.Write(matrix.M12);
		binaryWriter.Write(matrix.M13);
		binaryWriter.Write(matrix.M14);
		binaryWriter.Write(matrix.M21);
		binaryWriter.Write(matrix.M22);
		binaryWriter.Write(matrix.M23);
		binaryWriter.Write(matrix.M24);
		binaryWriter.Write(matrix.M31);
		binaryWriter.Write(matrix.M32);
		binaryWriter.Write(matrix.M33);
		binaryWriter.Write(matrix.M34);
		binaryWriter.Write(matrix.M41);
		binaryWriter.Write(matrix.M42);
		binaryWriter.Write(matrix.M43);
		binaryWriter.Write(matrix.M44);
	}

	public static void WriteLengthPrefixedList<T>(this BinaryWriter bw, List<T> list, Action<BinaryWriter, T> writer)
	{
		bw.Write(list.Count);

		for (int i = 0; i < list.Count; i++)
			writer(bw, list[i]);
	}
}
