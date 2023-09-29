using ImGuiNET;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class DebugWindow
{
	private static long _previousAllocatedBytes;

	public static List<string> Warnings { get; } = new();

	public static void Render(Vector2 size)
	{
		if (ImGui.BeginChild("Debug", size, true))
		{
			ImGui.SeparatorText("Debug");

			ImGui.Text(Inline.Span($"{App.Instance.Fps} FPS"));

			ImGui.SetNextItemOpen(true);
			if (ImGui.CollapsingHeader("Allocations"))
			{
				long allocatedBytes = GC.GetAllocatedBytesForCurrentThread();
				ImGui.Text(Inline.Span($"Allocated: {allocatedBytes:N0} bytes"));
				ImGui.Text(Inline.Span($"Since last update: {allocatedBytes - _previousAllocatedBytes:N0} bytes"));
				_previousAllocatedBytes = allocatedBytes;

				for (int i = 0; i < GC.MaxGeneration + 1; i++)
					ImGui.Text(Inline.Span($"Gen{i}: {GC.CollectionCount(i)} times"));

				ImGui.Text(Inline.Span($"Total memory: {GC.GetTotalMemory(false):N0} bytes"));
				ImGui.Text(Inline.Span($"Total pause duration: {GC.GetTotalPauseDuration().TotalSeconds:0.000} s"));
			}

			if (ImGui.CollapsingHeader("Debug stack"))
			{
				for (int i = 0; i < Warnings.Count; i++)
					ImGui.Text(Warnings[i]);
			}
		}

		ImGui.EndChild(); // End Debug
	}
}
