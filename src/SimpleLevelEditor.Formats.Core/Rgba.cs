using System.Numerics;

namespace SimpleLevelEditor.Formats.Core;

public record struct Rgba
{
	public byte R;
	public byte G;
	public byte B;
	public byte A;

	public Rgba(byte r, byte g, byte b, byte a)
	{
		R = r;
		G = g;
		B = b;
		A = a;
	}

	public Vector4 ToVector4()
	{
		return new Vector4(R / 255f, G / 255f, B / 255f, A / 255f);
	}

	public string ToDisplayString()
	{
		return $"{R.ToDisplayString()} {G.ToDisplayString()} {B.ToDisplayString()} {A.ToDisplayString()}";
	}
}
