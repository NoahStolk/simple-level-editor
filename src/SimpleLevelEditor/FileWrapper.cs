namespace SimpleLevelEditor;

public static class FileWrapper
{
	// TODO: Fix.
	public static byte[] ForceReadAllBytes(string path)
	{
		while (true)
		{
			try
			{
				return File.ReadAllBytes(path);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
