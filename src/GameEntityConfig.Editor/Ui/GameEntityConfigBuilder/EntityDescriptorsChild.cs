using GameEntityConfig.Core;
using ImGuiNET;
using System.Numerics;

namespace GameEntityConfig.Editor.Ui.GameEntityConfigBuilder;

public sealed class EntityDescriptorsChild
{
	private readonly List<EntityDescriptor> _entityDescriptors = [];

	private readonly CreateNewEntityDescriptorPopup _createNewEntityDescriptorPopup = new();

	public void Render()
	{
		ImGui.SeparatorText("Entity Descriptors");

		for (int i = _entityDescriptors.Count - 1; i >= 0; i--)
		{
			EntityDescriptor entityDescriptor = _entityDescriptors[i];
			if (ImGui.Button($"X##{entityDescriptor.Name}"))
				_entityDescriptors.Remove(entityDescriptor);
			ImGui.SameLine();
			ImGui.Text(entityDescriptor.Name);

			ImGui.Indent();

			ImGui.SeparatorText("Fixed Components");

			for (int j = entityDescriptor.FixedComponents.Count - 1; j >= 0; j--)
			{
				FixedComponent fixedComponent = entityDescriptor.FixedComponents[j];
				string fixedComponentTypeName = fixedComponent.GetType().Name;
				ImGui.Button($"X##{fixedComponentTypeName}");
				ImGui.SameLine();
				ImGui.Text(fixedComponentTypeName);
			}

			ImGui.SeparatorText("Varying Components");

			for (int j = entityDescriptor.VaryingComponents.Count - 1; j >= 0; j--)
			{
				VaryingComponent varyingComponent = entityDescriptor.VaryingComponents[j];
				string varyingComponentTypeName = varyingComponent.GetType().Name;
				ImGui.Button($"X##{varyingComponentTypeName}");
				ImGui.SameLine();
				ImGui.Text(varyingComponentTypeName);
			}

			ImGui.Unindent();
		}

		if (ImGui.Button("Create New Entity Descriptor"))
			ImGui.OpenPopup("Create New Entity Descriptor");

		ImGui.SetNextWindowSizeConstraints(new Vector2(640, 240), new Vector2(float.MaxValue, float.MaxValue));
		if (ImGui.BeginPopupModal("Create New Entity Descriptor"))
		{
			EntityDescriptor? newEntityDescriptor = _createNewEntityDescriptorPopup.Render();

			if (newEntityDescriptor != null && _entityDescriptors.TrueForAll(ct => ct.Name != newEntityDescriptor.Name))
				_entityDescriptors.Add(newEntityDescriptor);

			ImGui.EndPopup();
		}
	}
}
