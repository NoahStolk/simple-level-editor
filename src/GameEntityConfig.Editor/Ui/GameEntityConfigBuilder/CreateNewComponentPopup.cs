using Detach;
using GameEntityConfig.Editor.Utils;
using GameEntityConfig.Emit;
using ImGuiNET;
using System.Numerics;
using System.Reflection;

namespace GameEntityConfig.Editor.Ui.GameEntityConfigBuilder;

public sealed class CreateNewComponentPopup
{
	private string _newComponentTypeName = string.Empty;
	private readonly List<ComponentField> _newComponentTypeFields = [];

	private void Reset()
	{
		_newComponentTypeName = string.Empty;
		_newComponentTypeFields.Clear();
	}

	public TypeInfo? Render()
	{
		if (ImGui.BeginChild("NewComponentChildWindow", new Vector2(0, ImGui.GetWindowHeight() - 96)))
		{
			ImGui.InputText("Component Type Name", ref _newComponentTypeName, 100);

			if (ImGui.BeginTable("FieldsTable", 3, ImGuiTableFlags.Borders))
			{
				ImGui.TableSetupColumn("Remove", ImGuiTableColumnFlags.WidthFixed, 64);
				ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 128);
				ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);

				ImGui.TableSetupScrollFreeze(0, 1);
				ImGui.TableHeadersRow();

				for (int i = 0; i < _newComponentTypeFields.Count; i++)
				{
					ComponentField componentField = _newComponentTypeFields[i];

					ImGui.TableNextRow();

					ImGui.TableNextColumn();
					if (ImGui.Button(Inline.Span($"X##{i}")))
						_newComponentTypeFields.RemoveAt(i);
					if (ImGui.IsItemHovered())
						ImGui.SetTooltip("Remove this field");

					ImGui.TableNextColumn();
					if (ImGui.BeginCombo(Inline.Span($"##Type{i}"), componentField.Type?.Name ?? "<NONE>", ImGuiComboFlags.HeightLarge))
					{
						foreach (KeyValuePair<Type, string> type in DataTypeUtils.Primitives)
						{
							if (ImGui.Selectable(type.Value))
								_newComponentTypeFields[i].Type = type.Key;
						}

						ImGui.EndCombo();
					}

					ImGui.TableNextColumn();
					string temp = componentField.Name;
					if (ImGui.InputText(Inline.Span($"##Name{i}"), ref temp, 100))
						_newComponentTypeFields[i].Name = temp;
				}

				ImGui.EndTable();
			}

			if (ImGui.Button("+", new Vector2(32, 32)))
				_newComponentTypeFields.Add(new ComponentField());

			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("Add new field");
		}

		ImGui.EndChild();

		if (ImGui.BeginChild("NewComponentChildWindowBottom"))
		{
			ImGui.Separator();

			bool isValidComponent =
				ComponentTypeBuilder.IsValidTypeName(_newComponentTypeName) &&
				_newComponentTypeFields.Select(f => f.Name).Distinct().Count() == _newComponentTypeFields.Count &&
				_newComponentTypeFields.TrueForAll(f => ComponentTypeBuilder.IsValidFieldName(f.Name));
			ImGui.BeginDisabled(!isValidComponent);
			if (ImGui.Button("Create Component"))
			{
				if (isValidComponent)
				{
					ImGui.CloseCurrentPopup();
					TypeInfo newType = ConstructComponent();
					Reset();
					return newType;
				}
			}

			ImGui.EndDisabled();

			ImGui.SameLine();

			if (ImGui.Button("Cancel"))
				ImGui.CloseCurrentPopup();
		}

		ImGui.EndChild();

		return null;
	}

	private TypeInfo ConstructComponent()
	{
		// ! LINQ filtering.
		List<FieldDescriptor> fieldDescriptors = _newComponentTypeFields.Where(f => f.Type != null).Select(cf => new FieldDescriptor(cf.Name, cf.Type!)).ToList();
		return ComponentTypeBuilder.CompileResultTypeInfo(_newComponentTypeName, fieldDescriptors);
	}

	private sealed record ComponentField
	{
		public string Name { get; set; } = string.Empty;
		public Type? Type { get; set; }
	}
}
