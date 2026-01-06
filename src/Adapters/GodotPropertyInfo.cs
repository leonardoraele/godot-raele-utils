using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Raele.GodotUtils.Adapters;

public record GodotPropertyInfo
{
	//==================================================================================================================
	// STATICS
	//==================================================================================================================

	public static Godot.Collections.Dictionary ToGodotDictionary(GodotPropertyInfo subject)
		=> subject._dict;
	public static GodotPropertyInfo FromGodotDictionary(Godot.Collections.Dictionary dict)
		=> new() { Name = "", _dict = dict };

	//==================================================================================================================
	// FIELDS
	//==================================================================================================================

	private Godot.Collections.Dictionary _dict = new();

	//==================================================================================================================
	// COMPUTED PROPERTIES
	//==================================================================================================================

	public required string Name
	{
		get => _dict["name"].AsString();
		init => _dict["name"] = value;
	}
	public string ClassName
	{
		get => _dict.GetValueOrDefault("class_name", "").AsString();
		init => _dict["class_name"] = value;
	}
	public Variant.Type Type
	{
		get => (Variant.Type) _dict.GetValueOrDefault("type", (long) Variant.Type.Nil).AsInt64();
		init => _dict["type"] = (long) value;
	}
	public PropertyHint Hint
	{
		get => (PropertyHint) _dict.GetValueOrDefault("hint", (long) PropertyHint.None).AsInt64();
		init => _dict["hint"] = (long) value;
	}
	public string HintString
	{
		get => _dict.GetValueOrDefault("hint_string", "").AsString();
		init => _dict["hint_string"] = value;
	}
	public long UsageBitmask
	{
		get => _dict.GetValueOrDefault("usage", (long) PropertyUsageFlags.Default).AsInt64();
		init => _dict["usage"] = value;
	}
	public Variant DefaultValue
	{
		get => _dict.GetValueOrDefault("default_value", new Variant());
		init => _dict["default_value"] = value;
	}
	public HashSet<PropertyUsageFlags> Usage
	{
		get => Enumerable.Range(0, 64)
			.Select(bit => 1L << bit)
			.Where(mask => (this.UsageBitmask & mask) != 0)
			.Select(mask => (PropertyUsageFlags) mask)
			.ToHashSet();
		init => this.UsageBitmask = value.Aggregate(0L, (acc, flag) => acc | (long) flag);
	}

	//==================================================================================================================
	// METHODS
	//==================================================================================================================

	public Godot.Collections.Dictionary ToGodotDictionary() => ToGodotDictionary(this);
}
