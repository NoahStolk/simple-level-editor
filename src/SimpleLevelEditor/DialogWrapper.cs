using NativeFileDialogSharp;

namespace SimpleLevelEditor;

/// <summary>
/// Wrapper to make sure the <see cref="Dialog"/> class doesn't block the main thread and cause other problems like key states getting stuck.
/// </summary>
public static class DialogWrapper
{
	/// <summary>
	/// Used to prevent multiple dialogs from being opened from the main thread.
	/// </summary>
	public static bool DialogOpen { get; private set; }

	public static void FileOpen(Action<string?> callback, string? filterList = null, string? defaultPath = null)
	{
		if (DialogOpen)
			return;

		DialogOpen = true;
		OpenDialog(callback, async () =>
		{
			await Task.Yield();
			DialogResult dialogResult = Dialog.FileOpen(filterList, defaultPath);
			DialogOpen = false;
			return dialogResult.Path;
		});
	}

	public static void FileSave(Action<string?> callback, string? filterList = null, string? defaultPath = null)
	{
		if (DialogOpen)
			return;

		DialogOpen = true;
		OpenDialog(callback, async () =>
		{
			await Task.Yield();
			DialogResult dialogResult = Dialog.FileSave(filterList, defaultPath);
			DialogOpen = false;
			return dialogResult.Path;
		});
	}

	public static void FolderPicker(Action<string?> callback, string? defaultPath = null)
	{
		if (DialogOpen)
			return;

		DialogOpen = true;
		OpenDialog(callback, async () =>
		{
			await Task.Yield();
			DialogResult dialogResult = Dialog.FolderPicker(defaultPath);
			DialogOpen = false;
			return dialogResult.Path;
		});
	}

	public static void FileOpenMultiple(Action<IReadOnlyList<string>?> callback, string? filterList = null, string? defaultPath = null)
	{
		if (DialogOpen)
			return;

		DialogOpen = true;
		OpenDialog(callback, async () =>
		{
			await Task.Yield();
			DialogResult dialogResult = Dialog.FileOpenMultiple(filterList, defaultPath);
			DialogOpen = false;
			return dialogResult.Paths;
		});
	}

	private static void OpenDialog(Action<string?> callback, Func<Task<string?>> call)
	{
		Task.Run(async () => callback(await call()));
	}

	private static void OpenDialog(Action<IReadOnlyList<string>?> callback, Func<Task<IReadOnlyList<string>?>> call)
	{
		Task.Run(async () => callback(await call()));
	}
}
