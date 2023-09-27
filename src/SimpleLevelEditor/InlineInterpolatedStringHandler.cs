using System.Runtime.CompilerServices;

namespace SimpleLevelEditor;

#pragma warning disable CA1822, RCS1163
[InterpolatedStringHandler]
public ref struct InlineInterpolatedStringHandler
{
	private int _charsWritten;

	public InlineInterpolatedStringHandler(int literalLength, int formattedCount)
	{
	}

	public static implicit operator ReadOnlySpan<char>(InlineInterpolatedStringHandler handler)
	{
		return Inline.Buffer[..handler._charsWritten];
	}

	public void AppendLiteral(string s)
	{
		if (s.TryCopyTo(Inline.Buffer[_charsWritten..]))
			_charsWritten += s.Length;
	}

	public void AppendFormatted(ReadOnlySpan<char> s)
	{
		if (s.TryCopyTo(Inline.Buffer[_charsWritten..]))
			_charsWritten += s.Length;
	}

	public void AppendFormatted<T>(T t, ReadOnlySpan<char> format = default, IFormatProvider? provider = null)
		where T : ISpanFormattable
	{
		t.TryFormat(Inline.Buffer[_charsWritten..], out int charsWritten, format, provider);
		_charsWritten += charsWritten;
	}
}
