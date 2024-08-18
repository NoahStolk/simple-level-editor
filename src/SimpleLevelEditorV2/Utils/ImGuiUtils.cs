using ImGuiNET;

namespace SimpleLevelEditorV2.Utils;

public static class ImGuiUtils
{
	public static void TextOptional(string? text)
	{
		ImGui.TextColored(text == null ? Detach.Numerics.Rgba.Gray(0.5f) : Detach.Numerics.Rgba.White, text ?? "N/A");
	}

	public static void TextOptional(ReadOnlySpan<char> text, bool isOptional)
	{
		ImGui.TextColored(isOptional ? Detach.Numerics.Rgba.Gray(0.5f) : Detach.Numerics.Rgba.White, isOptional ? "N/A" : text);
	}
}
