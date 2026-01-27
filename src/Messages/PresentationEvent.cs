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
public partial class PresentationEvent : GenericMessage
{
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
	/// Adds a new task to represent a handler's presentation of this event, and returns an action that the handler
	/// can call to mark the task as completed.
	/// </summary>
	public Action AddTask()
	{
		TaskCompletionSource source = new();
		this.AddTask(source.Task);
		return source.SetResult;
	}

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
	/// Adds a new task that starts only after all previously registered tasks have completed.
	///
	/// The callback task function is called when it's time to start the task, and is provided with a callback function
	/// that the handler can call to mark the task as completed.
	/// </summary>
	public void AddTaskLate(Action<Action> task)
		=> this.AddTask(
			this.Completed.ContinueWith(_ =>
				{
					TaskCompletionSource source = new();
					task(source.SetResult);
					return source.Task;
				})
				.Unwrap()
		);

	/// <summary>
	/// Adds a new task that starts only after all previously registered tasks have completed.
	///
	/// The callback task function is called when it's time to start the task, and is provided with two callback
	/// functions: one to mark the task as completed successfully, and another to mark it as failed with an exception.
	/// </summary>
	public void AddTaskLate(Action<Action, Action<Exception>> task)
		=> this.AddTask(
			this.Completed.ContinueWith(_ =>
				{
					TaskCompletionSource source = new();
					task(source.SetResult, source.SetException);
					return source.Task;
				})
				.Unwrap()
		);

	/// <summary>
	/// Adds a new task that starts only after all previously registered tasks have completed.
	///
	/// The callback task function is called when it's time to start the task, and is provided with two callback
	/// functions: one to mark the task as completed successfully, and another to mark it as canceled.
	/// </summary>
	public void AddTaskLate(Action<Action, Action> task)
		=> this.AddTask(
			this.Completed.ContinueWith(_ =>
				{
					TaskCompletionSource source = new();
					task(source.SetResult, source.SetCanceled);
					return source.Task;
				})
				.Unwrap()
		);

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
}
