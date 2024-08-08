using SimpleLevelEditor.Formats.Level;

namespace SimpleLevelEditor.State.States.Level;

public sealed record HistoryEntry(Level3dData Level3dData, IReadOnlyList<byte> Hash, string EditDescription);
