using System;
using System.Collections.Generic;
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

	extension<T>(T self) where T : struct, INumber<T>, IShiftOperators<T, int, T>, IBitwiseOperators<T, T, T>
	{
		public bool HasBitSet(int index)
			=> (self & (T.One << index)) != T.Zero;
		public bool HasAnyBitSet()
			=> self != T.Zero;
		public T SetBit(int index, bool value)
			=> value
				? self.SetBit(index)
				: self.UnsetBit(index);
		public T SetBit(int index)
			=> self | (T.One << index);
		public T UnsetBit(int index)
			=> self & ~(T.One << index);
		public T ToggleBit(int index)
			=> self ^ (T.One << index);
		public T BitUnion(T other)
			=> self | other;
		public T BitIntersection(T other)
			=> self & other;
		public T BitDifference(T other)
			=> self & ~other;
		public T BitSymmetricDifference(T other)
			=> self ^ other;
		public T BitFill(int startIndex, int count)
			=> self | (((T.One << count) - T.One) << startIndex);
	}

	extension(uint self)
	{
		public IEnumerable<int> GetSetBitIndices()
		{
			for (int i = 0; i < sizeof(uint) * 8; i++)
				if (self.HasBitSet(i))
					yield return i;
		}
	}

	extension(ulong self)
	{
		public IEnumerable<int> GetSetBitIndices()
		{
			for (int i = 0; i < sizeof(ulong) * 8; i++)
				if (self.HasBitSet(i))
					yield return i;
		}
	}

	extension<T>(T self) where T : struct, IFloatingPoint<T>
	{
		public T Lerp(T to, T weight)
			=> self + (to - self) * weight;
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

	extension(bool self)
	{
		public byte ToInt()
			=> (byte) (self ? 1 : 0);
	}
}
