using System.Collections.Generic;
using System.Linq;
using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.Adapters;

public record GodotMethodInfo
{
	//==================================================================================================================
	// STATICS
	//==================================================================================================================

	//==================================================================================================================
	// FIELDS
	//==================================================================================================================

	private Godot.Collections.Dictionary _dict = new();

	//==================================================================================================================
	// COMPUTED PROPERTIES
	//==================================================================================================================

	public required string Name
	{
		get => this._dict["name"].AsString();
		init => this._dict["name"] = value;
	}
	public required GodotPropertyInfo[] Parameters
	{
		get => this._dict["args"].AsGodotArray()
			.Select(arg => arg.AsGodotDictionary())
			.Select(GodotPropertyInfo.FromGodotDictionary)
			.ToArray();
		init => this._dict["args"] = value.Select(GodotPropertyInfo.ToGodotDictionary).ToGodotArray();
	}
	public required Variant[] DefaultArguments
	{
		get => this._dict["default_args"].AsGodotArray().ToArray();
		init => this._dict["default_args"] = value.ToGodotArray();
	}
	public long FlagsBitmask
	{
		get => this._dict["flags"].AsInt64();
		init => this._dict["flags"] = value;
	}
	public required HashSet<MethodFlags> Flags
	{
		get => Enumerable.Range(0, 64)
			.Select(bit => 1L << bit)
			.Where(mask => (this.FlagsBitmask & mask) != 0)
			.Select(mask => (MethodFlags) mask)
			.ToHashSet();
		init => this.FlagsBitmask = value.Aggregate(0L, (acc, flag) => acc | (long) flag);
	}
	public required long Id
	{
		get => this._dict["id"].AsInt64();
		init => this._dict["id"] = value;
	}
	public required GodotPropertyInfo Return
	{
		get => GodotPropertyInfo.FromGodotDictionary(this._dict["return"].AsGodotDictionary());
		init => this._dict["return"] = GodotPropertyInfo.ToGodotDictionary(value);
	}

	//==================================================================================================================
	// METHODS
	//==================================================================================================================

	public Godot.Collections.Dictionary ToDictionary() => ToDictionary(this);

	public static Godot.Collections.Dictionary ToDictionary(GodotMethodInfo methodInfo) => methodInfo._dict;
	public static GodotMethodInfo FromDictionary(Godot.Collections.Dictionary dict)
		=> new()
		{
			DefaultArguments = default!,
			Flags = default!,
			Id = default,
			Name = default!,
			Parameters = default!,
			Return = default!,
			_dict = dict,
		};
}
