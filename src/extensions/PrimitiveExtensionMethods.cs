using System;
using System.Numerics;
using Godot;

namespace Raele.GodotUtils.Extensions;

public static class PrimitiveExtensionMethods
{
	extension<T>(T self) where T : struct, IComparable<T>
	{
		public T AtMost(T max)
			=> self.CompareTo(max) > 0 ? max : self;
		public T AtLeast(T min)
			=> self.CompareTo(min) < 0 ? min : self;
		public T Clamped(T min, T max)
			=> self.CompareTo(min) < 0 ? min
				: self.CompareTo(max) > 0 ? max
				: self;
		public bool IsBetween(T min, T max, bool minInclusive = true, bool maxInclusive = false)
			=> (minInclusive ? self.CompareTo(min) >= 0 : self.CompareTo(min) > 0)
				&& (maxInclusive ? self.CompareTo(max) <= 0 : self.CompareTo(max) < 0);
	}

	extension<T>(T self) where T : struct, INumber<T>
	{
		public T Abs()
			=> self < T.Zero ? -self : self;
		public T MoveToward(T target, T delta)
			=> self < target ? T.Min(self + delta, target)
				: self > target ? T.Max(self - delta, target)
				: target;
		public int Sign()
			=> self > T.Zero ? 1
				: self < T.Zero ? -1
				: 0;
	}

	extension(bool self)
	{
		public byte ToInt()
			=> (byte) (self ? 1 : 0);
	}

	extension(float self)
	{
		public bool IsZeroApprox()
			=> self.Abs() < Mathf.Epsilon;
		public bool IsEqualApprox(float other)
			=> (self - other).Abs() < Mathf.Epsilon;
	}

	extension (double self)
	{
		public bool IsZeroApprox()
			=> self.Abs() < Mathf.Epsilon;
		public bool IsEqualApprox(double other)
			=> (self - other).Abs() < Mathf.Epsilon;
	}
}
