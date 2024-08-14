using Detach;
using ImGuiNET;
using SimpleLevelEditorV2.Formats.GameEntityConfig.Model;
using SimpleLevelEditorV2.States.GameEntityConfigBuilder;
using System.Globalization;

namespace SimpleLevelEditorV2.Ui.GameEntityConfigBuilder;

public sealed class GameEntityConfigEntityDescriptorsWindow
{
	private readonly CreateNewEntityDescriptorPopup _createNewEntityDescriptorPopup = new();

	public void Render(GameEntityConfigBuilderState state, Vector2 initialWindowSize)
	{
		ImGuiIOPtr io = ImGui.GetIO();
		Vector2 screenCenter = new(io.DisplaySize.X / 2, io.DisplaySize.Y / 2);
		ImGui.SetNextWindowPos(screenCenter, ImGuiCond.Appearing);
		ImGui.SetNextWindowSize(initialWindowSize, ImGuiCond.Appearing);
		if (ImGui.Begin("Entity Descriptors"))
		{
			for (int i = state.EntityDescriptors.Count - 1; i >= 0; i--)
			{
				EntityDescriptor entityDescriptor = state.EntityDescriptors[i];
				if (ImGui.Button($"X##{entityDescriptor.Name}"))
					state.EntityDescriptors.Remove(entityDescriptor);
				ImGui.SameLine();
				ImGui.Text(entityDescriptor.Name);

				ImGui.Indent();

				ImGui.SeparatorText("Fixed Components");

				if (ImGui.BeginTable("FixedComponentsTable", 2, ImGuiTableFlags.Borders))
				{
					ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 128);
					ImGui.TableSetupColumn("Value", ImGuiTableColumnFlags.WidthStretch);

					ImGui.TableSetupScrollFreeze(0, 1);
					ImGui.TableHeadersRow();

					for (int j = entityDescriptor.FixedComponents.Count - 1; j >= 0; j--)
					{
						FixedComponent fixedComponent = entityDescriptor.FixedComponents[i];

						ImGui.TableNextRow();

						ImGui.TableNextColumn();
						ImGui.Text(fixedComponent.DataType.Name);

						ImGui.TableNextColumn();
						ImGui.Text(fixedComponent.Value);
					}

					ImGui.EndTable();
				}

				ImGui.SeparatorText("Varying Components");

				if (ImGui.BeginTable("VaryingComponentsTable", 5, ImGuiTableFlags.Borders))
				{
					ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 128);
					ImGui.TableSetupColumn("Default", ImGuiTableColumnFlags.WidthFixed, 128);
					ImGui.TableSetupColumn("Step", ImGuiTableColumnFlags.WidthFixed, 128);
					ImGui.TableSetupColumn("Min", ImGuiTableColumnFlags.WidthFixed, 128);
					ImGui.TableSetupColumn("Max", ImGuiTableColumnFlags.WidthFixed, 128);

					ImGui.TableSetupScrollFreeze(0, 1);
					ImGui.TableHeadersRow();

					for (int j = entityDescriptor.VaryingComponents.Count - 1; j >= 0; j--)
					{
						VaryingComponent varyingComponent = entityDescriptor.VaryingComponents[j];

						ImGui.TableNextRow();

						ImGui.TableNextColumn();
						ImGui.Text(varyingComponent.DataType.Name);

						ImGui.TableNextColumn();
						ImGui.Text(varyingComponent.DefaultValue);

						ImGui.TableNextColumn();
						ImGui.Text(Inline.Span(varyingComponent.SliderConfiguration?.Step.ToString(CultureInfo.InvariantCulture) ?? "N/A"));

						ImGui.TableNextColumn();
						ImGui.Text(Inline.Span(varyingComponent.SliderConfiguration?.Min.ToString(CultureInfo.InvariantCulture) ?? "N/A"));

						ImGui.TableNextColumn();
						ImGui.Text(Inline.Span(varyingComponent.SliderConfiguration?.Max.ToString(CultureInfo.InvariantCulture) ?? "N/A"));

						ImGui.TableNextColumn();
					}

					ImGui.EndTable();
				}

				ImGui.Unindent();
			}

			if (ImGui.Button("Create New Entity Descriptor"))
				ImGui.OpenPopup("Create New Entity Descriptor");

			ImGui.SetNextWindowSizeConstraints(new Vector2(640, 240), new Vector2(float.MaxValue, float.MaxValue));
			if (ImGui.BeginPopupModal("Create New Entity Descriptor"))
			{
				EntityDescriptor? newEntityDescriptor = _createNewEntityDescriptorPopup.Render(state);

				if (newEntityDescriptor != null && state.EntityDescriptors.TrueForAll(ct => ct.Name != newEntityDescriptor.Name))
					state.EntityDescriptors.Add(newEntityDescriptor);

				ImGui.EndPopup();
			}
		}

		ImGui.End();
	}
}
