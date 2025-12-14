using System;

public static class PrimitiveExtensionMethods
{
	extension<T>(T self) where T : struct, IComparable<T>
	{
		public T Clamp(T min, T max)
			=> self.CompareTo(min) < 0 ? min
				: self.CompareTo(max) > 0 ? max
				: self;

		public bool IsBetween(T min, T max, bool inclusive = true)
			=> inclusive
				? self.CompareTo(min) >= 0 && self.CompareTo(max) <= 0
				: self.CompareTo(min) > 0 && self.CompareTo(max) < 0;
	}
}
