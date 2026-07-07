using System.Linq;
using Godot;
using Raele.GodotUtils.Adapters;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils;

[Tool][GlobalClass]
public partial class ExpressionMapper : ValueMapper
{
	// -----------------------------------------------------------------------------------------------------------------
	// STATICS
	// -----------------------------------------------------------------------------------------------------------------

	// public static readonly string MyConstant = "";

	// -----------------------------------------------------------------------------------------------------------------
	// EXPORTS
	// -----------------------------------------------------------------------------------------------------------------

	[Export] public NodePath? Context;
	[Export(PropertyHint.Expression)] public string Expression = "";

	[ExportGroup("Options")]
	[Export] public Variant.Type InputTypeCheck = Variant.Type.Nil;
	[Export] public Variant.Type OutputTypeCheck = Variant.Type.Nil;
	[Export] public Godot.Collections.Dictionary Variables = [];

	// -----------------------------------------------------------------------------------------------------------------
	// FIELDS
	// -----------------------------------------------------------------------------------------------------------------

	private Expression? Interpreter;

	// -----------------------------------------------------------------------------------------------------------------
	// PROPERTIES
	// -----------------------------------------------------------------------------------------------------------------



	// -----------------------------------------------------------------------------------------------------------------
	// SIGNALS
	// -----------------------------------------------------------------------------------------------------------------

	// [Signal] public delegate void EventHandler()

	// -----------------------------------------------------------------------------------------------------------------
	// INTERNAL TYPES
	// -----------------------------------------------------------------------------------------------------------------

	// public enum ConvertionEnum {
	// 	None,
	// 	DegreesToRadians,
	// 	RadiansToDegrees,
	// 	SecondsToMilliseconds,
	// 	MillisecondsToSeconds,
	// }

	// -----------------------------------------------------------------------------------------------------------------
	// GODOT EVENTS
	// -----------------------------------------------------------------------------------------------------------------

	public override void _ValidateProperty(Godot.Collections.Dictionary property)
	{
		base._ValidateProperty(property);
		switch (property["name"].AsString())
		{
			case nameof(this.Context):
				if (!this.ResourceLocalToScene)
					property["usage"] = PropertyUsageFlags.ReadOnly.Union(PropertyUsageFlags.Editor).As<long>();
				break;
			case nameof(this.Variables):
				GodotPropertyInfo.Set(
					property,
					type: Variant.Type.Dictionary,
					hint: PropertyHint.DictionaryType,
					hintString: "String;Nil"
				);
				break;
		}
	}

	// public override string[] _GetConfigurationWarnings()
	// 	=> base._PhysicsProcess(delta);

	// public override void _EnterTree()
	// {
	// 	base._EnterTree();
	// }

	// public override void _ExitTree()
	// {
	// 	base._ExitTree();
	// }

	// public override void _Ready()
	// {
	// 	base._Ready();
	// }

	// public override void _Process(double delta)
	// {
	// 	base._Process(delta);
	// }

	// public override void _PhysicsProcess(double delta)
	// {
	// 	base._PhysicsProcess(delta);
	// }

	// -----------------------------------------------------------------------------------------------------------------
	// METHODS
	// -----------------------------------------------------------------------------------------------------------------

	public override Variant MapValue(Variant value)
	{
		if (this.InputTypeCheck != Variant.Type.Nil && value.VariantType.IsConvertibleTo(this.InputTypeCheck) == false)
		{
			GD.PushError($"ExpressionMapper: Input value type {value.VariantType} is not convertible to expected type {this.InputTypeCheck}");
			return Variant.GetDefault(this.OutputTypeCheck);
		}
		if (this.Interpreter == null || Engine.IsEditorHint())
		{
			this.Interpreter = new();
			this.Interpreter.Parse(this.Expression, this.Variables.Keys.Select(key => key.AsString()).ToArray());
		}
		Node? context = this.Context != null && this.ResourceLocalToScene
			? this.GetLocalScene().GetNode(this.Context)
			: null;
		Variant result = this.Interpreter.Execute(this.Variables.Values.ToGodotArray(), context);
		if (this.Interpreter.HasExecuteFailed())
		{
			GD.PushError($"Failed to execute expression. Cause: Execution errored. Error: {this.Interpreter.GetErrorText()}");
			return Variant.GetDefault(this.OutputTypeCheck);
		}
		if (this.OutputTypeCheck != Variant.Type.Nil && !result.VariantType.IsConvertibleTo(this.OutputTypeCheck))
		{
			GD.PushError($"Failed to execute expression. Cause: Expression result type mismatch. Expected type: {this.OutputTypeCheck}. Result: {result} (type '{result.VariantType}').");
			return Variant.GetDefault(this.OutputTypeCheck);
		}
		return result;
	}
}
