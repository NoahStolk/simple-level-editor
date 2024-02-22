using SimpleLevelEditor.Formats.Level.Model;

namespace SimpleLevelEditor.State;

public sealed record HistoryEntry(Level3dData Level3dData, IReadOnlyList<byte> Hash, string EditDescription);
