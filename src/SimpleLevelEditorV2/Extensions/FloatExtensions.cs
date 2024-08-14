namespace SimpleLevelEditorV2.Extensions;

public static class FloatExtensions
{
	public static bool IsZero(this float value)
	{
		return value is < float.Epsilon and > -float.Epsilon;
	}
}
