using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace Raele.GodotUtils.Extensions;

public static class GodotExtensionMethods
{
	extension(Engine)
	{
		public static SceneTree GetSceneTree() => (SceneTree) Engine.GetMainLoop();
	}

	extension (SignalAwaiter self)
	{
		public async Task<Variant[]> ToTask()
			=> await self;
		public Task<Variant[]> ToTask(GodotCancellationToken token)
		{
			TaskCompletionSource<Variant[]> source = new();
			token.Register(source.SetCanceled);
			return Task.WhenAny(self.ToTask(), source.Task).Unwrap();
		}
	}

	extension<[MustBeVariant] T>(IEnumerable<T> sequence)
	{
		public Godot.Collections.Array ToGodotArray() => [..sequence.Select(item => Variant.From(item))];
		public Godot.Collections.Array<T> ToGodotArrayT() => [..sequence];
	}

	extension<[MustBeVariant] TKey, [MustBeVariant] TValue>(Dictionary<TKey, TValue> dictionary) where TKey : notnull
	{
		public Godot.Collections.Dictionary ToGodotDictionary()
		{
			Godot.Collections.Dictionary godotDict = new();
			foreach (KeyValuePair<TKey, TValue> pair in dictionary)
			{
				godotDict[Variant.From(pair.Key)] = Variant.From(pair.Value);
			}
			return godotDict;
		}
		public Godot.Collections.Dictionary<TKey, TValue> ToGodotDictionaryT()
		{
			Godot.Collections.Dictionary<TKey, TValue> godotDict = new();
			foreach (KeyValuePair<TKey, TValue> pair in dictionary)
			{
				godotDict[pair.Key] = pair.Value;
			}
			return godotDict;
		}
	}

	extension<[MustBeVariant] TKey, [MustBeVariant] TValue>(IEnumerable<KeyValuePair<TKey, TValue>> self) where TKey : notnull
	{
		public Godot.Collections.Dictionary ToGodotDictionary()
			=> self.ToDictionary().ToGodotDictionary();

		public Godot.Collections.Dictionary<TKey, TValue> ToGodotDictionaryT()
			=> self.ToDictionary().ToGodotDictionaryT();
	}

	extension (Godot.Collections.Dictionary self)
	{
		public Godot.Collections.Dictionary MergeWith(params Godot.Collections.Dictionary[] others)
		{
			foreach (Godot.Collections.Dictionary other in others)
				self.Merge(other);
			return self;
		}
	}

	extension<[MustBeVariant] TKey, [MustBeVariant] TValue>(Godot.Collections.Dictionary<TKey, TValue> self) where TKey : notnull
	{
		public Godot.Collections.Dictionary<TKey, TValue> MergeWith(params Godot.Collections.Dictionary<TKey, TValue>[] others)
		{
			foreach (Godot.Collections.Dictionary<TKey, TValue> other in others)
				self.Merge(other);
			return self;
		}
	}
}
