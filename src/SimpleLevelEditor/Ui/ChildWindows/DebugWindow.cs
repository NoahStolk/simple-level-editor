using Detach;
using ImGuiNET;
using SimpleLevelEditor.State;

namespace SimpleLevelEditor.Ui.ChildWindows;

public static class DebugWindow
{
	private static long _previousAllocatedBytes;

	public static void Render(Vector2 size)
	{
		if (ImGui.BeginChild("Debug", size, ImGuiChildFlags.Border))
		{
			ImGui.SeparatorText("Warnings");

			if (ImGui.BeginChild("Warnings", new(0, 96)))
			{
				if (DebugState.Warnings.Count > 0)
				{
					foreach (KeyValuePair<string, int> kvp in DebugState.Warnings)
						ImGui.TextWrapped(Inline.Span($"{kvp.Key}: {kvp.Value}"));
				}
			}

			ImGui.EndChild(); // End Warnings

			ImGui.BeginDisabled(DebugState.Warnings.Count == 0);
			if (ImGui.Button("Clear"))
				DebugState.ClearWarnings();

			ImGui.EndDisabled();

			ImGui.SeparatorText("Performance");

			ImGui.Text(Inline.Span($"{App.Instance.Fps} FPS"));
			ImGui.Text(Inline.Span($"Frame time: {App.Instance.FrameTime:0.0000} s"));

			ImGui.SeparatorText("Allocations");

			long allocatedBytes = GC.GetAllocatedBytesForCurrentThread();
			ImGui.Text(Inline.Span($"Allocated: {allocatedBytes:N0} bytes"));
			ImGui.Text(Inline.Span($"Since last update: {allocatedBytes - _previousAllocatedBytes:N0} bytes"));
			_previousAllocatedBytes = allocatedBytes;

			for (int i = 0; i < GC.MaxGeneration + 1; i++)
				ImGui.Text(Inline.Span($"Gen{i}: {GC.CollectionCount(i)} times"));

			ImGui.Text(Inline.Span($"Total memory: {GC.GetTotalMemory(false):N0} bytes"));
			ImGui.Text(Inline.Span($"Total pause duration: {GC.GetTotalPauseDuration().TotalSeconds:0.000} s"));

			ImGui.SeparatorText("Watching directories");

			for (int i = 0; i < AssetFileWatcher.Directories.Count; i++)
				ImGui.TextWrapped(AssetFileWatcher.Directories[i]);
		}

		ImGui.EndChild(); // End Debug
	}
}
