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

			RenderFixedComponents(state);
			RenderVaryingComponents(state);
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

	private void RenderFixedComponents(GameEntityConfigBuilderState state)
	{
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
				if (ImGui.Button(Inline.Span($"X##Fixed{i}")))
					_fixedComponents.RemoveAt(i);
				if (ImGui.IsItemHovered())
					ImGui.SetTooltip("Remove this component");

				ImGui.TableNextColumn();
				if (ImGui.BeginCombo(Inline.Span($"##FixedType{i}"), fixedComponent.Type?.Name ?? "<NONE>", ImGuiComboFlags.HeightLarge))
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
	}

	private void RenderVaryingComponents(GameEntityConfigBuilderState state)
	{
		ImGui.SeparatorText("Varying Components");

		if (ImGui.BeginTable("VaryingComponentsTable", 6, ImGuiTableFlags.Borders))
		{
			ImGui.TableSetupColumn("Remove", ImGuiTableColumnFlags.WidthFixed, 64);
			ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 128);
			ImGui.TableSetupColumn("Default", ImGuiTableColumnFlags.WidthFixed, 128);
			ImGui.TableSetupColumn("Step", ImGuiTableColumnFlags.WidthFixed, 128);
			ImGui.TableSetupColumn("Min", ImGuiTableColumnFlags.WidthFixed, 128);
			ImGui.TableSetupColumn("Max", ImGuiTableColumnFlags.WidthFixed, 128);

			ImGui.TableSetupScrollFreeze(0, 1);
			ImGui.TableHeadersRow();

			for (int i = 0; i < _varyingComponents.Count; i++)
			{
				VaryingComponent varyingComponent = _varyingComponents[i];

				ImGui.TableNextRow();

				ImGui.TableNextColumn();
				if (ImGui.Button(Inline.Span($"X##Varying{i}")))
					_varyingComponents.RemoveAt(i);
				if (ImGui.IsItemHovered())
					ImGui.SetTooltip("Remove this component");

				ImGui.TableNextColumn();
				if (ImGui.BeginCombo(Inline.Span($"##VaryingType{i}"), varyingComponent.Type?.Name ?? "<NONE>", ImGuiComboFlags.HeightLarge))
				{
					List<TypeInfo> allComponentTypes = state.ComponentTypes;
					if (state.EnableDefaultComponents)
						allComponentTypes = allComponentTypes.Concat(ComponentUtils.DefaultComponents).ToList();

					foreach (TypeInfo type in allComponentTypes)
					{
						if (ImGui.Selectable(type.Name))
							_varyingComponents[i].Type = type;
					}

					ImGui.EndCombo();
				}

				ImGui.TableNextColumn();
				string temp = varyingComponent.DefaultValue;
				if (ImGui.InputText(Inline.Span($"##DefaultValue{i}"), ref temp, 100))
					_varyingComponents[i].DefaultValue = temp;

				ImGui.TableNextColumn();
				temp = varyingComponent.StepValue;
				if (ImGui.InputText(Inline.Span($"##StepValue{i}"), ref temp, 100))
					_varyingComponents[i].StepValue = temp;

				ImGui.TableNextColumn();
				temp = varyingComponent.MinValue;
				if (ImGui.InputText(Inline.Span($"##MinValue{i}"), ref temp, 100))
					_varyingComponents[i].MinValue = temp;

				ImGui.TableNextColumn();
				temp = varyingComponent.MaxValue;
				if (ImGui.InputText(Inline.Span($"##MaxValue{i}"), ref temp, 100))
					_varyingComponents[i].MaxValue = temp;
			}

			ImGui.EndTable();
		}

		if (ImGui.Button("+##Varying", new Vector2(32, 32)))
			_varyingComponents.Add(new VaryingComponent());
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
