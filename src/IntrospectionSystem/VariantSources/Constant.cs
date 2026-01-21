using System;
using System.Collections.Generic;
using Godot;
using Raele.GodotUtils.Adapters;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.IntrospectionSystem.VariantSources;

[Tool][GlobalClass]
public partial class Constant : VariantSource
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

	[ExportCategory(nameof(Constant))]
	[Export] public Variant Value;

	[ExportGroup("Options")]
	[Export] public PropertyHint TypeHint = PropertyHint.None;

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

	public override void _ValidateProperty(Godot.Collections.Dictionary property)
	{
		base._ValidateProperty(property);
		switch (property["name"].AsString())
		{
			case nameof(this.TypeHint):
				property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.UpdateAllIfModified;
				break;
			case nameof(this.Value):
				property["hint"] = (long) this.TypeHint;
				property["usage"] = (long) PropertyUsageFlags.Default
					| (long) PropertyUsageFlags.NodePathFromSceneRoot
					| (long) PropertyUsageFlags.NilIsVariant;
				break;
		}
	}

	protected override IEnumerable<GodotPropertyInfo> _GetAdditionalParameters()
		=> [];
	protected override Variant _GetValue(Dictionary<string, Variant> @params)
		=> this.Value;
	protected override bool _ReferencesSceneNode()
		=> this.Value.VariantType == Variant.Type.NodePath && !this.Value.AsNodePath().ToString().IsWhiteSpace();
	protected override Variant.Type _GetReturnType() => this.Value.VariantType;

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region METHODS
	//==================================================================================================================

	//==================================================================================================================
	#endregion
	//==================================================================================================================
}
