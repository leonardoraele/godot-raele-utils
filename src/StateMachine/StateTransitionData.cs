using Godot;

namespace Raele.GodotUtils.StateMachine;

public partial class StateTransitionData : GodotObject
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
	public ulong StateMachineId { get; init; }
	public bool IsCanceled { get; private set; } = false;

	// -----------------------------------------------------------------------------------------------------------------
	// PROPERTIES
	// -----------------------------------------------------------------------------------------------------------------

	private StateMachine? StateMachine => (StateMachine?) GodotObject.InstanceFromId(this.StateMachineId);
	public Node? PreviousState => this.StateMachine?.GetNodeOrNull(this.PreviousStateName);
	public Node? NextState => this.StateMachine?.GetNodeOrNull(this.NextStateName);

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
