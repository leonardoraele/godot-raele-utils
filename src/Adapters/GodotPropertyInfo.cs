using System.Collections.Generic;
using System.Linq;
using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.Adapters;

public record GodotPropertyInfo
{
	//==================================================================================================================
	// STATICS
	//==================================================================================================================

	public static Godot.Collections.Dictionary ToGodotDictionary(GodotPropertyInfo subject)
		=> subject.Dictionary.Duplicate();
	public static GodotPropertyInfo FromGodotDictionary(Godot.Collections.Dictionary dict)
		=> new(dict);
	public static Godot.Collections.Array<Godot.Collections.Dictionary> BuildPropertyList(GodotPropertyInfo[] properties)
		=> properties.Select(ToGodotDictionary).ToGodotArrayT();

	public static void Set(
		Godot.Collections.Dictionary dict,
		string? name = null,
		string? className = null,
		Variant.Type? type = null,
		PropertyHint? hint = null,
		string? hintString = null,
		long? usageBitmask = null,
		PropertyUsageFlags[]? usage = null,
		Variant? defaultValue = null
	)
	{
		if (name != null) dict["name"] = name;
		if (className != null) dict["class_name"] = className;
		if (type.HasValue) dict["type"] = (long) type.Value;
		if (hint.HasValue) dict["hint"] = (long) hint.Value;
		if (hintString != null) dict["hint_string"] = hintString;
		if (usageBitmask.HasValue) dict["usage"] = Enumerable.Range(0, 64)
			.Select(bit => 1L << bit)
			.Where(mask => (usageBitmask.Value & mask) != 0)
			.Aggregate(0L, (acc, flag) => acc | flag);
		if (usage != null) dict["usage"] = usage.Aggregate(0L, (acc, flag) => acc | (long) flag);
		if (defaultValue.HasValue) dict["default_value"] = defaultValue.Value;
	}

	public GodotPropertyInfo()
		=> this.Dictionary = new();
	public GodotPropertyInfo(Godot.Collections.Dictionary dict)
		=> this.Dictionary = dict.Duplicate();
	public GodotPropertyInfo(GodotPropertyInfo other)
		=> this.Dictionary = other.Dictionary.Duplicate();

	//==================================================================================================================
	// FIELDS
	//==================================================================================================================

	public Godot.Collections.Dictionary Dictionary;

	//==================================================================================================================
	// COMPUTED PROPERTIES
	//==================================================================================================================

	public string Name
	{
		get => Dictionary["name"].AsString();
		init => Dictionary["name"] = value;
	}
	public bool HasName => this.Dictionary.ContainsKey("name");
	public string ClassName
	{
		get => Dictionary.GetValueOrDefault("class_name", "").AsString();
		init => Dictionary["class_name"] = value;
	}
	public bool HasClassName => this.Dictionary.ContainsKey("class_name");
	public Variant.Type Type
	{
		get => (Variant.Type) Dictionary.GetValueOrDefault("type", (long) Variant.Type.Nil).AsInt64();
		init => Dictionary["type"] = (long) value;
	}
	public bool HasType => this.Dictionary.ContainsKey("type");
	public PropertyHint Hint
	{
		get => (PropertyHint) Dictionary.GetValueOrDefault("hint", (long) PropertyHint.None).AsInt64();
		init => Dictionary["hint"] = (long) value;
	}
	public bool HasHint => this.Dictionary.ContainsKey("hint");
	public string HintString
	{
		get => Dictionary.GetValueOrDefault("hint_string", "").AsString();
		init => Dictionary["hint_string"] = value;
	}
	public bool HasHintString => this.Dictionary.ContainsKey("hint_string");
	public long UsageBitmask
	{
		get => Dictionary.GetValueOrDefault("usage", (long) PropertyUsageFlags.Default).AsInt64();
		init => Dictionary["usage"] = value;
	}
	public bool HasUsageBitmask => this.Dictionary.ContainsKey("usage");
	public HashSet<PropertyUsageFlags> Usage
	{
		get => Enumerable.Range(0, 64)
			.Select(bit => 1L << bit)
			.Where(mask => (this.UsageBitmask & mask) != 0)
			.Select(mask => (PropertyUsageFlags) mask)
			.ToHashSet();
		init => this.UsageBitmask = value.Aggregate(0L, (acc, flag) => acc | (long) flag);
	}
	public bool HasUsage => this.Dictionary.ContainsKey("usage");
	public Variant DefaultValue
	{
		get => Dictionary.GetValueOrDefault("default_value", Variant.NULL);
		init => Dictionary["default_value"] = value;
	}
	public bool HasDefaultValue => this.Dictionary.ContainsKey("default_value");

	//==================================================================================================================
	// METHODS
	//==================================================================================================================

	public void MergeInto(Godot.Collections.Dictionary dict)
		=> dict.Merge(this.Dictionary, overwrite: true);
	public Godot.Collections.Dictionary ToGodotDictionary() => ToGodotDictionary(this);
}

public static class GodotPropertyInfoExtensions
{
	extension (Godot.Collections.Dictionary self)
	{
		public GodotPropertyInfo AsPropertyInfo() => GodotPropertyInfo.FromGodotDictionary(self);
	}
}