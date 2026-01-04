using System;
using System.Collections.Generic;
using System.Linq;

namespace Raele.GodotUtils.Extensions;

public static class IEnumerableExtensionMethods
{
	extension<T>(IEnumerable<T> self)
	{
		public void ForEach(Action<T> action)
		{
			foreach (T item in self) {
				action(item);
			}
		}

		public void ForEach(Action<T, int> action)
		{
			int i = 0;
			foreach (T item in self) {
				action(item, i++);
			}
		}

		public IEnumerable<T> Through(Action<T> action)
		{
			foreach (T item in self) {
				action(item);
			}
			return self;
		}

		public IEnumerable<T> Through(Action<T, int> action)
		{
			int i = 0;
			foreach (T item in self) {
				action(item, i++);
			}
			return self;
		}

		public T ElementAtOrDefault(int index, T defaultValue)
		{
			try
			{
				return self.ElementAt(index);
			}
			catch (ArgumentOutOfRangeException)
			{
				return defaultValue;
			}
		}

		public int FindIndex(Func<T, bool> predicate)
		{
			IEnumerator<T> enumerator = self.GetEnumerator();
			for (int i = 0; enumerator.MoveNext(); i++) {
				if (predicate(enumerator.Current)) {
					return i;
				}
			}
			return -1;
		}

		public bool TryFindIndex(out int index, Func<T, bool> predicate)
		{
			index = self.FindIndex(predicate);
			return index != -1;
		}

		public string JoinIntoString(string separator) => string.Join(separator, self);

		public void Deconstruct(out T? el1)
		{
			using IEnumerator<T> enumerator = self.GetEnumerator();
			el1 = enumerator.MoveNext() ? enumerator.Current : default;
		}
		public void Deconstruct(out T? el1, out T? el2)
		{
			using IEnumerator<T> enumerator = self.GetEnumerator();
			el1 = enumerator.MoveNext() ? enumerator.Current : default;
			el2 = enumerator.MoveNext() ? enumerator.Current : default;
		}
		public void Deconstruct(out T? el1, out T? el2, out T? el3)
		{
			using IEnumerator<T> enumerator = self.GetEnumerator();
			el1 = enumerator.MoveNext() ? enumerator.Current : default;
			el2 = enumerator.MoveNext() ? enumerator.Current : default;
			el3 = enumerator.MoveNext() ? enumerator.Current : default;
		}
		public void Deconstruct(out T? el1, out T? el2, out T? el3, out T? el4)
		{
			using IEnumerator<T> enumerator = self.GetEnumerator();
			el1 = enumerator.MoveNext() ? enumerator.Current : default;
			el2 = enumerator.MoveNext() ? enumerator.Current : default;
			el3 = enumerator.MoveNext() ? enumerator.Current : default;
			el4 = enumerator.MoveNext() ? enumerator.Current : default;
		}
	}
}
