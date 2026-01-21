using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Raele.GodotUtils.Adapters;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.IntrospectionSystem.VariantSources;

[Tool][GlobalClass]
public partial class GetProperty : VariantSource
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

	[ExportCategory(nameof(GetProperty))]
	[Export] public VariantSource? Target;
	[Export] public VariantSource? Property;

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

	// public Node? PropertyOwnerNode
	// 	=> this.GetLocalScene()?.GetNodeOrNull(this.PropertyOwner);

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

	// public override string[] _GetConfigurationWarnings()
	// 	=> (base._GetConfigurationWarnings() ?? [])
	// 		.Concat(
	// 			false
	// 				? ["This node is not configured correctly. Did you forget to assign a required field?"]
	// 				: []
	// 		)
	// 		.ToArray();

	// public override void _ValidateProperty(Godot.Collections.Dictionary property)
	// {
	// 	base._ValidateProperty(property);
	// 	switch (property["name"].AsString())
	// 	{
	// 		// case nameof(this.PropertyOwner):
	// 		// 	property["usage"] = (long) PropertyUsageFlags.Default
	// 		// 		| (long) PropertyUsageFlags.NodePathFromSceneRoot
	// 		// 		| (long) PropertyUsageFlags.UpdateAllIfModified;
	// 		// 	break;
	// 		// case nameof(this.Property):
	// 		// 	if (this.PropertyOwnerNode is not Node subject)
	// 		// 		break;
	// 		// 	string[] options = subject.GetPropertyList()
	// 		// 		.Where(property => property["type"].AsVariantType().IsConvertibleTo(this.Type, strict: this.StrictType))
	// 		// 		.Select(dict => dict["name"].AsString())
	// 		// 		.Where(name => name.Split('/').All(part => !part.StartsWith('_')))
	// 		// 		.ToArray();
	// 		// 	options.Sort();
	// 		// 	property["hint"] = (long) PropertyHint.EnumSuggestion;
	// 		// 	property["hint_string"] = options.Join(",");
	// 		// 	property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.UpdateAllIfModified;
	// 		// 	if (this.Property.IsWhiteSpace())
	// 		// 		break;
	// 		// 	Variant value = this._GetValue();
	// 		// 	if (value.VariantType.IsConvertibleTo(this.Type, strict: this.StrictType))
	// 		// 	{
	// 		// 		if (this.Type != Variant.Type.Nil)
	// 		// 			value = value.As(this.Type);
	// 		// 		property["comment"] = $"Current value: {Json.Stringify(value).BBCCode()} ({value.VariantType}).";
	// 		// 	}
	// 		// 	else
	// 		// 		property["error"] = "The selected property does not exist on the context node or does not match the expected type.";
	// 		// 	break;
	// 	}
	// }

	protected override Variant.Type _GetReturnType()
		=> Variant.Type.Nil;
	protected override IEnumerable<GodotPropertyInfo> _GetAdditionalParameters()
		=> (this.Target?.GetAdditionalParameters() ?? [])
			.Concat(this.Property?.GetAdditionalParameters() ?? []);
	protected override bool _ReferencesSceneNode()
		=> this.Target?.ReferencesSceneNode() == true
			|| this.Property?.ReferencesSceneNode() == true;
	protected override Variant _GetValue(Dictionary<string, Variant> @params)
		=> this.Property?.GetValue<string>(@params) is string property
			&& !property.IsWhiteSpace()
			&& this.Target?.GetValue<GodotObject>(@params) is GodotObject target
				? target.Get(property)
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
