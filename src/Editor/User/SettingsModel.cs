using System.Globalization;
using System.Text;

namespace Editor.User;

public sealed class SettingsModel
{
	public bool StartMaximized { get; set; }

	public byte[] Write()
	{
		// Save settings to file
		StringBuilder sb = new();
		sb.Append(CultureInfo.InvariantCulture, $"{nameof(StartMaximized)}={StartMaximized}");
		sb.AppendLine();
		return Encoding.ASCII.GetBytes(sb.ToString());
	}

	public void Read(byte[] data)
	{
		// Load settings from file
		string[] lines = Encoding.ASCII.GetString(data).Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
		foreach (string line in lines)
		{
			string[] parts = line.Split('=', 2);
			if (parts.Length != 2)
				continue;

			switch (parts[0])
			{
				case nameof(StartMaximized):
					StartMaximized = bool.TryParse(parts[1], out bool startMaximized) && startMaximized;
					break;
			}
		}
	}
}
