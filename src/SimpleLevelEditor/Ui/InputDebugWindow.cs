using Detach.Numerics;
using ImGuiGlfw;
using ImGuiNET;
using Silk.NET.GLFW;

namespace SimpleLevelEditor.Ui;

public static class InputDebugWindow
{
	private static string _debugTextInput = string.Empty;

	public static void Render(ref bool showWindow)
	{
		if (ImGui.Begin("Input debug", ref showWindow))
		{
			ImGuiIOPtr io = ImGui.GetIO();

			ImGui.SeparatorText("ImGui key modifiers");
			ImGui.TextColored(io.KeyCtrl ? Color.White : Color.Gray(0.4f), "CTRL");
			ImGui.SameLine();
			ImGui.TextColored(io.KeyShift ? Color.White : Color.Gray(0.4f), "SHIFT");
			ImGui.SameLine();
			ImGui.TextColored(io.KeyAlt ? Color.White : Color.Gray(0.4f), "ALT");
			ImGui.SameLine();
			ImGui.TextColored(io.KeySuper ? Color.White : Color.Gray(0.4f), "SUPER");

			ImGui.SeparatorText("GLFW keys");
			if (ImGui.BeginTable("GLFW keys", 8))
			{
				for (int i = 0; i < 1024; i++)
				{
					if (i == 0)
						ImGui.TableNextRow();

					Keys key = (Keys)i;
					if (!Enum.IsDefined(key))
						continue;

					bool isDown = GlfwInput.IsKeyDown(key);

					ImGui.TableNextColumn();
					ImGui.TextColored(isDown ? Color.White : Color.Gray(0.4f), key.ToString());
				}

				ImGui.EndTable();
			}

			ImGui.SeparatorText("Debug text input");
			ImGui.InputTextMultiline("##DebugTextInput", ref _debugTextInput, 1024, new(0, 128));
		}

		ImGui.End();
	}
}
