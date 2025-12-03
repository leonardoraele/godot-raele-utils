using System;
using Godot;

namespace Raele.GodotUtils.StateMachine;

public abstract partial class BaseState : Node
{
	// -----------------------------------------------------------------------------------------------------------------
	// EXPORTS
	// -----------------------------------------------------------------------------------------------------------------

	[Export] public ProcessModeEnum ProcessModeWhileInactive = Node.ProcessModeEnum.Disabled;

	// -----------------------------------------------------------------------------------------------------------------
	// PROPERTIES
	// -----------------------------------------------------------------------------------------------------------------

	public StateMachineNode StateMachine => this.RequireAncestor<StateMachineNode>();
	public bool IsActive => this.StateMachine.ActiveState == this;
	public bool IsPreviousActiveState => this.StateMachine.PreviousState == this;
	public TimeSpan ActiveDuration => this.IsActive ? this.StateMachine.ActiveStateDuration : TimeSpan.Zero;

	// -----------------------------------------------------------------------------------------------------------------
	// SIGNALS
	// -----------------------------------------------------------------------------------------------------------------

	[Signal] public delegate void StateEnteredEventHandler();
	[Signal] public delegate void StateExitedEventHandler();

	// -----------------------------------------------------------------------------------------------------------------
	// METHODS
	// -----------------------------------------------------------------------------------------------------------------

	public void _enter_state()
	{
		this.ProcessMode = ProcessModeEnum.Inherit;
		try
		{
			this._EnterState();
		}
		finally
		{
			this.EmitSignal(SignalName.StateEntered);
		}
	}

	public void _exit_state()
	{
		this.ProcessMode = this.ProcessModeWhileInactive;
		try
		{
			this._ExitState();
		}
		finally
		{
			this.EmitSignal(SignalName.StateExited);
		}
	}

	public void _process_active(double delta) => this._ProcessActive(delta);
	public void _physics_process_active(double delta) => this._PhysicsProcessActive(delta);

	// -----------------------------------------------------------------------------------------------------------------
	// VIRTUAL METHODS
	// -----------------------------------------------------------------------------------------------------------------

	public virtual void _EnterState() { }
	public virtual void _ExitState() { }
	public virtual void _ProcessActive(double delta) { }
	public virtual void _PhysicsProcessActive(double delta) { }
}
