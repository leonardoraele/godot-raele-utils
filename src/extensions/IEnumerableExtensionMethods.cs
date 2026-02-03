using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

		public IEnumerable<T> AppendIf(bool condition, T element)
			=> condition ? self.Append(element) : self;

		public IEnumerable<T> PrependIf(bool condition, T element)
			=> condition ? self.Prepend(element) : self;

		public IEnumerable<T> ConcatIf(bool condition, IEnumerable<T> elements)
			=> condition ? self.Concat(elements) : self;

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

	extension<T>(IEnumerable<T?> self)
	{
		public IEnumerable<T> WhereNotNull()
		{
			foreach (T? item in self) {
				if (item != null) {
					yield return item;
				}
			}
		}
	}

	extension<T>(IAsyncEnumerable<T> self)
	{
		public async Task ForEach(Action<T> action)
		{
			await foreach (T item in self) {
				action(item);
			}
		}
		public async Task ForEach(Func<T, Task> action)
		{
			await foreach (T item in self) {
				await action(item);
			}
		}
		public async IAsyncEnumerable<T> WhereNotNull()
		{
			await foreach (T? item in self) {
				if (item != null) {
					yield return item;
				}
			}
		}
	}
}