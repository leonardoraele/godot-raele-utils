using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Raele.GodotUtils.Adapters;

namespace Raele.GodotUtils.IntrospectionSystem.VariantSources;

[Tool][GlobalClass]
public partial class MathOperation : VariantSource
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

	[ExportCategory(nameof(MathOperation))]
	[Export] public VariantSource? LHS;
	[Export] public OperationEnum Operation = OperationEnum.Add;
	[Export] public VariantSource? RHS;

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

	private double LeftAsDouble => this.LHS?.GetValue<double>() ?? 0d;
	private double RightAsDouble => this.RHS?.GetValue<double>() ?? 0d;
	private long LeftAsLong => this.LHS?.GetValue<long>() ?? 0L;
	private long RightAsLong => this.RHS?.GetValue<long>() ?? 0L;

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

	public enum OperationEnum : short {
		Add = 100,
		Subtract = 200,
		Multiply = 300,
		Divide = 400,
		Modulo = 500,
		Power = 600,
		Root = 700,
		Min = 800,
		Max = 900,
		Average = 1000,
		Absolute = 1100,
		BitShiftLeft = 1200,
		BitshiftRight = 1300,
		BitwiseAnd = 1400,
		BitwiseOr = 1500,
		BitwiseXor = 1600,
	}

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region OVERRIDES & VIRTUALS
	//==================================================================================================================

	// public override void _ValidateProperty(Godot.Collections.Dictionary property)
	// {
	// 	base._ValidateProperty(property);
	// 	this.SetStrongType(Variant.Type.Float);
	// 	// switch (property["name"].AsString())
	// 	// {
	// 	// 	case nameof():
	// 	// 		break;
	// 	// }
	// }

	protected override Variant.Type _GetReturnType()
		=> Variant.Type.Float;
	protected override IEnumerable<GodotPropertyInfo> _GetAdditionalParameters()
		=> (this.LHS?.GetAdditionalParameters() ?? [])
			.Concat(this.RHS?.GetAdditionalParameters() ?? []);
	protected override bool _ReferencesSceneNode()
		=> this.LHS?.ReferencesSceneNode() == true
			|| this.RHS?.ReferencesSceneNode() == true;
	protected override Variant _GetValue(Dictionary<string, Variant> @params)
		=> this.Operation switch
		{
			OperationEnum.Add => this.LeftAsDouble + this.RightAsDouble,
			OperationEnum.Subtract => this.LeftAsDouble - this.RightAsDouble,
			OperationEnum.Multiply => this.LeftAsDouble * this.RightAsDouble,
			OperationEnum.Divide => this.LeftAsDouble / this.RightAsDouble,
			OperationEnum.Modulo => this.LeftAsDouble % this.RightAsDouble,
			OperationEnum.Power => Mathf.Pow(this.LeftAsDouble, this.RightAsDouble),
			OperationEnum.Root => Mathf.Pow(this.LeftAsDouble, 1d / this.RightAsDouble),
			OperationEnum.Min => Mathf.Min(this.LeftAsDouble, this.RightAsDouble),
			OperationEnum.Max => Mathf.Max(this.LeftAsDouble, this.RightAsDouble),
			OperationEnum.Average => (this.LeftAsDouble + this.RightAsDouble) / 2d,
			OperationEnum.Absolute => Mathf.Abs(this.LeftAsDouble),
			OperationEnum.BitShiftLeft => this.LeftAsLong << (int) this.RightAsLong,
			OperationEnum.BitshiftRight => this.LeftAsLong >> (int) this.RightAsLong,
			OperationEnum.BitwiseAnd => this.LeftAsLong & this.RightAsLong,
			OperationEnum.BitwiseOr => this.LeftAsLong | this.RightAsLong,
			OperationEnum.BitwiseXor => this.LeftAsLong ^ this.RightAsLong,
			_ => throw new Exception($"Unsupported operation {this.Operation}."),
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
