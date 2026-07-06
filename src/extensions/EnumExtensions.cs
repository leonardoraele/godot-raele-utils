using System;
using System.Numerics;

namespace Raele.GodotUtils.Extensions;

public static class EnumExtensions
{
	extension(Enum self)
	{
		public bool HasFlag(Enum flag)
			=> (Convert.ToInt64(self) & Convert.ToInt64(flag)) != 0;
		public Enum Union(Enum flag)
			=> (Enum) Enum.ToObject(self.GetType(), Convert.ToInt64(self) | Convert.ToInt64(flag));
		public Enum Intersection(Enum flag)
			=> (Enum) Enum.ToObject(self.GetType(), Convert.ToInt64(self) & Convert.ToInt64(flag));
		public Enum Difference(Enum flag)
			=> (Enum) Enum.ToObject(self.GetType(), Convert.ToInt64(self) & ~Convert.ToInt64(flag));
		public Enum SymmetricDifference(Enum flag)
			=> (Enum) Enum.ToObject(self.GetType(), Convert.ToInt64(self) ^ Convert.ToInt64(flag));
		public T As<T>() where T : struct, INumber<T>
			=> T.CreateTruncating(Convert.ToInt64(self));
	}
}
