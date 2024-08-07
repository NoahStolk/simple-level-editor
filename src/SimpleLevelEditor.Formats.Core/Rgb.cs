using System.Numerics;

namespace SimpleLevelEditor.Formats.Core;

public record struct Rgb
{
	public byte R;
	public byte G;
	public byte B;

	public Rgb(byte r, byte g, byte b)
	{
		R = r;
		G = g;
		B = b;
	}

	public Vector3 ToVector3()
	{
		return new Vector3(R / 255f, G / 255f, B / 255f);
	}

	public Vector4 ToVector4()
	{
		return new Vector4(R / 255f, G / 255f, B / 255f, 1);
	}

	public string ToDisplayString()
	{
		return $"{R.ToDisplayString()} {G.ToDisplayString()} {B.ToDisplayString()}";
	}
}
