using Detach;
using ImGuiNET;
using SimpleLevelEditorV2.Formats.EntityConfig;
using SimpleLevelEditorV2.Formats.EntityConfig.Model;
using SimpleLevelEditorV2.States.EntityConfigEditor;

namespace SimpleLevelEditorV2.Ui.EntityConfig;

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

	public EntityDescriptor? Render(EntityConfigEditorState state)
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
				EntityDescriptor entityDescriptor = ConstructEntityDescriptor(state);
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

	private void RenderFixedComponents(EntityConfigEditorState state)
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
				if (ImGui.BeginCombo(Inline.Span($"##FixedType{i}"), fixedComponent.DataTypeName, ImGuiComboFlags.HeightLarge))
				{
					foreach (DataType dataType in state.DataTypes)
					{
						if (ImGui.Selectable(dataType.Name))
							_fixedComponents[i].DataTypeName = dataType.Name;
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

	private void RenderVaryingComponents(EntityConfigEditorState state)
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
				if (ImGui.BeginCombo(Inline.Span($"##VaryingType{i}"), varyingComponent.DataTypeName, ImGuiComboFlags.HeightLarge))
				{
					foreach (DataType dataType in state.DataTypes)
					{
						if (ImGui.Selectable(dataType.Name))
							_varyingComponents[i].DataTypeName = dataType.Name;
					}

					ImGui.EndCombo();
				}

				ImGui.TableNextColumn();
				string temp = varyingComponent.DefaultValue;
				if (ImGui.InputText(Inline.Span($"##DefaultValue{i}"), ref temp, 100))
					_varyingComponents[i].DefaultValue = temp;

				ImGui.TableNextColumn();
				float floatTemp = varyingComponent.StepValue;
				if (ImGui.InputFloat(Inline.Span($"##StepValue{i}"), ref floatTemp))
					_varyingComponents[i].StepValue = floatTemp;

				ImGui.TableNextColumn();
				floatTemp = varyingComponent.MinValue;
				if (ImGui.InputFloat(Inline.Span($"##MinValue{i}"), ref floatTemp))
					_varyingComponents[i].MinValue = floatTemp;

				ImGui.TableNextColumn();
				floatTemp = varyingComponent.MaxValue;
				if (ImGui.InputFloat(Inline.Span($"##MaxValue{i}"), ref floatTemp))
					_varyingComponents[i].MaxValue = floatTemp;
			}

			ImGui.EndTable();
		}

		if (ImGui.Button("+##Varying", new Vector2(32, 32)))
			_varyingComponents.Add(new VaryingComponent());
	}

	private EntityDescriptor ConstructEntityDescriptor(EntityConfigEditorState state)
	{
		EntityDescriptorBuilder builder = _entityDescriptorBuilder.WithName(_name);

		foreach (FixedComponent fixedComponent in _fixedComponents)
		{
			DataType? dataType = GetRequiredDataType(state, fixedComponent.DataTypeName);
			if (dataType == null)
				continue;

			builder = builder.WithFixedComponent(dataType, fixedComponent.Value);
		}

		foreach (VaryingComponent varyingComponent in _varyingComponents)
		{
			DataType? dataType = GetRequiredDataType(state, varyingComponent.DataTypeName);
			if (dataType == null)
				continue;

			builder = builder.WithVaryingComponent(dataType, varyingComponent.DefaultValue, varyingComponent.StepValue, varyingComponent.MinValue, varyingComponent.MaxValue);
		}

		return builder.Build();

		static DataType? GetRequiredDataType(EntityConfigEditorState state, string dataTypeName)
		{
			return state.DataTypes.Find(dt => dt.Name == dataTypeName) ?? DataType.DefaultDataTypes.FirstOrDefault(dt => dt.Name == dataTypeName);
		}
	}

	private sealed record FixedComponent
	{
		public string DataTypeName = string.Empty;
		public string Value = string.Empty;
	}

	private sealed record VaryingComponent
	{
		public string DataTypeName = string.Empty;
		public string DefaultValue = string.Empty;
		public float StepValue = 0.1f;
		public float MinValue = -100f;
		public float MaxValue = 100f;
	}
}
