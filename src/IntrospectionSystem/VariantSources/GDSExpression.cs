using System.Collections.Generic;
using System.Linq;
using Godot;
using Raele.GodotUtils.Adapters;
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
	[Export] public VariantSource? Context;
	[Export] public Godot.Collections.Dictionary<string, VariantSource?> Parameters
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

	// public override void _ValidateProperty(Godot.Collections.Dictionary property)
	// {
	// 	base._ValidateProperty(property);
	// 	switch (property["name"].AsString())
	// 	{
	// 		case nameof(this.Context):
	// 			property["usage"] = (long) PropertyUsageFlags.Default
	// 				| (long) PropertyUsageFlags.UpdateAllIfModified
	// 				| (long) PropertyUsageFlags.NodePathFromSceneRoot;
	// 			break;
	// 		case nameof(this.Param):
	// 			property["usage"] = (long) PropertyUsageFlags.Default | (long) PropertyUsageFlags.NilIsVariant;
	// 			break;
	// 	}
	// }

	protected override Variant.Type _GetReturnType()
		=> this.ExpectedType;
	protected override IEnumerable<GodotPropertyInfo> _GetAdditionalParameters()
		=> (this.Context?.GetAdditionalParameters() ?? [])
			.Concat(this.Parameters.Values.WhereNotNull().SelectMany(@param => @param.GetAdditionalParameters()));
	protected override bool _ReferencesSceneNode()
		=> this.Context?.ReferencesSceneNode() == true
			|| this.Parameters.Values.Any(source => source?.ReferencesSceneNode() == true);
	protected override Variant _GetValue(Dictionary<string, Variant> @params)
	{
		Variant value = this.Interpreter.Execute(
			this.Parameters.Values.Select(source => source?.GetValue(@params) ?? Variant.NULL).ToGodotArray(),
			this.GetLocalScene().GetNode(this.Context?.GetValue<NodePath>(@params))
		);
		if (this.Interpreter.HasExecuteFailed())
			return Variant.GetDefault(this.ExpectedType);
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
