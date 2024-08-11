using System.Globalization;

namespace Editor.Utils;

public static class DateTimeUtils
{
	public static string FormatTimeAgo(DateTime dateTime)
	{
		TimeSpan diff = DateTime.UtcNow - dateTime;
		if (diff < TimeSpan.FromSeconds(1))
			return "Just now";

		int seconds = diff.Seconds;
		if (diff < TimeSpan.FromMinutes(1))
			return $"{seconds} second{S(seconds)} ago";

		int minutes = diff.Minutes;
		if (diff < TimeSpan.FromHours(1))
			return $"{minutes} minute{S(minutes)} ago";

		int hours = diff.Hours;
		if (diff < TimeSpan.FromDays(1))
			return $"{hours} hour{S(hours)} and {minutes} minute{S(minutes)} ago";

		return dateTime.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

		static string S(int value)
		{
			return value == 1 ? string.Empty : "s";
		}
	}
}
