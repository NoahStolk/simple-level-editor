using Detach.Numerics;
using GameEntityConfig.Editor.States;
using GameEntityConfig.Editor.Utils;
using ImGuiNET;
using System.Numerics;
using System.Reflection;

namespace GameEntityConfig.Editor.Ui.GameEntityConfigBuilder;

public sealed class ComponentsChild
{
	private readonly CreateNewComponentPopup _createNewComponentPopup = new();

	public void Render(GameEntityConfigBuilderState state)
	{
		ImGui.SeparatorText("Components");

		ImGui.Checkbox("Enable Default Components", ref state.EnableDefaultComponents);

		if (ImGui.BeginTable("ComponentsTable", 3, ImGuiTableFlags.Borders))
		{
			ImGui.TableSetupColumn("Component Type", ImGuiTableColumnFlags.WidthFixed, 200);
			ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, 200);
			ImGui.TableSetupColumn("Fields", ImGuiTableColumnFlags.WidthStretch);

			ImGui.TableSetupScrollFreeze(0, 1);
			ImGui.TableHeadersRow();

			if (state.EnableDefaultComponents)
			{
				foreach (TypeInfo defaultComponent in ComponentUtils.DefaultComponents)
					RenderComponent(state, true, defaultComponent);
			}

			for (int i = state.ComponentTypes.Count - 1; i >= 0; i--)
			{
				TypeInfo componentType = state.ComponentTypes[i];
				RenderComponent(state, false, componentType);
			}

			ImGui.EndTable();
		}

		if (ImGui.Button("Create New Component"))
			ImGui.OpenPopup("Create New Component");

		ImGui.SetNextWindowSizeConstraints(new Vector2(640, 240), new Vector2(float.MaxValue, float.MaxValue));
		if (ImGui.BeginPopupModal("Create New Component"))
		{
			TypeInfo? newComponent = _createNewComponentPopup.Render();

			if (newComponent != null && state.ComponentTypes.TrueForAll(ct => ct.Name != newComponent.Name))
				state.ComponentTypes.Add(newComponent);

			ImGui.EndPopup();
		}
	}

	private static void RenderComponent(GameEntityConfigBuilderState state, bool isDefaultComponent, TypeInfo componentType)
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
				state.ComponentTypes.Remove(componentType);
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
