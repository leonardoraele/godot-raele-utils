using System;
using System.Numerics;

namespace Raele.GodotUtils.Extensions;

public static class PrimitiveExtensionMethods
{
	extension<T>(T self) where T : struct, IComparable<T>
	{
		public T Clamped(T min, T max)
			=> self.CompareTo(min) < 0 ? min
				: self.CompareTo(max) > 0 ? max
				: self;
		public bool IsBetween(T min, T max, bool minInclusive = true, bool maxInclusive = false)
			=> (minInclusive ? self.CompareTo(min) >= 0 : self.CompareTo(min) > 0)
				&& (maxInclusive ? self.CompareTo(max) <= 0 : self.CompareTo(max) < 0);
	}

	extension(string str)
	{
		public bool IsWhiteSpace() => string.IsNullOrWhiteSpace(str);

		public string TrimStartString(string prefix)
		{
			string current = str;
			while (current.StartsWith(prefix, StringComparison.Ordinal))
				current = current.Substring(prefix.Length);
			return current;
		}
		public string TrimEndString(string suffix)
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
}
