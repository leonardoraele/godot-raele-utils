using System;
using System.Threading.Tasks;
using Godot;

namespace Raele.GodotUtils.StateMachine;

public partial class StateMachineNode : Node
{
	// -----------------------------------------------------------------------------------------------------------------
	// EXPORTS
	// -----------------------------------------------------------------------------------------------------------------

	[Export] public bool AutoStart = true;
	[Export] public Node? InitialState = null;

	[ExportGroup("Debug")]
	[Export] public bool EnableLogging = false;

	// -----------------------------------------------------------------------------------------------------------------
	// FIELDS
	// -----------------------------------------------------------------------------------------------------------------

	public Node? ActiveState { get; private set; }
	public Node? PreviousState { get; private set; }
	public ulong LastStateTransitionTimestamp { get; private set; } = 0;
	public StateTransition? OngoingTransition { get; private set; }

	// -----------------------------------------------------------------------------------------------------------------
	// SIGNALS
	// -----------------------------------------------------------------------------------------------------------------

	[Signal] public delegate void BeforeStateEnterEventHandler(StateTransition transition);
	[Signal] public delegate void BeforeStateExitEventHandler(StateTransition transition);
	[Signal] public delegate void StartedEventHandler();
	[Signal] public delegate void FinishedEventHandler();

	// -----------------------------------------------------------------------------------------------------------------
	// PROPERTIES
	// -----------------------------------------------------------------------------------------------------------------

	// TODO This should take into consideration game pause/process pause
	public TimeSpan ActiveStateDuration => TimeSpan.FromMilliseconds(Time.GetTicksMsec() - this.LastStateTransitionTimestamp);
	private Node? ResolvedInitialState => this.InitialState ?? this.GetChild(0);

	// -----------------------------------------------------------------------------------------------------------------
	// GODOT EVENTS
	// -----------------------------------------------------------------------------------------------------------------

	public override void _Ready()
	{
		this.DisableChildrenProcessing();
		if (this.AutoStart)
		{
			this.Start();
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		this.ActiveState?.Call("_process_active", delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		this.ActiveState?.Call("_physics_process_active", delta);
	}

	// -----------------------------------------------------------------------------------------------------------------
	// METHODS
	// -----------------------------------------------------------------------------------------------------------------

	private void DisableChildrenProcessing()
	{
		foreach (Node child in this.GetChildren())
		{
			child.ProcessMode = Node.ProcessModeEnum.Disabled;
		}
	}

	public void Start() => this.Reset();

	public async void Reset()
	{
		if (this.ActiveState != null)
		{
			await this.Stop();
		}
		this.QueueTransition(this.InitialState ?? (this.AutoStart ? this.GetChild(0) : null));
	}

	public async Task Stop()
	{
		this.QueueTransition((string?) null);
		TaskCompletionSource source = new();
		this.Finished += source.SetResult;
		await source.Task;
		this.Finished -= source.SetResult;
	}

	/// <summary>
	/// Queues a transition to the specified state. The transition is deferred to the next idle frame time. If another
	/// transition have already been queued, it will be canceled and the new one will be used.
	/// </summary>
	public void QueueTransition(string? newStateName, Variant? data = null)
	{
		this.OngoingTransition?.Cancel();
		this.OngoingTransition = new()
		{
			NextStateName = newStateName,
			PreviousStateName = this.ActiveState?.Name,
			Data = data,
			StateMachine = this,
		};
		this.CallDeferred(MethodName.PerformTransition, this.OngoingTransition);
	}

	public void QueueTransition(Node? newState, Variant? data = null)
		=> this.QueueTransition(newState?.Name, data);

	private void PerformTransition(StateTransition transition)
	{
		if (transition.IsCanceled)
		{
			return;
		}

		// Exit current state
		{
			if (this.ActiveState != null)
			{
				this.EmitSignal(SignalName.BeforeStateExit, transition);
				if (transition.IsCanceled)
				{
					this.DebugLog("🔀", this.ActiveState.Name, "❌ →", transition.NextStateName ?? "null", "(canceled by before_state_exit signal handler)");
					return;
				}
				if (this.ActiveState.HasMethod("_exit_state"))
				{
					try
					{
						this.ActiveState.Call("_exit_state");
					}
					catch (Exception e)
					{
						this.DebugLog("🔀", this.ActiveState.Name, "⚠ →", transition.NextStateName ?? "null", "(exception on exit)", e.Message);
					}
					if (transition.IsCanceled)
					{
						this.DebugLog("🔀", this.ActiveState.Name, "❌ →", transition.NextStateName ?? "null", "(canceled on exit)");
						return;
					}
				}
				this.ActiveState.ProcessMode = Node.ProcessModeEnum.Disabled;
			}

			this.PreviousState = this.ActiveState;
			this.ActiveState = null;
		}

		// Enter next state
		{
			Node? nextState = transition.NextStateName != null
				? this.GetNodeOrNull<Node?>(transition.NextStateName)
				: null;
			if (!string.IsNullOrEmpty(transition.NextStateName) && nextState == null)
			{
				this.DebugLog("🔀", transition.PreviousStateName ?? "null", "❌ →", transition.NextStateName ?? "null", "(state not found)");
				transition.Cancel();
				return;
			}
			this.EmitSignal(SignalName.BeforeStateEnter, transition);
			if (transition.IsCanceled)
			{
				this.DebugLog("🔀", transition.PreviousStateName ?? "null", "→ ❌", transition.NextStateName ?? "null", "(canceled on enter)");
				return;
			}
			if (nextState?.HasMethod("_enter_state") == true)
			{
				try
				{
					nextState.Call("_enter_state");
				}
				catch (Exception e)
				{
					this.DebugLog("🔀", transition.PreviousStateName ?? "null", "⚠ →", transition.NextStateName ?? "null", "(exception on enter)", e.Message);
					transition.Cancel();
				}
			}
			if (transition.IsCanceled)
			{
				this.DebugLog("🔀", transition.PreviousStateName ?? "null", "→ ❌", transition.NextStateName ?? "null", "(canceled on enter)");
				if (this.OngoingTransition == transition)
				{
					this.Reset();
				}
				return;
			}
			this.ActiveState = nextState;
			if (this.ActiveState != null)
			{
				this.ActiveState.ProcessMode = Node.ProcessModeEnum.Inherit;
			}
			this.DebugLog("🔀", transition.PreviousStateName ?? "null", "→", this.ActiveState?.Name ?? "null");
		}

		this.LastStateTransitionTimestamp = Time.GetTicksMsec();
		this.OngoingTransition = null;

		if (this.PreviousState == null && this.ActiveState != null)
		{
			this.EmitSignal(SignalName.Started);
		}
		else if (this.PreviousState != null && this.ActiveState == null)
		{
			this.EmitSignal(SignalName.Finished);
		}
	}

	private void DebugLog(params string[] args)
	{
		if (this.EnableLogging)
		{
			GD.PrintS(Time.GetTicksMsec(), nameof(StateMachineNode), this.GetParent().Name, ":", string.Join(" ", args));
		}
	}
}
