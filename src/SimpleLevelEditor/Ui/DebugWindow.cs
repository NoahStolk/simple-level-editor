using Detach;
using ImGuiNET;
using SimpleLevelEditor.State;
using SimpleLevelEditor.State.States.Assets;

namespace SimpleLevelEditor.Ui;

public static class DebugWindow
{
	private static long _previousAllocatedBytes;

	public static void Render(ref bool showWindow)
	{
		if (ImGui.Begin("Debug", ref showWindow))
		{
			ImGui.SeparatorText("Rendering");

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

			if (AssetFileWatcher.Directories.Count > 0)
			{
				for (int i = 0; i < AssetFileWatcher.Directories.Count; i++)
					ImGui.TextWrapped(AssetFileWatcher.Directories[i]);
			}
			else
			{
				ImGui.TextWrapped("No directories are currently being watched.");
			}
		}

		ImGui.End();
	}
}
