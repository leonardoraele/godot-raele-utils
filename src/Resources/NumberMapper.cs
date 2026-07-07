using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils;

public partial class NumberMapper : ValueMapper
{
	// -----------------------------------------------------------------------------------------------------------------
	// STATICS
	// -----------------------------------------------------------------------------------------------------------------

	// public static readonly string MyConstant = "";

	// -----------------------------------------------------------------------------------------------------------------
	// EXPORTS
	// -----------------------------------------------------------------------------------------------------------------

	[Export] public bool Absolute = false;

	[ExportGroup("Wrap", "Wrap")]
	[Export(PropertyHint.GroupEnable)] public bool WrapEnabled = false;
	[Export] public float WrapMin = 0;
	[Export] public float WrapMax = 1;

	[ExportGroup("Remap", "Remap")]
	[Export(PropertyHint.GroupEnable)] public bool RemapEnabled = false;
	[Export] public float RemapFromStart = 0;
	[Export] public float RemapFromEnd = 1;
	[Export] public float RemapToStart = 0;
	[Export] public float RemapToEnd = 1;

	// -----------------------------------------------------------------------------------------------------------------
	// FIELDS
	// -----------------------------------------------------------------------------------------------------------------



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

	// public override void _ValidateProperty(Godot.Collections.Dictionary property)
	// {
	// 	base._ValidateProperty(property);
	// 	switch (property["name"].AsString())
	// 	{
	// 		case nameof(this.PropName):
	// 			break;
	// 	}
	// }

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
		if (!value.IsConvertibleTo(Variant.Type.Float))
			return value;
		if (this.Absolute)
			value = Mathf.Abs(value.AsDouble());
		if (this.WrapEnabled)
			value = Mathf.Wrap(value.AsDouble(), this.WrapMin, this.WrapMax);
		if (this.RemapEnabled)
			value = Mathf.Remap(value.AsDouble(), this.RemapFromStart, this.RemapFromEnd, this.RemapToStart, this.RemapToEnd);
		return value;
	}
}
