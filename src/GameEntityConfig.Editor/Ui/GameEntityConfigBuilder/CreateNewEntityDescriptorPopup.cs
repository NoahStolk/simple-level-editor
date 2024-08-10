using GameEntityConfig.Core;
using ImGuiNET;
using System.Numerics;

namespace GameEntityConfig.Editor.Ui.GameEntityConfigBuilder;

public sealed class CreateNewEntityDescriptorPopup
{
	private readonly EntityDescriptorBuilder _entityDescriptorBuilder = new();

	private string _name = string.Empty;

	private void Reset()
	{
		_name = string.Empty;
	}

	public EntityDescriptor? Render()
	{
		if (ImGui.BeginChild("NewEntityDescriptorChildWindow", new Vector2(0, ImGui.GetWindowHeight() - 96)))
		{
			ImGui.InputText("Name", ref _name, 100);
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

	private EntityDescriptor ConstructEntityDescriptor()
	{
		EntityDescriptor entityDescriptor = _entityDescriptorBuilder
			.WithName(_name)
			.Build();

		return entityDescriptor;
	}
}
