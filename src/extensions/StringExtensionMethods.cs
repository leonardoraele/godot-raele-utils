using System;

namespace Raele.GodotUtils.Extensions;

public static class StringExtensionMethods
{
	extension(string str)
	{
		/// <summary>
		/// Determines whether the specified string is null, empty, or consists only of white-space characters.
		/// </summary>
		public bool IsWhiteSpace() => string.IsNullOrWhiteSpace(str);

		/// <summary>
		/// Trims the specified prefix from the start of the string.
		/// </summary>
		/// <param name="prefix">The prefix to remove.</param>
		/// <returns>The string with the prefix removed.</returns>
		public string TrimStart(string prefix)
		{
			string current = str;
			while (current.StartsWith(prefix, StringComparison.Ordinal))
				current = current.Substring(prefix.Length);
			return current;
		}
		/// <summary>
		/// Trims the specified suffix from the end of the string.
		/// </summary>
		/// <param name="suffix">The suffix to remove.</param>
		/// <returns>The string with the suffix removed.</returns>
		public string TrimEnd(string suffix)
		{
			string current = str;
			while (current.EndsWith(suffix, StringComparison.Ordinal))
				current = current.Substring(0, current.Length - suffix.Length);
			return current;
		}

		public string BBCBold() => $"[b]{str}[/b]";
		public string BBCItalic() => $"[i]{str}[/i]";
		public string BBCStrikethrough() => $"[s]{str}[/s]";
		public string BBCUnderscore() => $"[u]{str}[/u]";
		public string BBCCode() => $"[code]{str}[/code]";
		public string BBCUrl(string url) => $"[url={url}]{str}[/url]";
		public string BBCCenter() => $"[center]{str}[/center]";
		public string BBCRight() => $"[right]{str}[/right]";
		public string BBCColor(string color) => $"[color={color}]{str}[/color]";
		public string BBCBgColor(string color) => $"[bgcolor={color}]{str}[/bgcolor]";
		public string BBCFgColor(string color) => $"[fgcolor={color}]{str}[/fgcolor]";
	}

	extension(string? str)
	{
		/// <summary>
		/// Determines whether the specified string is null, empty, or consists only of white-space characters.
		/// </summary>
		public bool IsNullOrWhiteSpace() => string.IsNullOrWhiteSpace(str);
	}
}
