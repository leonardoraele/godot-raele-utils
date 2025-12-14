using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Raele.GodotUtils;

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

		public IEnumerable<T> Unique() => new HashSet<T>(self);

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
	}
}
