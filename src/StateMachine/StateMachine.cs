using System;
using Godot;

namespace Raele.GodotUtils.StateMachine;

public partial class StateMachine<T> where T : StateMachine<T>.IState
{
	// // -----------------------------------------------------------------------------------------------------------------
	// // EXPORTS
	// // -----------------------------------------------------------------------------------------------------------------

	// [Export] public bool AutoStart = true;
	// [Export] public Node? InitialState = null;

	// [ExportGroup("Debug")]
	// [Export] public bool EnableLogging = false;

	// -----------------------------------------------------------------------------------------------------------------
	// STATICS
	// -----------------------------------------------------------------------------------------------------------------

	public interface IState
	{
		public void EnterState(Transition transition);
		public void ExitState(Transition transition);
	}

	public partial class Transition : GodotObject
	{
		public required StateMachine<T> StateMachine { get; init; }
		public required T? EnterState { get; init; }
		public required T? ExitState { get; init; }
		public Variant Data { get; init; } = new Variant();
		public bool IsCanceled { get; private set; } = false;
		public event Action? Canceled;
		public void Cancel()
		{
			this.IsCanceled = true;
			this.Canceled?.Invoke();
		}
	}

	// -----------------------------------------------------------------------------------------------------------------
	// FIELDS
	// -----------------------------------------------------------------------------------------------------------------

	public T? ActiveState { get; private set; }
	public T? PreviousActiveState { get; private set; }
	public ulong LastStateTransitionTimestamp { get; private set; } = 0;
	private Transition? OngoingTransition = null;
	private Transition? QueuedTransition = null;
	private Callable PerformTransitionCallable => !field.Equals(default(Callable))
		? field
		: (field = Callable.From(this.PerformTransition));

	// -----------------------------------------------------------------------------------------------------------------
	// COMPUTED PROPERTIES
	// -----------------------------------------------------------------------------------------------------------------

	public TimeSpan ActiveStateDuration => TimeSpan.FromMilliseconds(Time.GetTicksMsec() - this.LastStateTransitionTimestamp);

	// -----------------------------------------------------------------------------------------------------------------
	// EVENTS
	// -----------------------------------------------------------------------------------------------------------------

	public event Action? Started;
	public event Action<Transition>? BeforeStateExit;
	public event Action<Transition>? BeforeStateEnter;
	public event Action<Transition>? TransitionCanceled;
	public event Action<Transition>? TransitionCompleted;
	public event Action? Stopped;

	// -----------------------------------------------------------------------------------------------------------------
	// METHODS
	// -----------------------------------------------------------------------------------------------------------------

	public void QueueTransition(T? state, Variant data = new Variant())
	{
		if (Engine.IsEditorHint())
		{
			return;
		}
		this.OngoingTransition?.Cancel();
		this.QueuedTransition?.Cancel();
		this.QueuedTransition = new()
		{
			StateMachine = this,
			ExitState = this.ActiveState,
			EnterState = state,
			Data = data
		};
		this.PerformTransitionCallable.CallDeferred();
	}

	public void Stop() => this.QueueTransition(default);

	private void PerformTransition()
	{
		if (this.QueuedTransition == null || this.QueuedTransition.IsCanceled)
		{
			return;
		}

		if (this.OngoingTransition != null)
		{
			throw new Exception("Failed to perform state transition. Cause: A state transition is already ongoing. This is a bug.");
		}

		Transition transition = this.OngoingTransition = this.QueuedTransition;
		this.QueuedTransition = null;

		this.TransitionOut(transition);

		if (transition.IsCanceled)
		{
			this.OngoingTransition = null;
			this.TransitionCanceled?.Invoke(transition);
			return;
		}

		this.PreviousActiveState = this.ActiveState;
		this.ActiveState = transition.EnterState;

		this.TransitionIn(transition);

		this.LastStateTransitionTimestamp = Time.GetTicksMsec();
		this.OngoingTransition = null;

		if (transition.IsCanceled)
		{
			this.ActiveState = default;
			try
			{
				this.TransitionCanceled?.Invoke(transition);
			}
			catch (Exception e)
			{
				GD.PushError(e);
			}
		}
		else
		{
			try
			{
				this.TransitionCompleted?.Invoke(transition);
			}
			catch (Exception e)
			{
				GD.PushError(e);
			}
			if (transition.ExitState == null && transition.EnterState != null)
			{
				this.Started?.Invoke();
			}
		}

		if (this.ActiveState == null && this.QueuedTransition == null)
		{
			this.Stopped?.Invoke();
		}
	}

	private void TransitionOut(Transition transition)
	{
		if (transition.ExitState == null)
		{
			return;
		}
		try
		{
			this.BeforeStateExit?.Invoke(transition);
		}
		catch (Exception e)
		{
			GD.PushError(e);
		}
		if (transition.IsCanceled)
		{
			return;
		}
		try
		{
			transition.ExitState?.ExitState(transition);
		}
		catch (Exception e)
		{
			GD.PushError(e);
		}
	}

	private void TransitionIn(Transition transition)
	{
		if (transition.EnterState == null)
		{
			return;
		}
		try
		{
			this.BeforeStateEnter?.Invoke(transition);
		}
		catch (Exception e)
		{
			GD.PushError(e);
		}
		if (transition.IsCanceled)
		{
			return;
		}
		try
		{
			transition.EnterState?.EnterState(transition);
		}
		catch (Exception e)
		{
			GD.PushError(e);
		}
	}
}
