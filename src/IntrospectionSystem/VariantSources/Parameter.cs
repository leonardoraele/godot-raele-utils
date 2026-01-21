using System.Collections.Generic;
using Godot;
using Raele.GodotUtils.Adapters;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.IntrospectionSystem.VariantSources;

[Tool][GlobalClass]
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
	[Export] public new Variant.Type Type;
	[Export] public PropertyHint Hint;
	[Export] public string HintString
		{ get; set { field = value; this.CallDebounced(2d, GodotObject.MethodName.NotifyPropertyListChanged); } }
		= "";
	[Export] public Variant DefaultValue;

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
			case nameof(this.Hint):
				property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.UpdateAllIfModified;
				break;
			case nameof(this.DefaultValue):
				property["type"] = (long) this.Type;
				property["hint"] = (long) this.Hint;
				property["hint_string"] = this.HintString;
				property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.NilIsVariant;
				break;
		}
	}

	protected override Variant.Type _GetReturnType() => this.Type;
	protected override IEnumerable<GodotPropertyInfo> _GetAdditionalParameters()
		=> [
			new()
			{
				Name = this.Name,
				Type = this.Type,
				Hint = this.Hint,
				HintString = this.HintString,
				DefaultValue = this.DefaultValue,
			}
		];
	protected override bool _ReferencesSceneNode()
		=> this.DefaultValue.VariantType == Variant.Type.NodePath && !this.DefaultValue.IsEmpty();
	protected override Variant _GetValue(Dictionary<string, Variant> @params)
		=> @params.GetValueOrDefault(this.Name, this.DefaultValue);

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region METHODS
	//==================================================================================================================

	//==================================================================================================================
		#endregion
	//==================================================================================================================
}
