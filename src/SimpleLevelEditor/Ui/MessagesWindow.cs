using ImGuiNET;
using SimpleLevelEditor.State.Messages;
using SimpleLevelEditor.Utils;

namespace SimpleLevelEditor.Ui;

public static class MessagesWindow
{
	public static unsafe void Render()
	{
		ImGui.SetNextWindowSizeConstraints(new Vector2(256, 256), new Vector2(float.MaxValue));
		if (ImGui.Begin("Messages"))
		{
			ImGui.BeginDisabled(MessagesState.Messages.Count == 0);
			if (ImGui.Button("Clear all"))
				MessagesState.ClearMessages();

			ImGui.SameLine();
			if (ImGui.Button("Clear all older than 1 minute"))
				MessagesState.ClearMessagesOlderThan(TimeSpan.FromMinutes(1));

			ImGui.EndDisabled();

			if (ImGui.BeginChild("MessagesList"))
			{
				if (MessagesState.Messages.Count > 0)
				{
					if (ImGui.BeginTable("MessagesTable", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedSame))
					{
						ImGui.TableSetupColumn("Severity", ImGuiTableColumnFlags.WidthFixed);
						ImGui.TableSetupColumn("Message", ImGuiTableColumnFlags.WidthStretch);
						ImGui.TableSetupColumn("Last appeared", ImGuiTableColumnFlags.WidthFixed);
						ImGui.TableSetupColumn("Count", ImGuiTableColumnFlags.WidthFixed);

						ImGui.TableHeadersRow();

						ImGuiListClipperPtr clipper = new(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
						clipper.Begin(MessagesState.Messages.Count);
						while (clipper.Step())
						{
							for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
							{
								Message message = MessagesState.Messages[i];
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
			_ => Detach.Numerics.Rgba.White,
		};
	}
}
