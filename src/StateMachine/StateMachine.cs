using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;

namespace Raele.GodotUtils.StateMachine;

public partial class StateMachine : Node
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
	public StateTransitionData? OngoingTransition { get; private set; }

	// -----------------------------------------------------------------------------------------------------------------
	// SIGNALS
	// -----------------------------------------------------------------------------------------------------------------

	[Signal] public delegate void BeforeStateEnterEventHandler(StateTransitionData transition);
	[Signal] public delegate void BeforeStateExitEventHandler(StateTransitionData transition);
	[Signal] public delegate void StartedEventHandler();
	[Signal] public delegate void FinishedEventHandler();

	// -----------------------------------------------------------------------------------------------------------------
	// PROPERTIES
	// -----------------------------------------------------------------------------------------------------------------

	// TODO This should take into consideration game pause/process pause
	public ulong TimeSinceLastStateTransitionMs => Time.GetTicksMsec() - this.LastStateTransitionTimestamp;
	public float TimeSinceLastStateTransitionSec => this.TimeSinceLastStateTransitionMs / 1000f;
	private string? InitialStateName => this.InitialState?.Name ?? this.GetChildren().FirstOrDefault()?.Name;

	// -----------------------------------------------------------------------------------------------------------------
	// LIFECYCLE HANDLERS
	// -----------------------------------------------------------------------------------------------------------------

	public override void _Ready()
	{
		this.DisableChildrenProcessing();
		if (this.AutoStart)
		{
			this.Start();
		}
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
		string? initialStateName = this.InitialStateName ?? this.GetChild(0)?.Name;
		if (!string.IsNullOrEmpty(initialStateName))
		{
			this.QueueTransition(initialStateName);
		}
	}

	public async Task Stop()
	{
		this.QueueTransition(null);
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
			StateMachineId = this.GetInstanceId(),
		};
		this.CallDeferred(MethodName.PerformTransition, this.OngoingTransition);
	}

	private void PerformTransition(StateTransitionData transition)
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
					this.DebugLog("🔀", this.ActiveState.Name, "❌ →", transition.NextStateName ?? "null", "(canceled on exit)");
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
			GD.PrintS(Time.GetTicksMsec(), nameof(StateMachine), this.GetParent().Name, ":", string.Join(" ", args));
		}
	}
}
