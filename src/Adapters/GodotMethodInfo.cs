using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Raele.GodotUtils.Adapters;

public record GodotMethodInfo
{
	public required string Name { get; init; }
	public required GodotPropertyInfo[] Params { get; init; }
	public required Variant[] DefaultArgs { get; init; }
	public required HashSet<MethodFlags> Flags { get; init; }
	public long FlagsBitmask => this.Flags.Aggregate(0L, (acc, flag) => acc | (long) flag);
	public required long Id { get; init; }
	public required GodotPropertyInfo Return { get; init; }

	public static GodotMethodInfo FromDictionary(Godot.Collections.Dictionary dict)
	{
		long flags = dict["flags"].AsInt64();
		return new GodotMethodInfo
		{
			Name = dict["name"].AsString(),
			Params = dict["args"].AsGodotArray()
				.Select(arg => arg.AsGodotDictionary())
				.Select(GodotPropertyInfo.FromDictionary)
				.ToArray(),
			DefaultArgs = dict["default_args"].AsGodotArray().ToArray(),
			Flags = Enumerable.Range(0, 64)
				.Select(bit => 1L << bit)
				.Where(mask => (flags & mask) != 0)
				.Select(mask => (MethodFlags) mask)
				.ToHashSet(),
			Id = dict["id"].AsInt64(),
			Return = GodotPropertyInfo.FromDictionary(dict["return"].AsGodotDictionary()),
		};
	}
}
