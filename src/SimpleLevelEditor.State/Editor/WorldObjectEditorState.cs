using SimpleLevelEditor.Formats.Types.Level;

namespace SimpleLevelEditor.State.Editor;

public static class WorldObjectEditorState
{
	public static WorldObject DefaultObject { get; private set; } = WorldObject.CreateDefault();

	public static void Reset()
	{
		DefaultObject = WorldObject.CreateDefault();
	}
}
