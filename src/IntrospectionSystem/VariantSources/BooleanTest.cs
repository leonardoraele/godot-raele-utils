using System.Collections.Generic;
using System.Linq;
using Godot;
using Raele.GodotUtils.Adapters;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.IntrospectionSystem.VariantSources;

[Tool][GlobalClass]
public partial class BooleanTest : VariantSource
{
	//==================================================================================================================
	#region STATICS
	//==================================================================================================================

	private static readonly ComparisonOperator[] OPERATORS_REQUIRING_ARGUMENT = [
		ComparisonOperator.Equals,
		ComparisonOperator.NotEquals,
		ComparisonOperator.IsLessThan,
		ComparisonOperator.IsLessOrEqualTo,
		ComparisonOperator.IsGreaterThan,
		ComparisonOperator.IsGreaterOrEqualTo,
		ComparisonOperator.HasAnyBitFlag,
		ComparisonOperator.HasAllBitFlags,
		ComparisonOperator.HasNoneBitFlags,
	];

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region EXPORTS
	//==================================================================================================================

	[ExportCategory(nameof(BooleanTest))]
	[Export] public VariantSource? Subject;
	[Export] public ComparisonOperator Operator = ComparisonOperator.IsTruthy;
	[Export] public VariantSource? Argument;

	[Export] public bool Not = false;

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

	private bool RequiresArgument => OPERATORS_REQUIRING_ARGUMENT.Contains(this.Operator);

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

	public enum ComparisonOperator : short {
		Equals = 100,
		NotEquals = 200,
		StrictIsTrue = 300,
		StrictIsFalse = 400,
		IsTruthy = 500,
		IsFalsy = 600,
		IsLessThan = 700,
		IsLessOrEqualTo = 800,
		IsGreaterThan = 900,
		IsGreaterOrEqualTo = 1000,
		HasAnyBitFlag = 1100,
		HasAllBitFlags = 1200,
		HasNoneBitFlags = 1300,
	}

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
			case nameof(this.Operator):
				property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.UpdateAllIfModified;
				break;
			case nameof(this.Argument):
				if (!this.RequiresArgument)
					property["usage"] = (long) PropertyUsageFlags.None;
				break;
		}
	}

	protected override Variant.Type _GetReturnType() => Variant.Type.Bool;
	protected override IEnumerable<GodotPropertyInfo> _GetAdditionalParameters()
		=> (this.Subject?.GetAdditionalParameters() ?? [])
			.Concat(this.RequiresArgument && this.Argument != null ? this.Argument.GetAdditionalParameters() : []);
	protected override bool _ReferencesSceneNode()
		=> this.Subject?.ReferencesSceneNode() == true
			|| this.RequiresArgument && this.Argument?.ReferencesSceneNode() == true;
	protected override Variant _GetValue(Dictionary<string, Variant> args)
		=> this.Not != this.Operator switch
		{
			ComparisonOperator.Equals => (this.Subject?.GetValue(args) ?? Variant.NULL)
				.Equals(this.Argument?.GetValue(args) ?? Variant.NULL),
			ComparisonOperator.NotEquals => !(this.Subject?.GetValue(args) ?? Variant.NULL)
				.Equals(this.Argument?.GetValue(args) ?? Variant.NULL),
			ComparisonOperator.StrictIsTrue => this.Subject?.GetValue(args) is Variant value
				&& value.VariantType == Variant.Type.Bool
				&& value.AsBool(),
			ComparisonOperator.StrictIsFalse => this.Subject?.GetValue(args) is Variant value
				&& value.VariantType == Variant.Type.Bool
				&& value.AsBool(),
			ComparisonOperator.IsTruthy => this.Subject?.GetValue(args).IsEmpty() == false,
			ComparisonOperator.IsFalsy => this.Subject?.GetValue(args).IsEmpty() != false,
			ComparisonOperator.IsLessThan => this.Subject?.GetValue<double>(args) is double lhs
				&& this.Argument?.GetValue<double>(args) is double rhs
				&& 	lhs < rhs,
			ComparisonOperator.IsLessOrEqualTo =>this.Subject?.GetValue<double>(args) is double lhs
				&& this.Argument?.GetValue<double>(args) is double rhs
				&& lhs <= rhs,
			ComparisonOperator.IsGreaterThan => this.Subject?.GetValue<double>(args) is double lhs
				&& this.Argument?.GetValue<double>(args) is double rhs
				&& lhs > rhs,
			ComparisonOperator.IsGreaterOrEqualTo => this.Subject?.GetValue<double>(args) is double lhs
				&& this.Argument?.GetValue<double>(args) is double rhs
				&& lhs >= rhs,
			ComparisonOperator.HasAnyBitFlag => this.Subject?.GetValue<long>(args) is long mask
				&& this.Argument?.GetValue<long>(args) is long flags
				&& (mask & flags) != 0,
			ComparisonOperator.HasAllBitFlags => this.Subject?.GetValue<long>(args) is long mask
				&& this.Argument?.GetValue<long>(args) is long flags
				&& (mask & flags) == flags,
			ComparisonOperator.HasNoneBitFlags =>this.Subject?.GetValue<long>(args) is long mask
				&& this.Argument?.GetValue<long>(args) is long flags
				&& (mask & flags) == 0,
			_ => false,
		};

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region METHODS
	//==================================================================================================================

	//==================================================================================================================
	#endregion
	//==================================================================================================================
}
