using Detach.Numerics;
using ImGuiNET;

namespace SimpleLevelEditor.Utils;

public static class ImGuiUtils
{
	public static bool ColorEdit3Rgb(ReadOnlySpan<char> label, ref Formats.Types.Rgb rgb)
	{
		Vector3 vector = new(rgb.R / (float)byte.MaxValue, rgb.G / (float)byte.MaxValue, rgb.B / (float)byte.MaxValue);
		bool edited = ImGui.ColorEdit3(label, ref vector);
		if (edited)
			rgb = new Formats.Types.Rgb((byte)(vector.X * byte.MaxValue), (byte)(vector.Y * byte.MaxValue), (byte)(vector.Z * byte.MaxValue));

		return edited;
	}

	public static bool ColorEdit4Rgba(ReadOnlySpan<char> label, ref Formats.Types.Rgba rgba)
	{
		Vector4 vector = new(rgba.R / (float)byte.MaxValue, rgba.G / (float)byte.MaxValue, rgba.B / (float)byte.MaxValue, rgba.A / (float)byte.MaxValue);
		bool edited = ImGui.ColorEdit4(label, ref vector);
		if (edited)
			rgba = new Formats.Types.Rgba((byte)(vector.X * byte.MaxValue), (byte)(vector.Y * byte.MaxValue), (byte)(vector.Z * byte.MaxValue), (byte)(vector.W * byte.MaxValue));

		return edited;
	}

	public static void TextOptional(string? text)
	{
		ImGui.TextColored(text == null ? Color.Gray(0.5f) : Color.White, text ?? "N/A");
	}

	public static void TextOptional(string text, bool isOptional)
	{
		ImGui.TextColored(isOptional ? Color.Gray(0.5f) : Color.White, isOptional ? "N/A" : text);
	}
}
