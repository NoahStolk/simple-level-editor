using Detach;
using GameEntityConfig.Core.Components;
using GameEntityConfig.Emit;
using ImGuiNET;
using System.Reflection;

namespace GameEntityConfig.Editor.Ui.GameEntityConfigBuilder;

public sealed class ComponentsChild
{
	private static readonly List<Type> _primitives =
	[
		typeof(bool),

		typeof(sbyte),
		typeof(short),
		typeof(int),
		typeof(long),
		typeof(Int128),

		typeof(byte),
		typeof(ushort),
		typeof(uint),
		typeof(ulong),
		typeof(UInt128),

		typeof(Half),
		typeof(float),
		typeof(double),

		typeof(decimal),

		typeof(char),

		typeof(string),
	];

	private static readonly List<TypeInfo> _defaultComponents =
	[
		typeof(DiffuseColor).GetTypeInfo(),
		typeof(Position).GetTypeInfo(),
		typeof(Rotation).GetTypeInfo(),
		typeof(Scale).GetTypeInfo(),
		typeof(Shape).GetTypeInfo(),
		typeof(Visualizer).GetTypeInfo(),
	];

	private readonly List<TypeInfo> _componentTypes = [];
	private bool _enableDefaultComponents;
	private string _newComponentTypeName = string.Empty;
	private readonly List<ComponentField> _newComponentTypeFields = [];

	public void Render()
	{
		ImGui.SeparatorText("Components");

		ImGui.Checkbox("Enable Default Components", ref _enableDefaultComponents);

		if (_enableDefaultComponents)
		{
			foreach (TypeInfo defaultComponent in _defaultComponents)
				RenderComponent(false, defaultComponent);
		}

		for (int i = _componentTypes.Count - 1; i >= 0; i--)
		{
			TypeInfo componentType = _componentTypes[i];
			RenderComponent(true, componentType);
		}

		ImGui.SeparatorText("Construct New Component");

		ImGui.InputText("Component Type", ref _newComponentTypeName, 100);

		for (int i = 0; i < _newComponentTypeFields.Count; i++)
		{
			ComponentField componentField = _newComponentTypeFields[i];

			if (ImGui.Button(Inline.Span($"X##{i}")))
				_newComponentTypeFields.RemoveAt(i);
			ImGui.SameLine();

			string temp = componentField.Name;
			if (ImGui.InputText(Inline.Span($"Field name {i}"), ref temp, 100))
				_newComponentTypeFields[i].Name = temp;
			ImGui.SameLine();

			if (ImGui.BeginCombo(Inline.Span($"Field type {i}"), componentField.Type?.Name ?? "None"))
			{
				foreach (Type type in _primitives)
				{
					if (ImGui.Selectable(type.Name))
						_newComponentTypeFields[i].Type = type;
				}

				ImGui.EndCombo();
			}
		}

		if (ImGui.Button("Add Field"))
			_newComponentTypeFields.Add(new ComponentField());

		if (ImGui.Button("Create Component"))
		{
			if (!string.IsNullOrWhiteSpace(_newComponentTypeName) && _componentTypes.All(ct => ct.Name != _newComponentTypeName))
				_componentTypes.Add(ConstructComponent());
		}
	}

	private void RenderComponent(bool isRemovable, TypeInfo componentType)
	{
		FieldInfo[] fields = componentType.GetFields(BindingFlags.Public | BindingFlags.Instance);

		if (isRemovable)
		{
			if (ImGui.Button($"X##{componentType.Name}"))
				_componentTypes.Remove(componentType);
			ImGui.SameLine();
		}

		ImGui.Text(Inline.Span($"{componentType.Name} ({fields.Length} {(fields.Length == 1 ? "field" : "fields")})"));

		ImGui.Indent();

		foreach (FieldInfo field in fields)
			ImGui.Text($"{field.FieldType.Name} {field.Name}");

		ImGui.Unindent();
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
