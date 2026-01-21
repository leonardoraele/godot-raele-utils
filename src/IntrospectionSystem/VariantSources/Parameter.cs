using System.Collections.Generic;
using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.IntrospectionSystem.VariantSources;

public partial class Parameter : VariantSource
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

	[Export] public string Name
		{ get; set { field = value; this.CallDebounced(2d, GodotObject.MethodName.NotifyPropertyListChanged); } }
		= "";
	[Export] public Variant DefaultValue;

	[ExportGroup("Type Checking")]
	[Export] public Variant.Type ExpectedType = Variant.Type.Nil;
	// [Export] public PropertyHint Hint = PropertyHint.None;
	// [Export] public string HintString
	// 	{ get; set { field = value; this.CallDebounced(2d, GodotObject.MethodName.NotifyPropertyListChanged); } }
	// 	= "";

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

	// public override string[] _GetConfigurationWarnings()
	// 	=> (base._GetConfigurationWarnings() ?? [])
	// 		.Concat(
	// 			false
	// 				? ["This node is not configured correctly. Did you forget to assign a required field?"]
	// 				: []
	// 		)
	// 		.ToArray();

	public override void _ValidateProperty(Godot.Collections.Dictionary property)
	{
		base._ValidateProperty(property);
		switch (property["name"].AsString())
		{
			case nameof(this.Name):
				if (this.Name.IsWhiteSpace())
					property["warning"] = "Parameter name cannot be empty or whitespace.";
				break;
			case nameof(this.Type):
			// case nameof(this.Hint):
				property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.UpdateAllIfModified;
				break;
			case nameof(this.DefaultValue):
				property["type"] = (long) this.Type;
				property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.NilIsVariant;
				break;
		}
	}

	protected override Godot.Collections.Dictionary<string, Variant.Type> _GetParameters()
		=> new() { { this.Name, this.ExpectedType } };
	protected override bool _ReferencesSceneNode() => false;
	protected override Variant.Type _GetReturnType() => this.ExpectedType;
	protected override Variant _GetValue(GodotObject self, Godot.Collections.Dictionary @params)
	{
		Variant value = @params.GetValueOrDefault(this.Name, this.DefaultValue);
		return this.ExpectedType != Variant.Type.Nil
			? value.As(this.ExpectedType)
			: value;
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region METHODS
	//==================================================================================================================

	//==================================================================================================================
		#endregion
	//==================================================================================================================
}
