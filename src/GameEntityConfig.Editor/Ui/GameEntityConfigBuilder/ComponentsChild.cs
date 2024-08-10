using Detach.Numerics;
using GameEntityConfig.Core.Components;
using ImGuiNET;
using System.Numerics;
using System.Reflection;

namespace GameEntityConfig.Editor.Ui.GameEntityConfigBuilder;

public sealed class ComponentsChild
{
	private static readonly List<TypeInfo> _defaultComponents =
	[
		typeof(DiffuseColor).GetTypeInfo(),
		typeof(Position).GetTypeInfo(),
		typeof(Rotation).GetTypeInfo(),
		typeof(Scale).GetTypeInfo(),
		typeof(Shape).GetTypeInfo(),
		typeof(Visualizer).GetTypeInfo(),
	];

	private bool _enableDefaultComponents;
	private readonly List<TypeInfo> _componentTypes = [];

	private readonly CreateNewComponentPopup _createNewComponentPopup = new();

	public void Render()
	{
		ImGui.SeparatorText("Components");

		ImGui.Checkbox("Enable Default Components", ref _enableDefaultComponents);

		if (ImGui.BeginTable("ComponentsTable", 3, ImGuiTableFlags.Borders))
		{
			ImGui.TableSetupColumn("Component Type", ImGuiTableColumnFlags.WidthFixed, 200);
			ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, 200);
			ImGui.TableSetupColumn("Fields", ImGuiTableColumnFlags.WidthStretch);

			ImGui.TableSetupScrollFreeze(0, 1);
			ImGui.TableHeadersRow();

			if (_enableDefaultComponents)
			{
				foreach (TypeInfo defaultComponent in _defaultComponents)
					RenderComponent(true, defaultComponent);
			}

			for (int i = _componentTypes.Count - 1; i >= 0; i--)
			{
				TypeInfo componentType = _componentTypes[i];
				RenderComponent(false, componentType);
			}

			ImGui.EndTable();
		}

		if (ImGui.Button("Create New Component"))
			ImGui.OpenPopup("Create New Component");

		ImGui.SetNextWindowSizeConstraints(new Vector2(640, 240), new Vector2(float.MaxValue, float.MaxValue));
		if (ImGui.BeginPopupModal("Create New Component"))
		{
			TypeInfo? newComponent = _createNewComponentPopup.Render();

			if (newComponent != null && _componentTypes.All(ct => ct.Name != newComponent.Name))
				_componentTypes.Add(newComponent);

			ImGui.EndPopup();
		}
	}

	private void RenderComponent(bool isDefaultComponent, TypeInfo componentType)
	{
		FieldInfo[] fields = componentType.GetFields(BindingFlags.Public | BindingFlags.Instance);

		ImGui.TableNextRow();

		ImGui.TableNextColumn();

		if (isDefaultComponent)
		{
			ImGui.Text("Default component");
		}
		else
		{
			ImGui.Text("Custom component");
			ImGui.SameLine();
			if (ImGui.SmallButton($"Remove##{componentType.Name}"))
				_componentTypes.Remove(componentType);
		}

		ImGui.TableNextColumn();

		ImGui.Text(componentType.Name);

		ImGui.TableNextColumn();

		if (fields.Length == 0)
		{
			ImGui.TextColored(Rgba.Gray(0.5f), "No fields");
		}
		else
		{
			foreach (FieldInfo field in fields)
			{
				ImGui.TextColored(Rgba.Green, field.FieldType.Name);
				ImGui.SameLine();
				ImGui.TextColored(Rgba.White, field.Name);
			}
		}
	}
}
