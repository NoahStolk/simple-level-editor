using GameEntityConfig.Core;
using ImGuiNET;

namespace GameEntityConfig.Editor.Ui.GameEntityConfigBuilder;

public sealed class EntityDescriptorsChild
{
	private readonly List<EntityDescriptor> _entityDescriptors = [];

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

		ImGui.SeparatorText("Construct New Entity Descriptor");
	}
}
