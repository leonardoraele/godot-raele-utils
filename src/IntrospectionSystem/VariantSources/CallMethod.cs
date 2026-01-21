using System.Collections.Generic;
using System.Linq;
using Godot;
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
	[Export] public VariantSource? Target;
	[Export(PropertyHint.TypeString)] public string TypeName = "";
	[Export] public VariantSource? Method;
	[Export] public VariantSource?[] Arguments = [];

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

	// public override void _ValidateProperty(Godot.Collections.Dictionary property)
	// {
	// 	base._ValidateProperty(property);
	// 	switch (property["name"].AsString())
	// 	{
	// 		case nameof(this.Target):
	// 			property["usage"] = (long) PropertyUsageFlags.Default
	// 				| (long) PropertyUsageFlags.NodePathFromSceneRoot
	// 				| (long) PropertyUsageFlags.UpdateAllIfModified;
	// 			break;
	// 		case nameof(this.Method): {
	// 			if (this.TargetNode is not Node context)
	// 			{
	// 				property["usage"] = (long) PropertyUsageFlags.ReadOnly;
	// 				break;
	// 			}
	// 			property["hint"] = (long) PropertyHint.Enum;
	// 			property["hint_string"] = context.GetMethodList()
	// 				.Where(method => method["return"].AsGodotDictionary()["type"].AsVariantType().IsConvertibleTo(this.Type, strict: this.StrictType))
	// 				.Select(dict => dict["name"].AsString())
	// 				.Where(name => !name.StartsWith('_'))
	// 				.JoinIntoString(",");
	// 			property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.UpdateAllIfModified;
	// 			if (!context.HasMethod(this.Method))
	// 				property["error"] = $"The method {this.Method.BBCCode()} does not exist on the target node.";
	// 			break;
	// 		}
	// 		case nameof(this.MethodArguments): {
	// 			property["usage"] = (long) PropertyUsageFlags.NoEditor;
	// 			if (this.TargetNode is not Node context || !context.HasMethod(this.Method))
	// 			{
	// 				this.MethodArguments = [];
	// 				break;
	// 			}
	// 			while (this.MethodArguments.Count < context.GetMethodArgumentCount(this.Method))
	// 			{
	// 				Variant value = this.MethodInfo?.Parameters[this.MethodArguments.Count].DefaultValue ?? Variant.NULL;
	// 				if (value.Equals(Variant.NULL) && this.MethodInfo != null)
	// 					value = Variant.GetDefault(this.MethodInfo.Parameters[this.MethodArguments.Count].Type);
	// 				this.MethodArguments.Add(value);
	// 			}
	// 			while (this.MethodArguments.Count > context.GetMethodArgumentCount(this.Method))
	// 				this.MethodArguments.RemoveAt(this.MethodArguments.Count - 1);
	// 			break;
	// 		}
	// 	}
	// }

	protected override IEnumerable<GodotPropertyInfo> _GetAdditionalParameters()
		=> (this.Target?.GetAdditionalParameters() ?? [])
			.Concat(this.Method?.GetAdditionalParameters() ?? [])
			.Concat(this.Arguments.SelectMany(arg => arg?.GetAdditionalParameters() ?? []));
	protected override bool _ReferencesSceneNode()
		=> this.Target?.ReferencesSceneNode() == true
			|| this.Method?.ReferencesSceneNode() == true
			|| this.Arguments.Any(arg => arg?.ReferencesSceneNode() == true);
	protected override Variant.Type _GetReturnType()
		=> Variant.Type.Nil;
	protected override Variant _GetValue(Dictionary<string, Variant> @params)
		=> this.Method?.GetValue<string>(@params) is string method
			&& !method.IsWhiteSpace()
			&& this.Target?.GetValue<GodotObject>(@params) is GodotObject target
			&& target.HasMethod(method)
				? target.Callv(
						method,
						this.Arguments.Select(source => source?.GetValue(@params) ?? Variant.NULL).ToGodotArray()
					)
					.As(this.Type)
				: Variant.NULL;

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region METHODS
	//==================================================================================================================

	//==================================================================================================================
	#endregion
	//==================================================================================================================
}
