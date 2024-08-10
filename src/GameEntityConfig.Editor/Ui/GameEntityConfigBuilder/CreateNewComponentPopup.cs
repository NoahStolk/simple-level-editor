using Detach;
using GameEntityConfig.Emit;
using ImGuiNET;
using System.Reflection;

namespace GameEntityConfig.Editor.Ui.GameEntityConfigBuilder;

public sealed class CreateNewComponentPopup
{
	private static readonly Dictionary<Type, string> _primitives = new()
	{
		{ typeof(bool), "bool" },

		{ typeof(sbyte), "i8" },
		{ typeof(short), "i16" },
		{ typeof(int), "i32" },
		{ typeof(long), "i64" },
		{ typeof(Int128), "i128" },

		{ typeof(byte), "u8" },
		{ typeof(ushort), "u16" },
		{ typeof(uint), "u32" },
		{ typeof(ulong), "u64" },
		{ typeof(UInt128), "u128" },

		{ typeof(Half), "f16" },
		{ typeof(float), "f32" },
		{ typeof(double), "f64" },

		{ typeof(decimal), "d128" },

		{ typeof(char), "char" },

		{ typeof(string), "str" },
	};

	private string _newComponentTypeName = string.Empty;
	private readonly List<ComponentField> _newComponentTypeFields = [];

	private void Reset()
	{
		_newComponentTypeName = string.Empty;
		_newComponentTypeFields.Clear();
	}

	public TypeInfo? Render()
	{
		ImGui.SeparatorText("Construct New Component");

		ImGui.InputText("Component Type", ref _newComponentTypeName, 100);

		for (int i = 0; i < _newComponentTypeFields.Count; i++)
		{
			ComponentField componentField = _newComponentTypeFields[i];

			if (ImGui.Button(Inline.Span($"X##{i}")))
				_newComponentTypeFields.RemoveAt(i);
			ImGui.SameLine();

			ImGui.PushItemWidth(120);
			if (ImGui.BeginCombo(Inline.Span($"Field Type##{i}"), componentField.Type?.Name ?? "None", ImGuiComboFlags.HeightLarge))
			{
				foreach (KeyValuePair<Type, string> type in _primitives)
				{
					if (ImGui.Selectable(type.Value))
						_newComponentTypeFields[i].Type = type.Key;
				}

				ImGui.EndCombo();
			}

			ImGui.PopItemWidth();

			ImGui.SameLine();

			ImGui.PushItemWidth(240);
			string temp = componentField.Name;
			if (ImGui.InputText(Inline.Span($"Field Name##{i}"), ref temp, 100))
				_newComponentTypeFields[i].Name = temp;
			ImGui.PopItemWidth();
		}

		if (ImGui.Button("Add Field"))
			_newComponentTypeFields.Add(new ComponentField());

		if (ImGui.Button("Create Component"))
		{
			if (!string.IsNullOrWhiteSpace(_newComponentTypeName))
			{
				ImGui.CloseCurrentPopup();
				TypeInfo newType = ConstructComponent();
				Reset();
				return newType;
			}
		}

		if (ImGui.Button("Cancel"))
			ImGui.CloseCurrentPopup();

		return null;
	}

	private TypeInfo ConstructComponent()
	{
		List<FieldDescriptor> fieldDescriptors = _newComponentTypeFields.Where(f => f.Type != null).Select(cf => new FieldDescriptor(cf.Name, cf.Type!)).ToList();
		return ComponentTypeBuilder.CompileResultTypeInfo(_newComponentTypeName, fieldDescriptors);
	}

	private record ComponentField
	{
		public string Name { get; set; } = string.Empty;
		public Type? Type { get; set; }
	}
}
