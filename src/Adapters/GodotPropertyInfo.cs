using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Raele.GodotUtils.Adapters;

public record GodotPropertyInfo
{
	public required string Name { get; init; }
	public string ClassName { get; init; } = "";
	public Variant.Type Type { get; init; } = Variant.Type.Nil;
	public PropertyHint Hint { get; init; } = PropertyHint.None;
	public string HintString { get; init; } = "";
	public long UsageBitmask { get; init; } = 0;
	public Variant DefaultValue { get; init; } = new Variant();
	public HashSet<PropertyUsageFlags> Usage
	{
		get => field ??= Enumerable.Range(0, 64)
			.Select(bit => 1L << bit)
			.Where(mask => (this.UsageBitmask & mask) != 0)
			.Select(mask => (PropertyUsageFlags) mask)
			.ToHashSet();
		init
		{
			field = value;
			this.UsageBitmask = value.Aggregate(0L, (acc, flag) => acc | (long) flag);
		}
	}

	public Godot.Collections.Dictionary ToDictionary()
		=> new()
		{
			["name"] = this.Name,
			["class_name"] = this.ClassName,
			["type"] = (long) this.Type,
			["hint"] = (long) this.Hint,
			["hint_string"] = this.HintString,
			["usage"] = this.UsageBitmask,
			["default_value"] = this.DefaultValue,
		};

	public static GodotPropertyInfo FromDictionary(Godot.Collections.Dictionary dict)
	{
		long usageFlags = dict["usage"].AsInt64();
		return new GodotPropertyInfo
		{
			Name = dict["name"].AsString(),
			ClassName = dict["class_name"].AsString(),
			Type = (Variant.Type) dict["type"].AsInt64(),
			Hint = (PropertyHint) dict["hint"].AsInt64(),
			HintString = dict["hint_string"].AsString(),
			Usage = Enumerable.Range(0, 64)
				.Select(bit => 1L << bit)
				.Where(mask => (usageFlags & mask) != 0)
				.Select(mask => (PropertyUsageFlags) mask)
				.ToHashSet(),
			DefaultValue = dict["default_value"],
		};
	}
}
