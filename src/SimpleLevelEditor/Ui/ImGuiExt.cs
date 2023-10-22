using ImGuiNET;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Ui;

public static class ImGuiExt
{
	public static bool KnobAngle(ReadOnlySpan<char> label, ref float value)
	{
		ImGuiStylePtr style = ImGui.GetStyle();

		const float radiusOuter = 20f;
		Vector2 pos = ImGui.GetCursorScreenPos();
		Vector2 center = new(pos.X + radiusOuter, pos.Y + radiusOuter);
		float lineHeight = ImGui.GetTextLineHeight();
		ImDrawListPtr drawList = ImGui.GetWindowDrawList();

		ImGui.InvisibleButton(label, new(radiusOuter * 2, radiusOuter * 2 + lineHeight + style.ItemInnerSpacing.Y));
		bool valueChanged = false;
		bool isActive = ImGui.IsItemActive();
		bool isHovered = ImGui.IsItemHovered();
		if (isActive)
		{
			// Calculate angle from mouse to center of knob.
			Vector2 mousePos = ImGui.GetMousePos();
			value = MathF.Atan2(mousePos.Y - center.Y, mousePos.X - center.X);

			const float snapDegrees = 5f;
			float rad = MathUtils.ToRadians(snapDegrees);
			value = MathF.Round(value / rad) * rad;

			valueChanged = true;
		}

		float angleCos = MathF.Cos(value);
		float angleSin = MathF.Sin(value);
		const float radiusInner = radiusOuter * 0.4f;
		drawList.AddCircleFilled(center, radiusOuter, ImGui.GetColorU32(ImGuiCol.FrameBg), 16);
		drawList.AddLine(new(center.X + angleCos * radiusInner, center.Y + angleSin * radiusInner), new(center.X + angleCos * (radiusOuter - 2), center.Y + angleSin * (radiusOuter - 2)), ImGui.GetColorU32(ImGuiCol.SliderGrabActive), 2f);
		drawList.AddCircleFilled(center, radiusInner, ImGui.GetColorU32(isActive ? ImGuiCol.FrameBgActive : isHovered ? ImGuiCol.FrameBgHovered : ImGuiCol.FrameBg), 16);
		drawList.AddText(new(pos.X, pos.Y + radiusOuter * 2 + style.ItemInnerSpacing.Y), ImGui.GetColorU32(ImGuiCol.Text), label);

		if (isActive || isHovered)
		{
			ImGui.SetNextWindowPos(new(pos.X - style.WindowPadding.X, pos.Y - lineHeight - style.ItemInnerSpacing.Y - style.WindowPadding.Y));
			ImGui.BeginTooltip();
			ImGui.Text(Inline.Span($"{MathUtils.ToDegrees(value):0} deg"));
			ImGui.EndTooltip();
		}

		return valueChanged;
	}
}
