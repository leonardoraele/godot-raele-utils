using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Raele.GodotUtils.Extensions;

namespace Raele.GodotUtils.Messages;

public partial class MessageBus : Node
{
	//==================================================================================================================
		#region STATICS
	//==================================================================================================================

	public static MessageBus Singleton
	{
		get
		{
			if (field == null)
			{
				field = new();
				Engine.GetSceneTree().Root.AddChild(field);
			}
			return field;
		}
		private set;
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region FIELDS
	//==================================================================================================================

	// TODO Create a way to enable/disable debug logging without having to recompile the project
	public bool DebugEnabled = true;
	// private readonly PresentationEventsState _PresentationEvents = new();

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region COMPUTED PROPERTIES
	//==================================================================================================================

	// public IPublicPresentationEventsState PresentationEvents => this._PresentationEvents;

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region EVENTS & SIGNALS
	//==================================================================================================================

	[Signal] public delegate void MessagePublishedEventHandler(Message message);

	[Signal] public delegate void BeforeCommandPublishedEventHandler(Command commnad);
	[Signal] public delegate void CommandPublishedEventHandler(Command commnad);

	// [Signal] public delegate void PresentationSequenceStartedEventHandler();
	// [Signal] public delegate void PresentationEventStartedEventHandler(AsyncEvent @event);
	// [Signal] public delegate void PresentationEventFinishedEventHandler(AsyncEvent @event);
	// [Signal] public delegate void PresentationSequenceFinishedEventHandler();

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region INTERNAL TYPES
	//==================================================================================================================

	// public interface IPublicPresentationEventsState
	// {
	// 	public TimeSpan Timeout { get; set; }
	// 	public void Stop();
	// 	public Task WaitSettled();
	// 	public Task StopAndWait();
	// }

	// private partial class PresentationEventsState : GodotObject, IPublicPresentationEventsState
	// {
	// 	public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
	// 	public readonly HashSet<AsyncEvent> Ongoing = [];
	// 	public readonly List<AsyncEvent> Queue = [];
	// 	public bool IsSettled => this.Queue.Count == 0 && this.Ongoing.Count == 0;
	// 	public void Stop()
	// 	{
	// 		this.Ongoing.ForEach(@event => @event.Cancel());
	// 		this.Ongoing.Clear();
	// 		this.Queue.Clear();
	// 	}
	// 	public async Task StopAndWait()
	// 	{
	// 		this.Stop();
	// 		await this.WaitSettled();
	// 	}
	// 	public async Task WaitSettled()
	// 		=> await Singleton.ToSignal(Singleton, MessageBus.SignalName.PresentationSequenceFinished);
	// }

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region OVERRIDES & VIRTUALS
	//==================================================================================================================

	public override void _Ready()
	{
		base._Ready();
		if (this.DebugEnabled)
			this.MessagePublished += message => this.DebugLog($"⚡ {message}", message.MessageId);
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region METHODS
	//==================================================================================================================

	public async IAsyncEnumerable<Message> GetMessageStream(GodotCancellationToken token)
	{
		while(true)
		{
			Variant[] args;
			try
				{ args = await this.ToSignal(this, SignalName.MessagePublished).ToTask(token); }
			catch (TaskCanceledException)
				{ break; }
			Message message = (Message) args[0].AsGodotObject();
			yield return message;
		}
	}

	public void Dispatch(Message message)
	{
		this.CallDeferred(GodotObject.MethodName.EmitSignal, SignalName.MessagePublished, message);
		if (message is Command command)
			this.DispatchCommand(command);
	}

	private void DispatchCommand(Command command)
	{
		Callable.From(_DispatchCommand).CallDeferred();
		void _DispatchCommand()
		{
			this.CallSafe(GodotObject.MethodName.EmitSignal, [SignalName.BeforeCommandPublished, command]);
			if (command.Cancelled)
				return;
			this.CallDeferred(GodotObject.MethodName.EmitSignal, SignalName.CommandPublished, command);
			command.Execute();
		}
	}

	// private void DispatchPresentationEvent(AsyncEvent @event)
	// {
	// 	this.Dispatch(@event);
	// 	lock (this._PresentationEvents)
	// 	{
	// 		if (this._PresentationEvents.IsSettled)
	// 		{
	// 			this.CallDeferred(GodotObject.MethodName.EmitSignal, SignalName.PresentationSequenceStarted);
	// 			this.CallDeferred(MethodName.ProcessPresentationEvent, @event);
	// 		}
	// 		else if (@event.Parallel && this._PresentationEvents.Queue.Count == 0)
	// 			this.CallDeferred(MethodName.ProcessPresentationEvent, @event);
	// 		this._PresentationEvents.Queue.Add(@event);
	// 	}
	// }

	// private async void ProcessPresentationEvent(AsyncEvent @event)
	// {
	// 	TimeSpan timeout;
	// 	lock (this._PresentationEvents)
	// 	{
	// 		this._PresentationEvents.Queue.Remove(@event);
	// 		if (this._PresentationEvents.Ongoing.Contains(@event))
	// 			return;
	// 		this._PresentationEvents.Ongoing.Add(@event);
	// 		timeout = this._PresentationEvents.Timeout;
	// 	}
	// 	this.CallSafe(GodotObject.MethodName.EmitSignal, SignalName.PresentationEventStarted, @event);
	// 	try
	// 		{ await @event.Completed.WaitAsync(timeout); }
	// 	catch (TimeoutException)
	// 		{ GD.PushWarning($"{nameof(MessageBus)}: {nameof(AsyncEvent)} timed out after {timeout}. Event: {@event}."); }
	// 	this.CallSafe(GodotObject.MethodName.EmitSignal, SignalName.PresentationEventFinished, @event);
	// 	lock (this._PresentationEvents)
	// 	{
	// 		this._PresentationEvents.Ongoing.Remove(@event);
	// 		if (this._PresentationEvents.Ongoing.Count > 0)
	// 			return;
	// 		if (this._PresentationEvents.Queue.Count > 0)
	// 			this.CallDeferred(MethodName.ProcessPresentationEvent, this._PresentationEvents.Queue.First());
	// 		else
	// 			this.EmitSignalPresentationSequenceFinished();
	// 	}
	// }

	//==================================================================================================================
		#endregion
	//==================================================================================================================
}
