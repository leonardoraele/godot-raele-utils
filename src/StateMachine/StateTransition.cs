using Godot;

namespace Raele.GodotUtils.StateMachine;

public partial class StateTransition : GodotObject
{
	// -----------------------------------------------------------------------------------------------------------------
	// STATICS
	// -----------------------------------------------------------------------------------------------------------------

	// public static readonly string MyConstant = "";

	// -----------------------------------------------------------------------------------------------------------------
	// FIELDS
	// -----------------------------------------------------------------------------------------------------------------

	public string? PreviousStateName { get; init; }
	public string? NextStateName { get; init; }
	public Variant? Data { get; init; }
	public required StateMachine StateMachine { get; init; }
	public bool IsCanceled { get; private set; } = false;

	// -----------------------------------------------------------------------------------------------------------------
	// PROPERTIES
	// -----------------------------------------------------------------------------------------------------------------

	public Node? PreviousState => !string.IsNullOrEmpty(this.PreviousStateName)
		? this.StateMachine.GetNodeOrNull(this.PreviousStateName)
		: null;
	public Node? NextState => !string.IsNullOrEmpty(this.NextStateName)
		? this.StateMachine.GetNodeOrNull(this.NextStateName)
		: null;

	// -----------------------------------------------------------------------------------------------------------------
	// SIGNALS
	// -----------------------------------------------------------------------------------------------------------------

	[Signal] public delegate void CanceledEventHandler();

	// -----------------------------------------------------------------------------------------------------------------
	// METHODS
	// -----------------------------------------------------------------------------------------------------------------

	public void Cancel()
	{
		this.IsCanceled = true;
		this.EmitSignal(SignalName.Canceled);
	}
}
