using Detach;
using GameEntityConfig.Core;
using GameEntityConfig.Editor.States;
using GameEntityConfig.Editor.Utils;
using ImGuiNET;
using System.Numerics;
using System.Reflection;

namespace GameEntityConfig.Editor.Ui.GameEntityConfigBuilder;

public sealed class CreateNewEntityDescriptorPopup
{
	private readonly EntityDescriptorBuilder _entityDescriptorBuilder = new();

	private string _name = string.Empty;
	private readonly List<FixedComponent> _fixedComponents = [];
	private readonly List<VaryingComponent> _varyingComponents = [];

	private void Reset()
	{
		_name = string.Empty;
	}

	public EntityDescriptor? Render(GameEntityConfigBuilderState state)
	{
		if (ImGui.BeginChild("NewEntityDescriptorChildWindow", new Vector2(0, ImGui.GetWindowHeight() - 96)))
		{
			ImGui.InputText("Name", ref _name, 100);

			ImGui.SeparatorText("Fixed Components");

			if (ImGui.BeginTable("FixedComponentsTable", 3, ImGuiTableFlags.Borders))
			{
				ImGui.TableSetupColumn("Remove", ImGuiTableColumnFlags.WidthFixed, 64);
				ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 128);
				ImGui.TableSetupColumn("Value", ImGuiTableColumnFlags.WidthStretch);

				ImGui.TableSetupScrollFreeze(0, 1);
				ImGui.TableHeadersRow();

				for (int i = 0; i < _fixedComponents.Count; i++)
				{
					FixedComponent fixedComponent = _fixedComponents[i];

					ImGui.TableNextRow();

					ImGui.TableNextColumn();
					if (ImGui.Button(Inline.Span($"X##{i}")))
						_fixedComponents.RemoveAt(i);
					if (ImGui.IsItemHovered())
						ImGui.SetTooltip("Remove this component");

					ImGui.TableNextColumn();
					if (ImGui.BeginCombo(Inline.Span($"##Type{i}"), fixedComponent.Type?.Name ?? "<NONE>", ImGuiComboFlags.HeightLarge))
					{
						List<TypeInfo> allComponentTypes = state.ComponentTypes;
						if (state.EnableDefaultComponents)
							allComponentTypes = allComponentTypes.Concat(ComponentUtils.DefaultComponents).ToList();

						foreach (TypeInfo type in allComponentTypes)
						{
							if (ImGui.Selectable(type.Name))
								_fixedComponents[i].Type = type;
						}

						ImGui.EndCombo();
					}

					ImGui.TableNextColumn();
					string temp = fixedComponent.Value;
					if (ImGui.InputText(Inline.Span($"##Value{i}"), ref temp, 100))
						_fixedComponents[i].Value = temp;
				}

				ImGui.EndTable();
			}

			if (ImGui.Button("+##Fixed", new Vector2(32, 32)))
				_fixedComponents.Add(new FixedComponent());

			ImGui.SeparatorText("Varying Components");

			for (int j = _varyingComponents.Count - 1; j >= 0; j--)
			{
				VaryingComponent varyingComponent = _varyingComponents[j];
				string varyingComponentTypeName = varyingComponent.Type?.Name ?? "<NONE>";
				ImGui.Button($"X##{varyingComponentTypeName}");
				ImGui.SameLine();
				ImGui.Text(varyingComponentTypeName);
				ImGui.SameLine();
				ImGui.InputText("Default Value", ref varyingComponent.DefaultValue, 100);
				ImGui.SameLine();
				ImGui.InputText("Step Value", ref varyingComponent.StepValue, 100);
				ImGui.SameLine();
				ImGui.InputText("Min Value", ref varyingComponent.MinValue, 100);
				ImGui.SameLine();
				ImGui.InputText("Max Value", ref varyingComponent.MaxValue, 100);
			}

			if (ImGui.Button("+##Varying", new Vector2(32, 32)))
				_varyingComponents.Add(new VaryingComponent());
		}

		ImGui.EndChild();

		if (ImGui.BeginChild("NewEntityDescriptorChildWindowBottom"))
		{
			ImGui.Separator();

			bool isValidEntityDescriptor = true; // TODO
			ImGui.BeginDisabled(!isValidEntityDescriptor);
			if (ImGui.Button("Create Entity Descriptor"))
			{
				ImGui.CloseCurrentPopup();
				EntityDescriptor entityDescriptor = ConstructEntityDescriptor();
				Reset();
				return entityDescriptor;
			}

			ImGui.EndDisabled();

			ImGui.SameLine();

			if (ImGui.Button("Cancel"))
				ImGui.CloseCurrentPopup();
		}

		ImGui.EndChild();

		return null;
	}

	private EntityDescriptor ConstructEntityDescriptor()
	{
		EntityDescriptorBuilder builder = _entityDescriptorBuilder.WithName(_name);

		foreach (FixedComponent fixedComponent in _fixedComponents)
		{
			if (fixedComponent.Type == null)
				continue;

			object? value = Activator.CreateInstance(fixedComponent.Type);

			// TODO: Use correct type.
			builder = builder.WithFixedComponent(value);
		}

		foreach (VaryingComponent varyingComponent in _varyingComponents)
		{
		}

		return builder.Build();
	}

	private sealed record FixedComponent
	{
		public Type? Type { get; set; }
		public string Value = string.Empty;
	}

	private sealed record VaryingComponent
	{
		public Type? Type { get; set; }
		public string DefaultValue = string.Empty;
		public string StepValue = string.Empty;
		public string MinValue = string.Empty;
		public string MaxValue = string.Empty;
	}
}
