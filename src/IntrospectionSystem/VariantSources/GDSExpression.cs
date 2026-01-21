using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.IntrospectionSystem.VariantSources;

[Tool][GlobalClass]
public partial class GDSExpression : VariantSource
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

	[ExportCategory(nameof(GDSExpression))]
	[Export] public Godot.Collections.Dictionary<string, VariantSource> Parameters
		{ get; set { field = value; this.Interpreter = null!; } }
		= [];
	[Export(PropertyHint.Expression)] public string Expression
		{ get; set { field = value; this.Interpreter = null!; } }
		= "";

	[ExportGroup("Type Checking")]
	[Export] public Variant.Type ExpectedType = Variant.Type.Nil;

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region FIELDS
	//==================================================================================================================

	private Expression Interpreter
	{
		get
		{
			if (field == null)
			{
				field = new();
				field.Parse(this.Expression, this.Parameters.Keys.ToArray());
			}
			return field;
		}
		set;
	}

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region COMPUTED PROPERTIES
	//==================================================================================================================

	// private Node? ContextNode
	// 	=> this.GetLocalScene()?.GetNodeOrNull(this.Context);

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
			// case nameof(this.Context):
			// 	property["usage"] = (long) PropertyUsageFlags.Default
			// 		| (long) PropertyUsageFlags.UpdateAllIfModified
			// 		| (long) PropertyUsageFlags.NodePathFromSceneRoot;
			// 	break;
			// case nameof(this.Param):
			// 	property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.NilIsVariant;
			// 	break;
		}
	}

	protected override Godot.Collections.Dictionary<string, Variant.Type> _GetParameters()
		=> this.Parameters.Values.Select(source => source.GetRequiredParamters())
			.Aggregate(new Godot.Collections.Dictionary<string, Variant.Type>(), (result, @params) =>
			{
				result.Merge(@params);
				return result;
			});
	protected override Variant _GetValue(GodotObject self, Dictionary @params)
	{
		Variant value = this.Interpreter.Execute(this.Parameters.Values.Select(source => source.GetValue(self, @params)).ToGodotArray(), self);
		if (this.Interpreter.HasExecuteFailed())
			return Variant.GetDefault(this.ExpectedType);
		return this.ExpectedType != Variant.Type.Nil ? value.As(this.ExpectedType) : value;
	}
	protected override bool _ReferencesSceneNode()
		=> this.Parameters.Values.Any(source => source.ReferencesSceneNode());
	protected override Variant.Type _GetReturnType()
		=> this.ExpectedType;

	//==================================================================================================================
	#endregion
	//==================================================================================================================
	#region METHODS
	//==================================================================================================================

	//==================================================================================================================
	#endregion
	//==================================================================================================================
}
