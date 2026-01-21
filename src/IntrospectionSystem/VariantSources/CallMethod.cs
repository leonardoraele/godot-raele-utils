using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Raele.GodotUtils.Adapters;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.IntrospectionSystem.VariantSources;

[Tool][GlobalClass]
public partial class CallMethod : VariantSource
{
	//==================================================================================================================
		#region STATICS
	//==================================================================================================================

	// public static readonly string MyConstant = "";

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region EXPORTS
	//==================================================================================================================

	[ExportCategory(nameof(CallMethod))]
	[Export(PropertyHint.TypeString)] public string TypeName = "";
	[Export] public string Method
		{ get; set { field = value; this.NotifyPropertyListChanged(); } }
		= "";
	/*NoEditor*/ [Export] public Godot.Collections.Array MethodArguments = [];

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region FIELDS
	//==================================================================================================================



	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region COMPUTED PROPERTIES
	//==================================================================================================================

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region EVENTS & SIGNALS
	//==================================================================================================================

	// [Signal] public delegate void EventHandler();

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region INTERNAL TYPES
	//==================================================================================================================

	// public enum Type {
	// 	Value1,
	// }

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region OVERRIDES & VIRTUALS
	//==================================================================================================================

	// public override Godot.Collections.Array<Godot.Collections.Dictionary> _GetPropertyList()
	// 	=> (base._GetPropertyList() ?? [])
	// 		.Concat(
	// 			this.TargetNode?.GetMethodList()
	// 				.Where(method => method["name"].AsString() == this.Method)
	// 				.SelectMany(method => method["args"].AsGodotArray<Godot.Collections.Dictionary>())
	// 				.Through((argument, index) => argument["name"] = $"arguments/{index}")
	// 				?? []
	// 		)
	// 		.ToGodotArrayT();

	// public override Variant _Get(StringName property)
	// 	=> property.ToString().StartsWith("arguments/")
	// 		? this.MethodArguments.ElementAtOrDefault(property.ToString().Substring("arguments/".Length).ToInt())
	// 		: base._Get(property);

	// public override bool _Set(StringName property, Variant value)
	// {
	// 	if (property.ToString().StartsWith("arguments/"))
	// 	{
	// 		int index = property.ToString().Substring("arguments/".Length).ToInt();
	// 		while (this.MethodArguments.Count <= index)
	// 			this.MethodArguments.Add(Variant.NULL);
	// 		this.MethodArguments[index] = value;
	// 		return true;
	// 	}
	// 	return false;
	// }

	public override void _ValidateProperty(Godot.Collections.Dictionary property)
	{
		base._ValidateProperty(property);
		switch (property["name"].AsString())
		{
			// case nameof(this.Target):
			// 	property["usage"] = (long) PropertyUsageFlags.Default
			// 		| (long) PropertyUsageFlags.NodePathFromSceneRoot
			// 		| (long) PropertyUsageFlags.UpdateAllIfModified;
			// 	break;
			// case nameof(this.Method): {
			// 	if (this.TargetNode is not Node context)
			// 	{
			// 		property["usage"] = (long) PropertyUsageFlags.ReadOnly;
			// 		break;
			// 	}
			// 	property["hint"] = (long) PropertyHint.Enum;
			// 	property["hint_string"] = context.GetMethodList()
			// 		.Where(method => method["return"].AsGodotDictionary()["type"].AsVariantType().IsConvertibleTo(this.Type, strict: this.StrictType))
			// 		.Select(dict => dict["name"].AsString())
			// 		.Where(name => !name.StartsWith('_'))
			// 		.JoinIntoString(",");
			// 	property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.UpdateAllIfModified;
			// 	if (!context.HasMethod(this.Method))
			// 		property["error"] = $"The method {this.Method.BBCCode()} does not exist on the target node.";
			// 	break;
			// }
			// case nameof(this.MethodArguments): {
			// 	property["usage"] = (long) PropertyUsageFlags.NoEditor;
			// 	if (this.TargetNode is not Node context || !context.HasMethod(this.Method))
			// 	{
			// 		this.MethodArguments = [];
			// 		break;
			// 	}
			// 	while (this.MethodArguments.Count < context.GetMethodArgumentCount(this.Method))
			// 	{
			// 		Variant value = this.MethodInfo?.Parameters[this.MethodArguments.Count].DefaultValue ?? Variant.NULL;
			// 		if (value.Equals(Variant.NULL) && this.MethodInfo != null)
			// 			value = Variant.GetDefault(this.MethodInfo.Parameters[this.MethodArguments.Count].Type);
			// 		this.MethodArguments.Add(value);
			// 	}
			// 	while (this.MethodArguments.Count > context.GetMethodArgumentCount(this.Method))
			// 		this.MethodArguments.RemoveAt(this.MethodArguments.Count - 1);
			// 	break;
			// }
		}
	}

	protected override Godot.Collections.Dictionary<string, Variant.Type> _GetParameters() => [];
	protected override bool _ReferencesSceneNode() => false;
	protected override Variant.Type _GetReturnType() => Variant.Type.Nil;
	protected override Variant _GetValue(GodotObject self, Dictionary @params)
		=> self.Call(this.Method);

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region METHODS
	//==================================================================================================================

	//==================================================================================================================
	#endregion
	//==================================================================================================================
}
