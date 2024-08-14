using Detach.Numerics;
using ImGuiNET;
using SimpleLevelEditorV2.Formats.GameEntityConfig.Model;
using SimpleLevelEditorV2.States.GameEntityConfigBuilder;

namespace SimpleLevelEditorV2.Ui.GameEntityConfigBuilder;

public sealed class GameEntityConfigDataTypesWindow
{
	private readonly CreateNewDataTypePopup _createNewDataTypePopup = new();

	public void Render(GameEntityConfigBuilderState state, Vector2 initialWindowSize)
	{
		ImGuiIOPtr io = ImGui.GetIO();
		Vector2 screenCenter = new(io.DisplaySize.X / 2, io.DisplaySize.Y / 2);
		ImGui.SetNextWindowPos(screenCenter - initialWindowSize with { Y = 0 }, ImGuiCond.Appearing);
		ImGui.SetNextWindowSize(initialWindowSize, ImGuiCond.Appearing);
		if (ImGui.Begin("Data Types"))
		{
			if (ImGui.Button("Add Default Data Types"))
			{
				foreach (DataType defaultDataType in DataType.DefaultDataTypes)
				{
					if (state.DataTypes.TrueForAll(ct => ct.Name != defaultDataType.Name))
						state.DataTypes.Add(defaultDataType);
				}
			}

			if (ImGui.BeginTable("DataTypesTable", 3, ImGuiTableFlags.Borders))
			{
				ImGui.TableSetupColumn("Data Type", ImGuiTableColumnFlags.WidthFixed, 200);
				ImGui.TableSetupColumn("Name", ImGuiTableColumnFlags.WidthFixed, 200);
				ImGui.TableSetupColumn("Fields", ImGuiTableColumnFlags.WidthStretch);

				ImGui.TableSetupScrollFreeze(0, 1);
				ImGui.TableHeadersRow();

				for (int i = state.DataTypes.Count - 1; i >= 0; i--)
				{
					DataType dataType = state.DataTypes[i];
					RenderDataType(state, dataType);
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

		ImGui.End();
	}

	private static void RenderDataType(GameEntityConfigBuilderState state, DataType dataType)
	{
		ImGui.TableNextRow();

		ImGui.TableNextColumn();
		if (ImGui.SmallButton($"Remove##{dataType}"))
			state.DataTypes.Remove(dataType);

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
