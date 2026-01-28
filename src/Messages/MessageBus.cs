using System;
using System.Collections.Generic;
using System.Linq;
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

	public bool DebugEnabled = false;
	private readonly PresentationEventsState PresentationEvents = new();

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region COMPUTED PROPERTIES
	//==================================================================================================================

	public IPresentationEventsConfig PresentationEventsConfig => this.PresentationEvents;

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region EVENTS & SIGNALS
	//==================================================================================================================

	[Signal] public delegate void MessagePublishedEventHandler(Message message);

	[Signal] public delegate void BeforeCommandPublishedEventHandler(Command commnad);
	[Signal] public delegate void CommandPublishedEventHandler(Command commnad);

	[Signal] public delegate void PresentationSequenceStartedEventHandler();
	[Signal] public delegate void PresentationEventStartedEventHandler(PresentationEvent @event);
	[Signal] public delegate void PresentationEventFinishedEventHandler(PresentationEvent @event);
	[Signal] public delegate void PresentationSequenceFinishedEventHandler();

	//==================================================================================================================
		#endregion
	//==================================================================================================================
		#region INTERNAL TYPES
	//==================================================================================================================

	public interface IPresentationEventsConfig
	{
		public TimeSpan Timeout { get; set; }
	}

	private partial class PresentationEventsState : GodotObject, IPresentationEventsConfig
	{
		public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
		public readonly HashSet<PresentationEvent> Ongoing = [];
		public readonly List<PresentationEvent> Queue = [];
		public bool IsSettled => this.Queue.Count == 0 && this.Ongoing.Count == 0;
	}

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

	public void Dispatch(Message message)
		=> this.CallDeferred(GodotObject.MethodName.EmitSignal, SignalName.MessagePublished, message);

	public void DispatchCommand(Command command)
	{
		this.Dispatch(command);
		this.CallDeferred(MethodName.ProcessCommand, command);
	}

	private void ProcessCommand(Command command)
	{
		this.CallSafe(GodotObject.MethodName.EmitSignal, SignalName.BeforeCommandPublished, command);
		if (command.Cancelled)
			return;
		this.CallDeferred(GodotObject.MethodName.EmitSignal, SignalName.CommandPublished, command);
		command.Execute();
	}

	public void DispatchPresentationEvent(PresentationEvent @event)
	{
		this.Dispatch(@event);
		lock (this.PresentationEvents)
		{
			if (this.PresentationEvents.IsSettled)
			{
				this.CallDeferred(GodotObject.MethodName.EmitSignal, SignalName.PresentationSequenceStarted);
				this.CallDeferred(MethodName.ProcessPresentationEvent, @event);
			}
			else if (@event.Parallel && this.PresentationEvents.Queue.Count == 0)
				this.CallDeferred(MethodName.ProcessPresentationEvent, @event);
			this.PresentationEvents.Queue.Add(@event);
		}
	}

	private async void ProcessPresentationEvent(PresentationEvent @event)
	{
		TimeSpan timeout;
		lock (this.PresentationEvents)
		{
			this.PresentationEvents.Queue.Remove(@event);
			if (this.PresentationEvents.Ongoing.Contains(@event))
				return;
			this.PresentationEvents.Ongoing.Add(@event);
			timeout = this.PresentationEvents.Timeout;
		}
		this.CallSafe(GodotObject.MethodName.EmitSignal, SignalName.PresentationEventStarted, @event);
		try
			{ await @event.Completed.WaitAsync(timeout); }
		catch (TimeoutException)
			{ GD.PushWarning($"{nameof(MessageBus)}: {nameof(PresentationEvent)} timed out after {this.PresentationEvents.Timeout}. Event: {@event}."); }
		this.CallSafe(GodotObject.MethodName.EmitSignal, SignalName.PresentationEventFinished, @event);
		lock (this.PresentationEvents)
		{
			this.PresentationEvents.Ongoing.Remove(@event);
			if (this.PresentationEvents.Ongoing.Count > 0)
				return;
			if (this.PresentationEvents.Queue.Count > 0)
				this.CallDeferred(MethodName.ProcessPresentationEvent, this.PresentationEvents.Queue.First());
			else
				this.EmitSignalPresentationSequenceFinished();
		}
	}

	//==================================================================================================================
		#endregion
	//==================================================================================================================
}
