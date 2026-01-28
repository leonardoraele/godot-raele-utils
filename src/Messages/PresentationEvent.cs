using System;
using System.Threading.Tasks;
using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.Messages;

/// <summary>
/// An event that is part of a presentation sequence (like a cutscene or dialog). Handlers can register tasks that
/// represent the presentation of the event to the user. The registered tasks as compiled into the
/// <see cref="Completed"/> task, which completes when all registered tasks have completed; i.e. when all presentation
/// handlers have finished presenting the event.
///
/// The message bus should queue dispatched presentation events and deliver them to handlers one at a time in the order
/// they were published, waiting for each event's <see cref="Completed"/> task to complete before delivering the next,
/// so that presentation events are executed sequentially.
/// </summary>
public partial class PresentationEvent : Event
{
	/// <summary>
	/// By default, the message bus will wait for a presentation event to complete before publishing the next one.
	/// this flag is set to `true`, the message bus will publish this event in parallel with any others that are
	/// currently being presented.
	///
	/// Note: This method allows this event to run simultaneously with the *previous* event. To run simultaneously with
	/// a subsequent event, either this event's handlers should not call AddTask() or AddTween() methods, or the
	/// subsequent event must have this flag set to `true`.
	/// </summary>
	public bool Parallel { get; init; } = false;

	/// <summary>
	/// A task that completes when all handlers have finished presenting this event.
	/// </summary>
	public Task Completed { get; private set; } = Task.CompletedTask;

	/// <summary>
	/// Adds a task that represents a handler's presentation of this event. Handlers must call this method to register
	/// their work. This event's <see cref="Completed"/> task completes when all registered tasks have completed.
	/// </summary>
	public void AddTask(Task task)
		=> this.Completed = this.Completed.ContinueWith(_ => task).Unwrap();

	public void AddTask(Func<Task> task)
		=> this.AddTask(task());

	/// <summary>
	/// Adds a tween as a task representing a handler's presentation of this event. The task completes when the tween
	/// finishes.
	/// </summary>
	public void AddTween(Tween tween)
		=>this.AddTask(tween.ToSignal(tween, Tween.SignalName.Finished).ToTask());

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
		this.AddTaskLate(async () =>
		{
			tween.Play();
			await tween.ToSignal(tween, Tween.SignalName.Finished).ToTask();
		});
	}

	protected override void _Publish()
		=> MessageBus.Singleton.DispatchPresentationEvent(this);
}
