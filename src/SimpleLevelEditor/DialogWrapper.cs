using NativeFileDialogSharp;

namespace SimpleLevelEditor;

/// <summary>
/// Wrapper to make sure key states do not get stuck when opening a dialog.
/// </summary>
public static class DialogWrapper
{
	public static DialogResult FileOpen(string? filterList = null, string? defaultPath = null)
	{
		Input.ForceClear();
		return Dialog.FileOpen(filterList, defaultPath);
	}

	public static DialogResult FileSave(string? filterList = null, string? defaultPath = null)
	{
		Input.ForceClear();
		return Dialog.FileSave(filterList, defaultPath);
	}

	public static DialogResult FolderPicker(string? defaultPath = null)
	{
		Input.ForceClear();
		return Dialog.FolderPicker(defaultPath);
	}

	public static DialogResult FileOpenMultiple(string? dialogFilterList)
	{
		Input.ForceClear();
		return Dialog.FileOpenMultiple(dialogFilterList);
	}
}
