using System;
using System.Threading.Tasks;
using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.Messages;

/// <summary>
/// An event with an interface that allows handlers to register asynchronous handling work that the emitter can wait
/// for before continuing. This is useful if the game logic wants to wait for presentation work to be done before
/// continuing.
/// </summary>
public partial class AsyncEvent : Event
{
	//==================================================================================================================
	// FIELDS
	//==================================================================================================================

	/// <summary>
	/// Event handlers can use this token to register a callback to be notified when the emitter has stopped waiting for
	/// the event.
	///
	/// Ideally, handlers should use this token to cancel any ongoing presentation work if the event has been cancelled.
	/// However, this is not enforced, as we can't ensure the handlers will respect the token.
	/// </summary>
	public GodotCancellationToken CancellationToken
	{
		get => field ??= GodotCancellationToken.None;
		init
		{
			field = value;
			field.Register(() => this.Completed = Task.FromCanceled(field.BackingToken));
		}
	}

	/// <summary>
	/// The task that wraps the tasks registered by handlers. When this task ends, all registered tasks have completed.
	/// </summary>
	private Task Completed { get; set; } = Task.CompletedTask;

	//==================================================================================================================
	// METHODS
	//==================================================================================================================

	/// <summary>
	/// Adds a task that represents the work being done by one of this event's handlers. This should be called by an
	/// event handler so that the emitter can wait for the event handling completion.
	/// </summary>
	public void AddTask(Task task)
		=> this.Completed = this.Completed.ContinueWith(_ => task).Unwrap();

	public void AddTask(Func<Task> task)
		=> this.AddTask(task());

	/// <summary>
	/// Adds a tween as a task representing a handler's work.
	/// </summary>
	public void AddTween(Tween tween)
		=> this.AddTask(tween.ToSignal(tween, Tween.SignalName.Finished).ToTask());

	/// <summary>
	/// Adds a sequential task that starts only after all previously registered tasks have completed. This is useful for
	/// chaining multiple steps of presentation that must occur in sequence.
	/// </summary>
	public void AddTaskLate(Func<Task> task)
		=> this.AddTask(this.Completed.ContinueWith(_ => task()).Unwrap());

	/// <summary>
	/// Adds a tween as a late task representing a handler's presentation of this event. The tween starts only after
	/// all previously registered tasks have completed, and the task completes when the tween finishes.
	/// </summary>
	public void AddTweenLate(Tween tween)
	{
		tween.Stop();
		this.AddTaskLate(() =>
		{
			tween.Play();
			return tween.ToSignal(tween, Tween.SignalName.Finished).ToTask();
		});
	}

	/// <summary>
	/// Publishes the event and returns a task that completes when the handlers have completed.
	/// </summary>
	public async Task PublishAndWait(GodotCancellationToken? token = null)
	{
		this.Publish();
		await this.WhenCompleted(token);
	}

	/// <summary>
	/// Returns a task that completes when the presentation of this event has completed. i.e. When all tasks registered
	/// by event handlers have completed.
	///
	/// If the passed token is triggered, the returned task is cancelled before the handlers have completed. This has no
	/// effect on event handlers.
	/// </summary>
	public async Task WhenCompleted(GodotCancellationToken? token = null)
	{
		token ??= GodotCancellationToken.None;
		Task localCompleted;
		do await (localCompleted = this.Completed).WaitAsync(token.BackingToken);
		while (localCompleted != this.Completed);
	}
}
