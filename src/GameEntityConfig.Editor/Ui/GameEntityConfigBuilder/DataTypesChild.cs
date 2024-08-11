using Detach.Numerics;
using GameEntityConfig.Core;
using GameEntityConfig.Editor.States;
using ImGuiNET;
using System.Numerics;

namespace GameEntityConfig.Editor.Ui.GameEntityConfigBuilder;

public sealed class DataTypesChild
{
	private readonly CreateNewDataTypePopup _createNewDataTypePopup = new();

	public void Render(GameEntityConfigBuilderState state)
	{
		ImGui.SeparatorText("Data Types");

		ImGui.Checkbox("Enable Default Components", ref state.EnableDefaultComponents);

		if (ImGui.BeginTable("DataTypesTable", 3, ImGuiTableFlags.Borders))
		{
			ImGui.TableSetupColumn("Data Type", ImGuiTableColumnFlags.WidthFixed, 200);
			ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, 200);
			ImGui.TableSetupColumn("Fields", ImGuiTableColumnFlags.WidthStretch);

			ImGui.TableSetupScrollFreeze(0, 1);
			ImGui.TableHeadersRow();

			if (state.EnableDefaultComponents)
			{
				foreach (DataType defaultDataType in DataType.DefaultDataTypes)
					RenderComponent(state, true, defaultDataType);
			}

			for (int i = state.DataTypes.Count - 1; i >= 0; i--)
			{
				DataType dataType = state.DataTypes[i];
				RenderComponent(state, false, dataType);
			}

			ImGui.EndTable();
		}

		if (ImGui.Button("Create New Data Type"))
			ImGui.OpenPopup("Create New Data Type");

		ImGui.SetNextWindowSizeConstraints(new Vector2(640, 240), new Vector2(float.MaxValue, float.MaxValue));
		if (ImGui.BeginPopupModal("Create New Data Type"))
		{
			DataType? newDataType = _createNewDataTypePopup.Render();

			if (newDataType != null && state.DataTypes.TrueForAll(ct => ct.Name != newDataType.Name))
				state.DataTypes.Add(newDataType);

			ImGui.EndPopup();
		}
	}

	private static void RenderComponent(GameEntityConfigBuilderState state, bool isDefaultComponent, DataType dataType)
	{
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
			if (ImGui.SmallButton($"Remove##{dataType}"))
				state.DataTypes.Remove(dataType);
		}

		ImGui.TableNextColumn();
		ImGui.Text(dataType.Name);

		ImGui.TableNextColumn();
		if (dataType.Fields.Count == 0)
		{
			ImGui.TextColored(Rgba.Gray(0.5f), "No fields");
		}
		else
		{
			foreach (DataTypeField field in dataType.Fields)
			{
				ImGui.TextColored(Rgba.Green, field.Primitive.ToString());
				ImGui.SameLine();
				ImGui.TextColored(Rgba.White, field.Name);
			}
		}
	}
}
