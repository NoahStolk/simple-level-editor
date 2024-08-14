using Detach;
using ImGuiNET;
using SimpleLevelEditorV2.Formats.GameEntityConfig;
using SimpleLevelEditorV2.Formats.GameEntityConfig.Model;

namespace Editor.Ui.GameEntityConfigBuilder;

public sealed class CreateNewDataTypePopup
{
	private string _newDataTypeName = string.Empty;
	private readonly List<Field> _newDataTypeFields = [];

	private void Reset()
	{
		_newDataTypeName = string.Empty;
		_newDataTypeFields.Clear();
	}

	public DataType? Render()
	{
		if (ImGui.BeginChild("NewDataTypeChildWindow", new Vector2(0, ImGui.GetWindowHeight() - 96)))
		{
			ImGui.InputText("Data Type Name", ref _newDataTypeName, 100);

			if (ImGui.BeginTable("FieldsTable", 3, ImGuiTableFlags.Borders))
			{
				ImGui.TableSetupColumn("Remove", ImGuiTableColumnFlags.WidthFixed, 64);
				ImGui.TableSetupColumn("Type", ImGuiTableColumnFlags.WidthFixed, 128);
				ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthStretch);

				ImGui.TableSetupScrollFreeze(0, 1);
				ImGui.TableHeadersRow();

				for (int i = 0; i < _newDataTypeFields.Count; i++)
				{
					Field field = _newDataTypeFields[i];

					ImGui.TableNextRow();

					ImGui.TableNextColumn();
					if (ImGui.Button(Inline.Span($"X##{i}")))
						_newDataTypeFields.RemoveAt(i);
					if (ImGui.IsItemHovered())
						ImGui.SetTooltip("Remove this field");

					ImGui.TableNextColumn();
					if (ImGui.BeginCombo(Inline.Span($"##Type{i}"), field.Primitive.ToString(), ImGuiComboFlags.HeightLarge))
					{
						foreach (Primitive primitive in Primitives.All)
						{
							if (ImGui.Selectable(primitive.ToString()))
								_newDataTypeFields[i].Primitive = primitive;
						}

						ImGui.EndCombo();
					}

					ImGui.TableNextColumn();
					string temp = field.Name;
					if (ImGui.InputText(Inline.Span($"##Name{i}"), ref temp, 100))
						_newDataTypeFields[i].Name = temp;
				}

				ImGui.EndTable();
			}

			if (ImGui.Button("+", new Vector2(32, 32)))
				_newDataTypeFields.Add(new Field());

			if (ImGui.IsItemHovered())
				ImGui.SetTooltip("Add new field");
		}

		ImGui.EndChild();

		if (ImGui.BeginChild("NewComponentChildWindowBottom"))
		{
			ImGui.Separator();

			bool isValidComponent =
				DataTypeBuilder.IsValidTypeName(_newDataTypeName) &&
				_newDataTypeFields.Select(f => f.Name).Distinct().Count() == _newDataTypeFields.Count &&
				_newDataTypeFields.TrueForAll(f => DataTypeBuilder.IsValidFieldName(f.Name));
			ImGui.BeginDisabled(!isValidComponent);
			if (ImGui.Button("Create Component"))
			{
				if (isValidComponent)
				{
					ImGui.CloseCurrentPopup();
					DataType newDataType = DataTypeBuilder.Build(_newDataTypeName, _newDataTypeFields.ConvertAll(f => new DataTypeField(f.Name, f.Primitive)));
					Reset();
					return newDataType;
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

	private sealed record Field
	{
		public string Name { get; set; } = string.Empty;
		public Primitive Primitive { get; set; } = Primitive.U8;
	}
}
