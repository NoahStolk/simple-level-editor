using SimpleLevelEditor.Formats.Level;

namespace SimpleLevelEditor.State.States.EntityEditor;

public static class EntityEditorState
{
	public static Entity DefaultEntity { get; private set; } = Entity.CreateDefault();

	public static void Reset()
	{
		DefaultEntity = Entity.CreateDefault();
	}
}
