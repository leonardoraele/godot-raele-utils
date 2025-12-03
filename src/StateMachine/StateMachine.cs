using System;
using System.Collections.Generic;
using Godot;

namespace Raele.GodotUtils.StateMachine;

public class StateMachine<T> where T : StateMachine<T>.IState
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
		public void EnterState(TransitionRecord transition);
		public void ExitState(TransitionRecord transition);
	}

	public record TransitionRecord
	{
		public required StateMachine<T> StateMachine { get; init; }
		public required T? StateIn { get; init; }
		public required T? StateOut { get; init; }
		public Variant Data { get; init; } = new Variant();
		public bool Canceled { get; private set; } = false;
		public void Cancel() => this.Canceled = true;
	}

	// -----------------------------------------------------------------------------------------------------------------
	// FIELDS
	// -----------------------------------------------------------------------------------------------------------------

	public T? ActiveState { get; private set; }
	public T? PreviousState { get; private set; }
	public ulong LastStateTransitionTimestamp { get; private set; } = 0;

	// -----------------------------------------------------------------------------------------------------------------
	// PROPERTIES
	// -----------------------------------------------------------------------------------------------------------------

	public TimeSpan ActiveStateDuration => TimeSpan.FromMilliseconds(Time.GetTicksMsec() - this.LastStateTransitionTimestamp);

	// -----------------------------------------------------------------------------------------------------------------
	// EVENTS
	// -----------------------------------------------------------------------------------------------------------------

	public event Action<TransitionRecord>? BeforeStateExit;
	public event Action<TransitionRecord>? BeforeStateEnter;
	public event Action<TransitionRecord>? TransitionCompleted;

	// -----------------------------------------------------------------------------------------------------------------
	// METHODS
	// -----------------------------------------------------------------------------------------------------------------

	public void Transition(T state, Variant data = new Variant())
	{
		TransitionRecord transition = new()
		{
			StateMachine = this,
			StateOut = this.ActiveState,
			StateIn = state,
			Data = data
		};

		this.TransitionOut(transition);

		if (transition.Canceled)
		{
			return;
		}

		this.TransitionIn(transition);

		this.LastStateTransitionTimestamp = Time.GetTicksMsec();

		if (!transition.Canceled)
		{
			this.TransitionCompleted?.Invoke(transition);
		}
	}

	private void TransitionOut(TransitionRecord transition)
	{
		if (transition.StateOut == null)
		{
			return;
		}
		this.BeforeStateExit?.Invoke(transition);
		if (transition.Canceled)
		{
			return;
		}
		try
		{
			transition.StateIn?.EnterState(transition);
		}
		catch (Exception e)
		{
			GD.PushError(e);
		}
		if (transition.Canceled)
		{
			return;
		}
		this.PreviousState = this.ActiveState;
		this.ActiveState = default;
	}

	private void TransitionIn(TransitionRecord transition)
	{
		this.BeforeStateEnter?.Invoke(transition);
		if (transition.Canceled)
		{
			return;
		}
		try
		{
			transition.StateIn?.EnterState(transition);
		}
		catch (Exception e)
		{
			GD.PushError(e);
		}
		if (transition.Canceled)
		{
			return;
		}
		this.ActiveState = transition.StateIn;
	}
}
