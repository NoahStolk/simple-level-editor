using SimpleLevelEditor.Formats.Level;

namespace SimpleLevelEditor.State.States.WorldObjectEditor;

public static class WorldObjectEditorState
{
	public static WorldObject DefaultObject { get; private set; } = WorldObject.CreateDefault();

	public static void Reset()
	{
		DefaultObject = WorldObject.CreateDefault();
	}
}
