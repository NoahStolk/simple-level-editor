using ImGuiNET;
using SimpleLevelEditorV2.Logging;
using SimpleLevelEditorV2.Utils;

namespace SimpleLevelEditorV2.Ui.Logging;

public sealed class LoggingWindow
{
	public unsafe void Render()
	{
		ImGui.SetNextWindowSizeConstraints(new Vector2(256, 256), new Vector2(float.MaxValue));
		if (ImGui.Begin("Messages"))
		{
			ImGui.BeginDisabled(GlobalLogger.Messages.Count == 0);
			if (ImGui.Button("Clear all"))
				GlobalLogger.ClearMessages();

			ImGui.SameLine();
			if (ImGui.Button("Clear all older than 1 minute"))
				GlobalLogger.ClearMessagesOlderThan(TimeSpan.FromMinutes(1));

			ImGui.EndDisabled();

			if (ImGui.BeginChild("MessagesList"))
			{
				if (GlobalLogger.Messages.Count > 0)
				{
					if (ImGui.BeginTable("MessagesTable", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedSame))
					{
						ImGui.TableSetupColumn("Severity", ImGuiTableColumnFlags.WidthFixed);
						ImGui.TableSetupColumn("Message", ImGuiTableColumnFlags.WidthStretch);
						ImGui.TableSetupColumn("Last appeared", ImGuiTableColumnFlags.WidthFixed);
						ImGui.TableSetupColumn("Count", ImGuiTableColumnFlags.WidthFixed);

						ImGui.TableHeadersRow();

						ImGuiListClipperPtr clipper = new(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
						clipper.Begin(GlobalLogger.Messages.Count);
						while (clipper.Step())
						{
							for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
							{
								Message message = GlobalLogger.Messages[i];
								ImGui.TableNextRow();

								ImGui.TableNextColumn();
								ImGui.TextColored(GetMessageColor(message.Severity), message.Severity.ToString());

								ImGui.TableNextColumn();
								ImGui.TextWrapped(message.Text);

								ImGui.TableNextColumn();
								ImGui.TextWrapped(DateTimeUtils.FormatTimeAgo(message.LastAppeared));

								ImGui.TableNextColumn();
								ImGui.Text($"{message.Count}");
							}
						}

						ImGui.EndTable();
					}
				}
				else
				{
					ImGui.TextColored(Detach.Numerics.Rgba.Green, "No warnings");
				}
			}

			ImGui.EndChild();
		}

		ImGui.End();
	}

	private static Detach.Numerics.Rgba GetMessageColor(MessageSeverity messageSeverity)
	{
		return messageSeverity switch
		{
			MessageSeverity.Info => new Detach.Numerics.Rgba(127, 255, 127),
			MessageSeverity.Warning => Detach.Numerics.Rgba.Yellow,
			MessageSeverity.Error => Detach.Numerics.Rgba.Red,
			MessageSeverity.Fatal => Detach.Numerics.Rgba.Purple,
			_ => Detach.Numerics.Rgba.White,
		};
	}
}
